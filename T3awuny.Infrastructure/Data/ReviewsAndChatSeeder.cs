using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using T3awuny.Core.Entities.ChatModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.ReviewModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Infrastructure.Data;

namespace T3awuny.Infrastructure.Seed
{
    public class ReviewsAndChatSeeder
    {
        private readonly T3awunyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsAndChatSeeder(
            T3awunyDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (await _context.Reviews.AnyAsync() ||
                await _context.Conversations.AnyAsync())
                return;

            // ── Fetch ONLY real users (non-null Name) by role ──
            var allFarmers = await _userManager.GetUsersInRoleAsync("Farmer");
            var allTraders = await _userManager.GetUsersInRoleAsync("Trader");

            // Filter out any ghost users that have null Name
            var farmers = allFarmers.Where(u => !string.IsNullOrEmpty(u.Name)).ToList();
            var traders = allTraders.Where(u => !string.IsNullOrEmpty(u.Name)).ToList();

            if (!farmers.Any())
                throw new Exception("No valid farmers found. Fix user seeding first.");
            if (!traders.Any())
                throw new Exception("No valid traders found. Fix user seeding first.");

            Console.WriteLine($"Found {farmers.Count} valid farmers and {traders.Count} valid traders");

            await SeedReviewsAsync(farmers, traders);
            await SeedChatAsync(farmers, traders);
        }

        // ══════════════════════════════════════════════════
        // REVIEWS
        // ══════════════════════════════════════════════════
        private async Task SeedReviewsAsync(
            List<ApplicationUser> farmers,
            List<ApplicationUser> traders)
        {
            // Fetch delivered order IDs only — we only need the ID for the FK
            // We do NOT use order.BuyerId or order.FarmerId (they may point to ghost users)
            var deliveredOrderIds = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .Select(o => o.Id)
                .ToListAsync();

            if (!deliveredOrderIds.Any())
                throw new Exception("No delivered orders found. Run main DataSeeder first.");

            var comments = new[]
            {
                "منتج ممتاز وطازج جداً، سأشتري مرة أخرى بالتأكيد",
                "الجودة عالية والتوصيل في الموعد المحدد",
                "تعامل راقي والمنتج طبق الوصف تماماً",
                "سعر مناسب جداً مقارنة بالجودة المقدمة",
                "المزارع محترف جداً ومنتجاته دائماً طازجة",
                "أنصح الجميع بالتعامل مع هذا المزارع",
                "المنتج وصل في حالة ممتازة والتغليف احترافي",
                "جودة استثنائية وأسعار تنافسية",
                "تجربة تسوق رائعة وسأكرر التعامل",
                "المنتج طازج والكميات دقيقة كما طلبت"
            };

            var reviews = new List<Review>();
            var reviewedOrders = new HashSet<int>(); // one review per order

            int limit = Math.Min(deliveredOrderIds.Count, 100);

            for (int i = 0; i < limit; i++)
            {
                var orderId = deliveredOrderIds[i];
                if (reviewedOrders.Contains(orderId)) continue;

                // ── Use REAL verified users by index — never from order FK ──
                var reviewer = traders[i % traders.Count];   // trader reviews farmer
                var target = farmers[i % farmers.Count];   // farmer being reviewed

                reviews.Add(new Review
                {
                    ReviewerId = reviewer.Id,   // real trader
                    TargetUserId = target.Id,     // real farmer
                    OrderId = orderId,        // real delivered order ID
                    Rating = Random.Shared.Next(3, 6),
                    Comment = comments[i % comments.Length],
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30))
                });

                reviewedOrders.Add(orderId);
            }

            await _context.Reviews.AddRangeAsync(reviews);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded {reviews.Count} reviews");
        }

        // ══════════════════════════════════════════════════
        // CONVERSATIONS + MESSAGES
        // ══════════════════════════════════════════════════
        private async Task SeedChatAsync(
            List<ApplicationUser> farmers,
            List<ApplicationUser> traders)
        {
            var conversations = new List<Conversation>();
            var convPairs = new HashSet<string>();

            for (int i = 0; i < 50; i++)
            {
                // ── Direct index access — no random, no do-while, no null risk ──
                var farmer = farmers[i % farmers.Count];
                var trader = traders[i % traders.Count];
                var farmerId = farmer.Id;
                var traderId = trader.Id;

                // Normalize to prevent (A,B) and (B,A) duplicates
                var firstId = string.Compare(farmerId, traderId) < 0 ? farmerId : traderId;
                var secondId = string.Compare(farmerId, traderId) < 0 ? traderId : farmerId;
                var pairKey = $"{firstId}_{secondId}";

                if (convPairs.Contains(pairKey)) continue;
                convPairs.Add(pairKey);

                conversations.Add(new Conversation
                {
                    User1Id = firstId,
                    User2Id = secondId,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60)),
                    LastMessageAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 5))
                });
            }

            await _context.Conversations.AddRangeAsync(conversations);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded {conversations.Count} conversations");

            // Fetch saved conversations with real IDs from DB
            var savedConversations = await _context.Conversations
                .OrderByDescending(c => c.CreatedAt)
                .Take(conversations.Count)
                .ToListAsync();

            var msgTemplates = new[]
            {
                "مرحباً، أنا مهتم بمنتجاتك، هل متاح للتفاوض؟",
                "نعم بالتأكيد، ما الكمية التي تحتاجها؟",
                "نحتاج حوالي 500 كيلو أسبوعياً بشكل منتظم",
                "ممتاز، سعرنا للكميات الكبيرة يختلف، سأرسل لك العرض",
                "شكراً، هل يمكن الاطلاع على جودة المنتج أولاً؟",
                "بالطبع، يمكنك زيارة المزرعة أي وقت أو نرسل لك عينة",
                "متى يمكن بدء التوريد؟",
                "يمكنني البدء من الأسبوع القادم إن شاء الله",
                "هل التوصيل متاح لمنطقة القاهرة؟",
                "نعم، لدينا سيارة توصيل مخصصة للقاهرة يومياً",
                "ممتاز جداً، سنتفق على العقد إذن",
                "بإذن الله، سأرسل لك العقد للمراجعة قريباً",
            };

            var messages = new List<Message>();

            foreach (var conv in savedConversations)
            {
                int msgCount = Random.Shared.Next(8, 20);
                for (int m = 0; m < msgCount; m++)
                {
                    messages.Add(new Message
                    {
                        ConversationId = conv.Id,
                        SenderId = m % 2 == 0 ? conv.User1Id : conv.User2Id,
                        Content = msgTemplates[m % msgTemplates.Length],
                        IsRead = true,
                        SentAt = conv.CreatedAt.AddHours(m + 1)
                    });
                }
            }

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded {messages.Count} messages");
        }
    }
}