using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.ChatModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.ProductModule;
using T3awuny.Core.Entities.ReviewModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Infrastructure.Data;

namespace T3awuny.Infrastructure.Seed
{
    public class DataSeeder
    {
        private readonly T3awunyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public DataSeeder(T3awunyDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            //_roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task SeedAsync()
        {
            // Guard — only seed if DB is empty
            if (await _context.Users.AnyAsync()) return;//////////////////////////////////////////////////////////////

            //await SeedRolesAsync();
            var users = await SeedUsersAsync();
            await SeedAddressesAsync(users);
            //await SeedCategoriesAsync();
            await SeedProductsAsync(users);
            await SeedOrdersAsync(users);
            await SeedAuctionsAsync(users);
            await SeedReviewsAsync(users);
            //await SeedReportsAsync(users);
            //await SeedCommunityAsync(users);
            await SeedChatAsync(users);
        }

        // ══════════════════════════════════════════════════
        // STEP 1 — ROLES
        // ══════════════════════════════════════════════════
        //private async Task SeedRolesAsync()
        //{
        //    string[] roles = { "Admin", "Farmer", "Trader" };
        //    foreach (var role in roles)
        //        if (!await _roleManager.RoleExistsAsync(role))
        //            await _roleManager.CreateAsync(new IdentityRole(role));
        //}

        // ══════════════════════════════════════════════════
        // STEP 2 — USERS
        // ══════════════════════════════════════════════════
        private async Task<Dictionary<string, ApplicationUser>> SeedUsersAsync()
        {
            var users = new Dictionary<string, ApplicationUser>();

            // ── Admin ────────────────────────────────────
            var admin = new ApplicationUser
            {
                UserName = "admin@t3awuny.com",
                Email = "admin@t3awuny.com",
                Name = "مدير النظام",
                PhoneNumber = "01000000000",
                IsActive = true,
                IsVerified = true,
                JoinDate = DateTime.UtcNow.AddMonths(-6),
                Addresses = new List<Address> { new Address { Street = "التعاونيات", City = "مدينة الفيوم", Governorate = "الفيوم", Country = "مصر" } },
                EmailConfirmed = true
            };
            var result =  await _userManager.CreateAsync(admin, "Admin@123456");
            if (!result.Succeeded)
                throw new Exception($"Admin creation failed: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            await _userManager.AddToRoleAsync(admin, "Admin");
            users["admin"] = admin;

            // ── Farmers ──────────────────────────────────
            var farmerData = new[]
            {
                ("ahmed.farmer@t3awuny.com",  "أحمد محمد السيد",    "01001234567"),
                ("mohamed.ali@t3awuny.com",   "محمد علي إبراهيم",   "01012345678"),
                ("hassan.omar@t3awuny.com",   "حسن عمر عبدالله",    "01023456789"),
                ("youssef.nour@t3awuny.com",  "يوسف نور الدين",     "01034567890"),
                ("ibrahim.saad@t3awuny.com",  "إبراهيم سعد مصطفى",  "01045678901"),
                ("karim.fadl@t3awuny.com",    "كريم فضل الله",      "01056789012"),
                ("tarek.mansour@t3awuny.com", "طارق منصور أحمد",    "01067890123"),
                ("walid.ragab@t3awuny.com",   "وليد رجب محمود",     "01078901234"),
                ("sameh.attia@t3awuny.com",   "سامح عطية سالم",     "01089012345"),
                ("nasser.zaki@t3awuny.com",   "ناصر زكي حسين",      "01090123456"),
                ("fathy.bakr@t3awuny.com",    "فتحي بكر عثمان",     "01101234567"),
                ("emad.helal@t3awuny.com",    "عماد هلال جمعة",     "01112345678"),
                ("gamal.sobhy@t3awuny.com",   "جمال صبحي رمضان",    "01123456789"),
                ("hany.issa@t3awuny.com",     "هاني عيسى سليمان",   "01134567890"),
                ("magdy.tawfik@t3awuny.com",  "مجدي توفيق فؤاد",    "01145678901"),
                ("ramzy.shafik@t3awuny.com",  "رمزي شفيق نجيب",     "01156789012"),
                ("sayed.hakim@t3awuny.com",   "سيد حكيم لطفي",      "01167890123"),
                ("adel.barakat@t3awuny.com",  "عادل بركات منير",    "01178901234"),
                ("fouad.nassar@t3awuny.com",  "فؤاد نصار وهبة",     "01189012345"),
                ("lotfy.samir@t3awuny.com",   "لطفي سمير عزت",      "01190123456"),
            };

            for (int i = 0; i < farmerData.Length; i++)
            {
                var (email, name, phone) = farmerData[i];
                var farmer = new ApplicationUser
                {
                    UserName = email.Substring(0, email.IndexOf(".")),
                    Email = email,
                    Name = name,
                    PhoneNumber = phone,
                    IsActive = true,
                    IsVerified = true,
                    JoinDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 180)),
                    EmailConfirmed = true
                };
                var farmerResult = await _userManager.CreateAsync(farmer, "Farmer@123456");
                if (!farmerResult.Succeeded)
                    throw new Exception($"Farmer {email} failed: " +
                        string.Join(", ", farmerResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(farmer, "Farmer");
                users[$"farmer_{i + 1}"] = farmer;
            }

            // ── Traders ───────────────────────────────────
            var traderData = new[]
            {
                ("trader1@t3awuny.com",  "شركة النيل للتجارة",        "02012345678"),
                ("trader2@t3awuny.com",  "مؤسسة البركة الزراعية",     "02023456789"),
                ("trader3@t3awuny.com",  "شركة الخير للاستيراد",      "02034567890"),
                ("trader4@t3awuny.com",  "مطاعم كوزينة الصحية",       "02045678901"),
                ("trader5@t3awuny.com",  "شركة الأمل للتوريدات",      "02056789012"),
                ("trader6@t3awuny.com",  "مؤسسة الرزق الحلال",        "02067890123"),
                ("trader7@t3awuny.com",  "شركة بيتى للمواد الغذائية", "02078901234"),
                ("trader8@t3awuny.com",  "مطعم الشيف الفرنسي",        "02089012345"),
                ("trader9@t3awuny.com",  "شركة سوبر ماركت الدلتا",   "02090123456"),
                ("trader10@t3awuny.com", "مؤسسة الفيوم للتجارة",      "02101234567"),
                ("trader11@t3awuny.com", "شركة الواحة الخضراء",       "02112345678"),
                ("trader12@t3awuny.com", "مطاعم طعام البلد",          "02123456789"),
                ("trader13@t3awuny.com", "شركة النضارة للخضروات",     "02134567890"),
                ("trader14@t3awuny.com", "مؤسسة الإسكندرية للتجارة", "02145678901"),
                ("trader15@t3awuny.com", "شركة الحصاد الذهبي",        "02156789012"),
            };

            for (int i = 0; i < traderData.Length; i++)
            {
                var (email, name, phone) = traderData[i];
                var trader = new ApplicationUser
                {
                    UserName = email.Substring(0, email.IndexOf("@")),
                    Email = email,
                    Name = name,
                    PhoneNumber = phone,
                    IsActive = true,
                    IsVerified = true,
                    JoinDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(20, 150)),
                    EmailConfirmed = true
                };
                var traderResult = await _userManager.CreateAsync(trader, "Trader@123456");
                if (!traderResult.Succeeded)
                    throw new Exception($"Trader {email} failed: " +
                        string.Join(", ", traderResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(trader, "Trader");
                users[$"trader_{i + 1}"] = trader;
            }

            // ── Consumers ─────────────────────────────────
            var consumerData = new[]
            {
                ("consumer1@t3awuny.com",  "سارة أحمد مصطفى",  "03012345678"),
                ("consumer2@t3awuny.com",  "نور علي حسن",      "03023456789"),
                ("consumer3@t3awuny.com",  "ريم محمد خالد",    "03034567890"),
                ("consumer4@t3awuny.com",  "دينا حسام عمر",    "03045678901"),
                ("consumer5@t3awuny.com",  "مي وائل سامي",     "03056789012"),
                ("consumer6@t3awuny.com",  "هناء جمال فريد",   "03067890123"),
                ("consumer7@t3awuny.com",  "شيرين مدحت بكر",   "03078901234"),
                ("consumer8@t3awuny.com",  "منى كمال درويش",   "03089012345"),
                ("consumer9@t3awuny.com",  "رانيا صلاح طه",    "03090123456"),
                ("consumer10@t3awuny.com", "ياسمين نبيل فاروق","03101234567"),
            };

            for (int i = 0; i < consumerData.Length; i++)
            {
                var (email, name, phone) = consumerData[i];
                var consumer = new ApplicationUser
                {
                    UserName = email.Substring(0, email.IndexOf("@")),
                    Email = email,
                    Name = name,
                    PhoneNumber = phone,
                    IsActive = true,
                    IsVerified = true,
                    JoinDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(10, 90)),
                    EmailConfirmed = true
                };
                var consumerResult = await _userManager.CreateAsync(consumer, "Trader@123456");
                if (!consumerResult.Succeeded)
                    throw new Exception($"Consumer {email} failed: " +
                        string.Join(", ", consumerResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(consumer, "Trader");
                users[$"consumer_{i + 1}"] = consumer;
            }

            // ── Farmer Profiles ───────────────────────────
            var farmData = new[]
            {
                ("مزرعة النيل الخضراء",   "الفيوم",    "القاهرة",   25.5m,  29.3084, 30.8428, "مزرعة متخصصة في زراعة الخضروات العضوية بمنطقة الفيوم"),
                ("مزرعة البركة",          "المنيا",    "المنيا",    40.0m,  28.1099, 30.7503, "إنتاج متنوع من الفاكهة والخضروات"),
                ("مزرعة الأمل",           "أسيوط",    "أسيوط",    30.0m,  27.1809, 31.1837, "تخصص في زراعة القمح والذرة"),
                ("مزرعة الخير",           "سوهاج",    "سوهاج",    35.0m,  26.5569, 31.6948, "إنتاج قصب السكر والمحاصيل الصيفية"),
                ("مزرعة السلام",          "أسوان",    "أسوان",    20.0m,  24.0889, 32.8998, "تخصص في الفواكه الاستوائية والنخيل"),
                ("مزرعة النضارة",         "الدقهلية", "الدقهلية", 50.0m,  31.0364, 31.3807, "خضروات طازجة وفواكه موسمية"),
                ("مزرعة الوادي",          "البحيرة",  "البحيرة",  45.0m,  30.8481, 30.3436, "تخصص في الألبان ومشتقاتها والخضروات"),
                ("مزرعة الفرحة",          "كفر الشيخ","كفر الشيخ",38.0m,  31.1107, 30.9388, "إنتاج الأرز والمحاصيل الشتوية"),
                ("مزرعة الرزق",           "الشرقية",  "الشرقية",  42.0m,  30.7323, 31.7185, "خضروات وفاكهة ومنتجات عضوية"),
                ("مزرعة الحصاد الذهبي",  "الغربية",  "الغربية",  55.0m,  30.8650, 31.0336, "قمح وذرة ومحاصيل حبوب"),
                ("مزرعة الطيبة",          "المنوفية", "المنوفية", 28.0m,  30.5965, 30.9876, "فراولة وعنب ومحاصيل صيفية"),
                ("مزرعة الحياة",          "الإسماعيلية","الإسماعيلية",32.0m, 30.5965, 32.2715, "خضروات مائية ومنتجات الدلتا"),
                ("مزرعة السنابل",         "بني سويف", "بني سويف", 48.0m,  29.0661, 31.0994, "قمح وعدس وبقوليات"),
                ("مزرعة الغلة",           "قنا",      "قنا",      22.0m,  26.1551, 32.7160, "تمور وفواكه صعيدية"),
                ("مزرعة الفيروز",         "مطروح",    "مطروح",    15.0m,  31.3543, 27.2373, "زيتون وتين وفواكه ساحلية"),
                ("مزرعة الواحة",          "الوادي الجديد","الوادي الجديد",60.0m, 25.4871, 29.1734, "تمور وخضروات واسعة"),
                ("مزرعة الضفة",           "قليوبية",  "قليوبية",  33.0m,  30.3292, 31.2168, "خوخ وكمثرى ومشمش"),
                ("مزرعة النور",           "السويس",   "السويس",   18.0m,  29.9668, 32.5498, "خضروات صحراوية ونباتات طبية"),
                ("مزرعة الأخضر",          "الجيزة",   "الجيزة",   27.0m,  29.9870, 31.2118, "خضروات وأعشاب طازجة"),
                ("مزرعة الكنانة",         "الدقهلية", "الدقهلية", 36.0m,  31.1650, 31.4930, "فاكهة متنوعة وعنب"),
            };

            for (int i = 0; i < farmData.Length; i++)
            {
                var (farmName, city, gov, size, lat, lng, desc) = farmData[i];
                var farmerProfile = new FarmerProfile
                {
                    FarmerId = users[$"farmer_{i + 1}"].Id,
                    FarmName = farmName,
                    Description = desc,
                    IsVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(10, 60))
                };
                await _context.FarmerProfiles.AddAsync(farmerProfile);
            }

            // ── Trader Profiles ───────────────────────────
            var traderProfileData = new[]
            {
                ("شركة النيل للتجارة",        "Wholesaler",  "123456789"),
                ("مؤسسة البركة الزراعية",     "Retailer",    "234567891"),
                ("شركة الخير للاستيراد",      "Wholesaler",  "345678912"),
                ("مطاعم كوزينة الصحية",       "Restaurant",  "456789123"),
                ("شركة الأمل للتوريدات",      "Wholesaler",  "567891234"),
                ("مؤسسة الرزق الحلال",        "Retailer",    "678912345"),
                ("شركة بيتى للمواد الغذائية", "Retailer",    "789123456"),
                ("مطعم الشيف الفرنسي",        "Restaurant",  "891234567"),
                ("شركة سوبر ماركت الدلتا",   "Retailer",    "912345678"),
                ("مؤسسة الفيوم للتجارة",      "Wholesaler",  "123456780"),
                ("شركة الواحة الخضراء",       "Wholesaler",  "234567801"),
                ("مطاعم طعام البلد",          "Restaurant",  "345678012"),
                ("شركة النضارة للخضروات",     "Retailer",    "456780123"),
                ("مؤسسة الإسكندرية للتجارة", "Wholesaler",  "567801234"),
                ("شركة الحصاد الذهبي",        "Wholesaler",  "678012345"),
            };

            for (int i = 0; i < traderProfileData.Length; i++)
            {
                var (bizName, bizType, taxNo) = traderProfileData[i];
                var traderProfile = new TraderProfile
                {
                    TraderId = users[$"trader_{i + 1}"].Id,
                    BusinessName = bizName,
                    BusinessType = (BusinessType)(Enum.TryParse(typeof(BusinessType), bizType, out var traderType) ? traderType : BusinessType.Wholesaler),
                    TaxNumber = taxNo,
                    IsVerified = true,
                    VerifiedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(5, 40))
                };
                await _context.TraderProfiles.AddAsync(traderProfile);
            }

            await _context.SaveChangesAsync();
            return users;
        }

        // ══════════════════════════════════════════════════
        // STEP 3 — ADDRESSES
        // ══════════════════════════════════════════════════
        private async Task SeedAddressesAsync(Dictionary<string, ApplicationUser> users)
        {
            var addresses = new List<Address>();

            var govData = new[]
            {
                ("القاهرة",        "مدينة نصر",       "شارع عباس العقاد",      30.0626, 31.3419),
                ("الجيزة",         "الدقي",           "شارع التحرير",           29.9700, 31.2100),
                ("الإسكندرية",    "سيدي بشر",        "شارع الكورنيش",          31.2001, 29.9187),
                ("الفيوم",         "مدينة الفيوم",    "شارع الحرية",            29.3084, 30.8428),
                ("المنيا",         "مدينة المنيا",    "شارع الجمهورية",         28.1099, 30.7503),
                ("أسيوط",         "مدينة أسيوط",     "شارع طلعت حرب",         27.1809, 31.1837),
                ("الدقهلية",      "المنصورة",        "شارع الجمهورية",         31.0364, 31.3807),
                ("البحيرة",       "دمنهور",          "شارع محمد فريد",         30.8481, 30.3436),
                ("الشرقية",       "الزقازيق",        "شارع ابن خلدون",         30.5877, 31.5021),
                ("الغربية",       "طنطا",            "شارع البحر",             30.7865, 31.0019),
            };

            // Addresses for farmers
            for (int i = 1; i <= 20; i++)
            {
                var govIdx = (i - 1) % govData.Length;
                var (gov, city, street, lat, lng) = govData[govIdx];
                addresses.Add(new Address
                {
                    UserId = users[$"farmer_{i}"].Id,
                    Label = AddressLabel.Farm,
                    Street = street,
                    City = city,
                    Governorate = gov,
                    PostalCode = 1000 + i * 100,
                    Latitude = lat + (Random.Shared.NextDouble() * 0.1),
                    Longitude = lng + (Random.Shared.NextDouble() * 0.1),
                    IsDefault = true
                });
            }

            // Addresses for traders 
            for (int i = 1; i <= 15; i++)
            {
                var govIdx = (i - 1) % govData.Length;
                var (gov, city, street, lat, lng) = govData[govIdx];
                addresses.Add(new Address
                {
                    UserId = users[$"trader_{i}"].Id,
                    Label = AddressLabel.Warehouse,
                    Street = street,
                    City = city,
                    Governorate = gov,
                    PostalCode = 2000 + i * 100,
                    Latitude = lat,
                    Longitude = lng,
                    IsDefault = true
                });          
            }

            // Addresses for consumers
            for (int i = 1; i <= 10; i++)
            {
                var govIdx = (i - 1) % govData.Length;
                var (gov, city, street, lat, lng) = govData[govIdx];
                addresses.Add(new Address
                {
                    UserId = users[$"consumer_{i}"].Id,
                    Label = AddressLabel.Home,
                    Street = street,
                    City = city,
                    Governorate = gov,
                    PostalCode = 3000 + i * 100,
                    Latitude = lat,
                    Longitude = lng,
                    IsDefault = true
                });
            }

            await _context.Addresses.AddRangeAsync(addresses);
            await _context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════
        // STEP 4 — CATEGORIES (already seeded via ModelBuilder)
        // Fetch existing ones
        // ══════════════════════════════════════════════════

        // ══════════════════════════════════════════════════
        // STEP 5 — PRODUCTS + IMAGES
        // ══════════════════════════════════════════════════
        private async Task SeedProductsAsync(Dictionary<string, ApplicationUser> users)
        {
            var categories = await _context.Categories.ToListAsync();

            var productTemplates = new[]
            {
                // (name, description, unit, minPrice, maxPrice, categoryName)
                ("طماطم عضوية طازجة",     "طماطم طازجة مزروعة بدون مبيدات كيميائية",         "كيلو",  3.5m,  7.0m,  "خضروات"),
                ("خيار بلدي",             "خيار طازج من المزرعة مباشرة",                     "كيلو",  2.0m,  5.0m,  "خضروات"),
                ("فلفل ألوان",            "فلفل أحمر وأصفر وأخضر طازج",                     "كيلو",  8.0m,  15.0m, "خضروات"),
                ("بطاطس بيضاء",          "بطاطس طازجة من أجود الأنواع",                     "كيلو",  3.0m,  6.0m,  "خضروات"),
                ("بصل أبيض",             "بصل أبيض طازج للطهي والتصدير",                    "كيلو",  2.5m,  5.5m,  "خضروات"),
                ("ثوم بلدي",             "ثوم بلدي ذو رائحة قوية ومذاق مميز",               "كيلو",  20.0m, 35.0m, "خضروات"),
                ("جزر طازج",             "جزر طازج ومقرمش من حقول الدلتا",                  "كيلو",  2.0m,  4.5m,  "خضروات"),
                ("كوسة خضراء",           "كوسة خضراء طازجة للطهي",                          "كيلو",  3.0m,  6.0m,  "خضروات"),
                ("باذنجان بلدي",         "باذنجان بلدي طازج",                               "كيلو",  2.5m,  5.0m,  "خضروات"),
                ("ملوخية طازجة",         "ملوخية طازجة من أفضل المزارع",                    "كيلو",  5.0m,  10.0m, "خضروات"),
                ("مانجو فاخر",           "مانجو صيفي من أجود الأصناف الصعيدية",             "كيلو",  10.0m, 20.0m, "فاكهة"),
                ("برتقال بلدي",          "برتقال طازج حلو ومليء بالفيتامينات",              "كيلو",  4.0m,  8.0m,  "فاكهة"),
                ("فراولة طازجة",         "فراولة حمراء طازجة من مزارع الدلتا",              "كيلو",  12.0m, 22.0m, "فاكهة"),
                ("عنب أبيض",            "عنب أبيض حلو من مزارع الصعيد",                    "كيلو",  8.0m,  18.0m, "فاكهة"),
                ("تين طازج",            "تين طازج من مزارع البحر المتوسط",                  "كيلو",  15.0m, 28.0m, "فاكهة"),
                ("رمان",               "رمان أحمر حلو طازج",                               "كيلو",  10.0m, 18.0m, "فاكهة"),
                ("خوخ",                "خوخ طازج موسمي",                                   "كيلو",  12.0m, 22.0m, "فاكهة"),
                ("تفاح أحمر",           "تفاح مستورد الزراعة من المشاتل",                  "كيلو",  14.0m, 25.0m, "فاكهة"),
                ("موز",                "موز طازج من أجود الأنواع",                          "كيلو",  5.0m,  10.0m, "فاكهة"),
                ("جوافة",              "جوافة بيضاء حلوة من أفضل المزارع",                 "كيلو",  6.0m,  12.0m, "فاكهة"),
                ("قمح بلدي",           "قمح بلدي صافي للطحن",                              "طن",   4500m, 5500m, "حبوب"),
                ("أرز شعير",           "أرز شعير مصري أبيض فاخر",                         "طن",   6000m, 7500m, "حبوب"),
                ("ذرة صفراء",          "ذرة صفراء للأعلاف والصناعة",                      "طن",   3500m, 4500m, "حبوب"),
                ("فول بلدي",           "فول بلدي مصري من أجود الأنواع",                   "كيلو",  8.0m,  14.0m, "حبوب"),
                ("عدس",               "عدس أخضر طازج من الصعيد",                          "كيلو",  10.0m, 18.0m, "حبوب"),
                ("لبن جاموسي",        "لبن جاموسي طازج يومي",                            "لتر",   10.0m, 16.0m, "ألبان"),
                ("جبن قريش",          "جبن قريش طازج صنع يدوي",                          "كيلو",  20.0m, 35.0m, "ألبان"),
                ("سمن بلدي",          "سمن بلدي أصلي من الجاموس",                        "كيلو",  80.0m, 120.0m,"ألبان"),
                ("زبادي بلدي",        "زبادي بلدي طازج يومي",                            "كيلو",  12.0m, 20.0m, "ألبان"),
                ("لبن بقري طازج",     "لبن بقري طازج يومي من المزرعة",                  "لتر",   7.0m,  12.0m, "ألبان"),
            };

            var products = new List<Product>();
            var farmerCount = 20;

            for (int i = 0; i < productTemplates.Length; i++)
            {
                var (name, desc, unit, minPrice, maxPrice, catName) = productTemplates[i];
                var category = categories.FirstOrDefault(c => c.NameAr == catName) ?? categories.First();
                var farmerKey = $"farmer_{(i % farmerCount) + 1}";
                var price = minPrice + (decimal)(Random.Shared.NextDouble() * (double)(maxPrice - minPrice));

                products.Add(new Product
                {
                    FarmerId = users[farmerKey].Id,
                    CategoryId = category.Id,
                    Name = name,
                    Description = desc,
                    Quantity = Random.Shared.Next(100, 20000),
                    Unit = unit,
                    UnitPrice = Math.Round(price, 2),
                    HarvestDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 14)),
                    ExpiryDate = DateTime.UtcNow.AddDays(Random.Shared.Next(7, 30)),
                    Status = ProductStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60)),
                });
            }

            // Add extra products to reach ~150
            var extraProducts = new[]
            {
                "بروكلي طازج","قرنبيط أبيض","خس بلدي","سبانخ طازجة","كرنب أخضر",
                "بامية طازجة","لوبيا خضراء","فاصوليا بيضاء","بازلاء طازجة","حلبة بذور",
                "كسبرة طازجة","بقدونس","جرجير طازج","نعناع بلدي","ريحان طازج",
                "أفوكادو","بابايا","ليمون بلدي","يوسفي","كيوي",
                "شعير حبوب","دخن","سمسم","كتان بذور","حمص جاف",
                "قشدة بلدية","مشمشية","إيروبيك بلدي","كريمة طازجة","بيض بلدي",
                "دجاج بلدي","أرانب","عسل نحل بلدي","زيت زيتون","مربى تين",
                "بلح سكري","بلح أجوة","تمر هندي","دوم طازج","عرق سوس",
                "ترمس مصري","فول سوداني","عين جمل","مكسرات مشكلة","زبيب أسود",
            };

            for (int i = 0; i < extraProducts.Length; i++)
            {
                var catIdx = i % categories.Count;
                var farmerKey = $"farmer_{(i % farmerCount) + 1}";
                products.Add(new Product
                {
                    FarmerId = users[farmerKey].Id,
                    CategoryId = categories[catIdx].Id,
                    Name = extraProducts[i],
                    Description = $"منتج طازج عالي الجودة: {extraProducts[i]}",
                    Quantity = Random.Shared.Next(50, 1500),
                    Unit = i % 3 == 0 ? "طن" : "كيلو",
                    UnitPrice = Math.Round((decimal)(Random.Shared.NextDouble() * 30 + 3), 2),
                    HarvestDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)),
                    ExpiryDate = DateTime.UtcNow.AddDays(Random.Shared.Next(5, 25)),
                    Status = i % 10 == 0 ? ProductStatus.SoldOut : ProductStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 45)),
                });
            }

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Product Images
            var savedProducts = await _context.Products.ToListAsync();
            var images = new List<ProductImage>();
            var imageBase = "https://placehold.co/600x400?text=";

            foreach (var product in savedProducts)
            {
                int imageCount = Random.Shared.Next(1, 4);
                for (int j = 0; j < imageCount; j++)
                {
                    images.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = $"{imageBase}{Uri.EscapeDataString(product.Name)}_{j + 1}",
                        IsMain = j == 0,
                        DisplayOrder = j + 1
                    });
                }
            }

            await _context.ProductImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════
        // STEP 6 — ORDERS + ITEMS + PAYMENTS + LOGISTICS
        // ══════════════════════════════════════════════════
        private async Task SeedOrdersAsync(Dictionary<string, ApplicationUser> users)
        {
            var products = await _context.Products.Where(p => p.Status == ProductStatus.Active).ToListAsync();

            var traderAddresses = await _context.Addresses.Where(a => a.IsDefault).ToListAsync();

            var orders = new List<Order>();
            var payments = new List<Payment>();
            var logisticsList = new List<Logistics>();

            var orderStatuses = new[]
            {
                OrderStatus.Delivered, OrderStatus.Delivered, OrderStatus.Delivered,
                OrderStatus.InDelivery, OrderStatus.Confirmed,
                OrderStatus.Pending, OrderStatus.Cancelled, OrderStatus.Rejected
            };

            var paymentMethods = new[]
            {
                PaymentMethod.CashOnDelivery, PaymentMethod.Card,
                PaymentMethod.Card, PaymentMethod.Card
            };

            for (int i = 0; i < 150; i++)
            {
                var traderKey = $"trader_{(i % 15) + 1}";
                var trader = users[traderKey];
                var traderAddress = traderAddresses.FirstOrDefault(a => a.UserId == trader.Id);
                if (traderAddress == null) continue;

                var product = products[Random.Shared.Next(products.Count)];
                var quantity = product.Quantity;//Random.Shared.Next(5, 100);
                var priceAtOrder = product.UnitPrice;
                var subtotal = quantity * priceAtOrder;
                var status = orderStatuses[i % orderStatuses.Length];
                var method = paymentMethods[i % paymentMethods.Length];
                var orderDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 90));

                var itemordered = new ProductItemOrdered() { ProductId = product.Id, ProductName = product.Name, Unit = product.Unit};

                var order = new Order
                {
                    BuyerId = trader.Id,
                    BuyerEmail = trader.Email!,
                    FarmerId = product.FarmerId,
                    DliveryAddress = _mapper.Map<OrderAddress>(traderAddress),
                    SubTotal = subtotal,
                    Status = status,
                    PaymentStatus = status == OrderStatus.Delivered ? PaymentStatus.Paid : PaymentStatus.Unpaid,
                    Notes = i % 5 == 0 ? "يرجى التوصيل في الصباح" : null,
                    CreatedAt = orderDate,
                    UpdatedAt = orderDate.AddDays(Random.Shared.Next(1, 5)),
                    DeliveryMethod = _context.DeliveryMethods.Find(4),
                    
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ItemOrdered      = itemordered,
                            Quantity         = quantity,
                            UnitPriceAtOrder = priceAtOrder,
                            Subtotal         = subtotal
                        }
                    }
                };

                orders.Add(order);

                payments.Add(new Payment
                {
                    Order = order,
                    PayerId = trader.Id,
                    Amount = subtotal,
                    Method = method,
                    Status = status == OrderStatus.Delivered? PaymentStatus.Paid : PaymentStatus.Unpaid,
                    PaidAt = status == OrderStatus.Delivered ? orderDate.AddDays(3) : null,
                    CreatedAt = orderDate
                });

                // Find farmer address for pickup traderAddresses contain all users not trader only
                var farmerAddress = traderAddresses.FirstOrDefault(a => a.UserId == product.FarmerId) ?? traderAddress;

                logisticsList.Add(new Logistics
                {
                    Order = order,
                    PickupAddressId = farmerAddress.Id,
                    DeliveryAddressId = traderAddress.Id,
                    DriverName = i % 7 != 0? $"سائق رقم {(i % 10) + 1}": null,
                    DriverPhone = i % 7 != 0? $"010{Random.Shared.Next(10000000, 99999999)}": null,
                    Status = status == OrderStatus.Delivered? LogisticsStatus.Delivered: status == OrderStatus.InDelivery? LogisticsStatus.InTransit: LogisticsStatus.Scheduled,
                    EstimatedDelivery = orderDate.AddDays(2),
                    ActualDelivery = status == OrderStatus.Delivered? orderDate.AddDays(3) : null
                });
            }

            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            await _context.Payments.AddRangeAsync(payments);
            await _context.Logistics.AddRangeAsync(logisticsList);
            await _context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════
        // STEP 7 — AUCTIONS + BIDS
        // ══════════════════════════════════════════════════
        private async Task SeedAuctionsAsync(Dictionary<string, ApplicationUser> users)
        {
            var products = await _context.Products.Where(p => p.Status == ProductStatus.Active).Take(20).ToListAsync();

            var auctions = new List<Auction>();

            for (int i = 0; i < 20; i++)
            {
                var product = products[i % products.Count];
                product.HasActiveAcution = true;
                var startDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30));
                var endDate = startDate.AddDays(Random.Shared.Next(1, 7));
                var isEnded = endDate < DateTime.UtcNow;
                var startPrice = product.UnitPrice * 0.8m * product.Quantity;

                auctions.Add(new Auction
                {
                    ProductId = product.Id,
                    FarmerId = product.FarmerId,
                    StartDate = startDate,
                    EndDate = endDate,
                    StartingPrice = Math.Round(startPrice, 2),
                    ReservePrice = Math.Round(startPrice * 1.1m, 2),
                    CurrentPrice = Math.Round(startPrice * (decimal)(1 + Random.Shared.NextDouble()), 2),
                    Status = isEnded ? AuctionStatus.Ended : AuctionStatus.Active,
                    CreatedAt = startDate.AddHours(-2)
                });
            }

            await _context.Auctions.AddRangeAsync(auctions);
            await _context.SaveChangesAsync();

            // Bids
            var savedAuctions = await _context.Auctions.ToListAsync();
            var bids = new List<Bid>();

            foreach (var auction in savedAuctions)
            {
                int bidCount = Random.Shared.Next(2, 8);
                decimal currentAmount = auction.StartingPrice;

                for (int b = 0; b < bidCount; b++)
                {
                    currentAmount += Math.Round((decimal)(Random.Shared.NextDouble() * 50 + 10), 2);
                    var traderKey = $"trader_{(b % 15) + 1}";

                    bids.Add(new Bid
                    {
                        AuctionId = auction.Id,
                        BidderId = users[traderKey].Id,
                        Amount = currentAmount,
                        IsWinning = b == bidCount - 1,  // last bid is winning
                        BidTime = auction.StartDate.AddHours(b + 1)
                    });
                }

                // Set winner for ended auctions
                if (auction.Status == AuctionStatus.Ended)
                {
                    var winnerKey = $"trader_{(auction.Id % 15) + 1}";
                    auction.WinnerId = users[winnerKey].Id;
                    auction.CurrentPrice = currentAmount;
                }
            }

            await _context.Bids.AddRangeAsync(bids);
            await _context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════
        // STEP 8 — REVIEWS + REPORTS
        // ══════════════════════════════════════════════════
        private async Task SeedReviewsAsync(Dictionary<string, ApplicationUser> users)
        {
            var deliveredOrders = await _context.Orders.Include(o => o.Items)/*.ThenInclude(i => i.Product)*/.Where(o => o.Status == OrderStatus.Delivered).ToListAsync();

            var reviews = new List<Review>();
            //var reports = new List<Report>();

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

            //var reportReasons = new[]
            //{
            //    "المنتج لا يطابق الوصف المعلن",
            //    "تأخر في التسليم بدون إشعار",
            //    "جودة المنتج أقل من المتوقع",
            //    "سلوك غير لائق من البائع",
            //    "معلومات مضللة في الإعلان"
            //};

            for (int i = 0; i < deliveredOrders.Count && i < 100; i++)
            {
                var order = deliveredOrders[i];
                var farmerId = order.FarmerId;
                if (farmerId == null) continue;

                reviews.Add(new Review
                {
                    ReviewerId = order.BuyerId!,
                    TargetUserId = farmerId,
                    OrderId = order.Id,
                    Rating = Random.Shared.Next(3, 6),
                    Comment = comments[i % comments.Length],
                    IsApproved = true,
                    CreatedAt = order.UpdatedAt?.AddDays(1) ?? DateTime.UtcNow
                });
            }
            
            await _context.Reviews.AddRangeAsync(reviews);

            //// Reports
            //var allProducts = await _context.Products.Take(20).ToListAsync();
            //for (int i = 0; i < 30; i++)
            //{
            //    var reporterKey = $"trader_{(i % 15) + 1}";
            //    var product = i % 2 == 0 ? allProducts[i % allProducts.Count] : null;
            //    var reportedKey = i % 2 != 0 ? $"farmer_{(i % 20) + 1}" : null;

            //    reports.Add(new Report
            //    {
            //        ReporterId = users[reporterKey].Id,
            //        ReportedUserId = reportedKey != null ? users[reportedKey].Id : null,
            //        ReportedProductId = product?.Id,
            //        Reason = (ReportReason)(i % 5),
            //        Description = reportReasons[i % reportReasons.Length],
            //        Status = (ReportStatus)(i % 4),
            //        AdminNotes = i % 3 == 0 ? "تمت المراجعة واتخاذ الإجراء المناسب" : null,
            //        CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60)),
            //        ResolvedAt = i % 4 > 1 ? DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)) : null
            //    });
            //}

            //await _context.Reports.AddRangeAsync(reports);
            await _context.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════
        // STEP 9 — COMMUNITY POSTS + LIKES + COMMENTS
        // ══════════════════════════════════════════════════
        #region COMMUNITY
        //private async Task SeedCommunityAsync(Dictionary<string, ApplicationUser> users)
        //{
        //    var postContents = new[]
        //    {
        //        ("نصائح لزراعة الطماطم في فصل الصيف", PostCategory.Tip,
        //         "شاركوني تجربتي في زراعة الطماطم هذا الموسم، استخدمت الري بالتنقيط ووفرت 40% من المياه مع إنتاج أفضل 🌱"),
        //        ("طلب توريد خضروات بكميات كبيرة", PostCategory.Offer,
        //         "نحن شركة توريد نبحث عن مزارعين لتوريد خضروات طازجة بكميات كبيرة لمطاعمنا في القاهرة والجيزة"),
        //        ("سؤال عن أفضل أسمدة للطماطم", PostCategory.Question,
        //         "ما هي أفضل الأسمدة العضوية للطماطم؟ نريد تقليل الكيماويات وزيادة الجودة"),
        //        ("موسم حصاد الفراولة بدأ!", PostCategory.News,
        //         "بشرى سارة لمحبي الفراولة، موسم الحصاد بدأ رسمياً في مزارع الدلتا والأسعار متوقع انخفاضها قريباً 🍓"),
        //        ("كيفية مكافحة حشرة المن على الخيار", PostCategory.Tip,
        //         "تعرضت مزرعتي لهجوم حشرة المن، جربت الرش بالصابون السائل والثوم وكان فعالاً جداً"),
        //        ("أسعار البطاطس هذا الموسم", PostCategory.News,
        //         "تحديث أسعار البطاطس: 4-6 جنيه للكيلو في الدلتا، 5-7 في الصعيد. الإنتاج وفير والأسعار مستقرة"),
        //        ("فرصة شراكة في مزرعة عضوية", PostCategory.General,
        //         "صاحب مزرعة في الفيوم يبحث عن شريك للتوسع في الزراعة العضوية وتصدير المنتجات"),
        //        ("نصائح تخزين الثوم بعد الحصاد", PostCategory.Tip,
        //         "أفضل طريقة لتخزين الثوم: تجفيفه في الهواء 3 أسابيع ثم حفظه في مكان جاف. يدوم حتى 8 أشهر!"),
        //        ("طلب عروض توريد مانجو", PostCategory.Offer,
        //         "مطعمنا يحتاج 500 كيلو أسبوعياً من المانجو الفاخر. من يستطيع التوريد يتواصل معنا"),
        //        ("أخبار الزراعة: دعم الحكومة للمزارعين", PostCategory.News,
        //         "وزارة الزراعة تعلن عن حزمة دعم جديدة للمزارعين الصغار تشمل بذور مجانية وأسمدة مدعومة"),
        //        ("كيف أبدأ في الزراعة العضوية؟", PostCategory.Question,
        //         "أنا مزارع جديد وأريد التحول للزراعة العضوية. ما هي الخطوات الأولى والتكاليف المتوقعة؟"),
        //        ("مزرعة الأمل تعلن عن موسم الذرة", PostCategory.Offer,
        //         "مزرعة الأمل في أسيوط تقدم ذرة صيفية بجودة ممتازة وأسعار تنافسية للكميات الكبيرة"),
        //        ("فوائد الملوخية الصحية", PostCategory.General,
        //         "الملوخية كنز غذائي! غنية بالحديد والكالسيوم وفيتامين ك. إنتاجنا الطازج متاح طوال الأسبوع"),
        //        ("تجربتي مع الري بالتنقيط", PostCategory.Tip,
        //         "بعد 3 سنوات مع الري بالتنقيط: وفرت 50% من المياه وزاد إنتاجي 30%. استثمار يستحق التجربة"),
        //        ("سوق الجملة هذا الأسبوع", PostCategory.News,
        //         "أسعار الجملة هذا الأسبوع: طماطم 2-3 جنيه، خيار 1.5-2.5، بطاطس 3-4. الأسواق مستقرة"),
        //    };

        //    var posts = new List<Post>();
        //    for (int i = 0; i < postContents.Length; i++)
        //    {
        //        var (content, category, fullContent) = postContents[i];
        //        var authorKey = i % 3 == 0
        //            ? $"trader_{(i % 15) + 1}"
        //            : $"farmer_{(i % 20) + 1}";

        //        posts.Add(new Post
        //        {
        //            AuthorId = users[authorKey].Id,
        //            Content = fullContent,
        //            Category = category,
        //            LikesCount = Random.Shared.Next(0, 50),
        //            CommentsCount = Random.Shared.Next(0, 20),
        //            IsDeleted = false,
        //            CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60)),
        //            UpdatedAt = DateTime.UtcNow
        //        });
        //    }

        //    Extra posts to reach ~100
        //    for (int i = 0; i < 85; i++)
        //    {
        //        var authorKey = i % 2 == 0
        //            ? $"farmer_{(i % 20) + 1}"
        //            : $"trader_{(i % 15) + 1}";

        //        posts.Add(new Post
        //        {
        //            AuthorId = users[authorKey].Id,
        //            Content = $"منشور رقم {i + 1}: شارك تجربتك الزراعية مع المجتمع وساهم في تطوير قطاع الزراعة المصري 🌿",
        //            Category = (PostCategory)(i % 5),
        //            LikesCount = Random.Shared.Next(0, 100),
        //            CommentsCount = Random.Shared.Next(0, 30),
        //            IsDeleted = false,
        //            CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 90)),
        //            UpdatedAt = DateTime.UtcNow
        //        });
        //    }

        //    await _context.Posts.AddRangeAsync(posts);
        //    await _context.SaveChangesAsync();

        //    Likes
        //   var savedPosts = await _context.Posts.ToListAsync();
        //    var likes = new List<PostLike>();
        //    var likedPairs = new HashSet<string>();

        //    for (int i = 0; i < 200; i++)
        //    {
        //        var post = savedPosts[i % savedPosts.Count];
        //        var userKey = i % 3 == 0
        //            ? $"consumer_{(i % 10) + 1}"
        //            : i % 2 == 0
        //                ? $"trader_{(i % 15) + 1}"
        //                : $"farmer_{(i % 20) + 1}";
        //        var userId = users[userKey].Id;
        //        var pairKey = $"{post.Id}_{userId}";

        //        if (!likedPairs.Contains(pairKey))
        //        {
        //            likedPairs.Add(pairKey);
        //            likes.Add(new PostLike
        //            {
        //                PostId = post.Id,
        //                UserId = userId,
        //                CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30))
        //            });
        //        }
        //    }

        //    await _context.PostLikes.AddRangeAsync(likes);

        //    Comments
        //   var commentTexts = new[]
        //   {
        //        "معلومة قيمة جداً، شكراً على المشاركة!",
        //        "جربت هذه الطريقة وأعطت نتائج ممتازة",
        //        "هل يمكنك إرسال مزيد من التفاصيل؟",
        //        "أنا مهتم بالتواصل، سأراسلك",
        //        "نفس تجربتي بالضبط، استمر في العطاء",
        //        "معلومة مفيدة، سأطبقها في مزرعتي",
        //        "شكراً على هذا المحتوى الرائع",
        //        "تجربة ثرية تستحق المشاركة",
        //        "هل هذا متاح في منطقة الدلتا أيضاً؟",
        //        "أتمنى لك موسم زراعي موفق",
        //   };

        //    var comments = new List<PostComment>();
        //    for (int i = 0; i < 150; i++)
        //    {
        //        var post = savedPosts[i % savedPosts.Count];
        //        var userKey = i % 2 == 0
        //            ? $"trader_{(i % 15) + 1}"
        //            : $"farmer_{(i % 20) + 1}";

        //        comments.Add(new PostComment
        //        {
        //            PostId = post.Id,
        //            AuthorId = users[userKey].Id,
        //            Content = commentTexts[i % commentTexts.Length],
        //            ParentCommentId = null,
        //            IsDeleted = false,
        //            CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 25))
        //        });
        //    }

        //    await _context.PostComments.AddRangeAsync(comments);
        //    await _context.SaveChangesAsync();
        //}

        #endregion
        // ══════════════════════════════════════════════════
        // STEP 10 — CONVERSATIONS + MESSAGES
        // ══════════════════════════════════════════════════
        private async Task SeedChatAsync(Dictionary<string, ApplicationUser> users)
        {
            var conversations = new List<Conversation>();
            var convPairs = new HashSet<string>();

            for (int i = 0; i < 50; i++)
            {
                var farmerKey = $"farmer_{(i % 20) + 1}";
                var traderKey = $"trader_{(i % 15) + 1}";
                var farmerId = users[farmerKey].Id;
                var traderId = users[traderKey].Id;

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

            var savedConversations = await _context.Conversations.ToListAsync();
            var messages = new List<Message>();

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

            foreach (var conv in savedConversations)
            {
                int msgCount = Random.Shared.Next(6, 20);
                for (int m = 0; m < msgCount; m++)
                {
                    var isUser1 = m % 2 == 0;
                    messages.Add(new Message
                    {
                        ConversationId = conv.Id,
                        SenderId = isUser1 ? conv.User1Id : conv.User2Id,
                        Content = msgTemplates[m % msgTemplates.Length],
                        IsRead = true,
                        SentAt = conv.CreatedAt.AddHours(m + 1)
                    });
                }
            }

            await _context.Messages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();
        }
    }
}
