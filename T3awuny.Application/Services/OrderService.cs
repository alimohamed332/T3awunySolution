using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Order;
using T3awuny.Application.Helpers;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Core.Specifications;
using T3awuny.Core.Specifications.OrderSpecs;
using T3awuny.Core.Specifications.ProductSpecs;

namespace T3awuny.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailService emailService, IPaymentService paymentService)
        {
            _basketRepo = basketRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _emailService = emailService;
            _paymentService = paymentService;
        }

        public async Task<ApiResponse<OrderSummaryDto>> PlaceOrderAsync(string buyerId, CreateOrderDto dto)
        {
            var buyer = await _userManager.FindByIdAsync(buyerId);
            if (buyer is null || !buyer.IsActive || !buyer.IsVerified)
                return ApiResponse<OrderSummaryDto>.Fail("هذا المستخدم غير موجود او حسابه غير نشط");

            //1. Get the basket from the basket repository
            var basket = await _basketRepo.GetBasketAsync(dto.BasketId);
            if (basket is null)
                return ApiResponse<OrderSummaryDto>.Fail("اختر منتجاتك من مزراع واحد في المرة واضفها الي السلة لاستكمال العملية");

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
                return ApiResponse<OrderSummaryDto>.Fail("يجب الحصول علي نية دفع أولاً");

            //2. Get the items from the product repository
            var orderItems = new List<OrderItem>();
            var farmerIds = new List<string>();
            if (basket.Items.Any())
            {
                var productRepo = _unitOfWork.Repository<Product>();         
                foreach (var item in basket.Items)
                {
                    var productSpec = new ProductSpecifications(p => p.Id == item.ProductId);
                    var product = await productRepo.GetByIdWithSpecAsync(productSpec);
                    if (product is null || product.Status != ProductStatus.Active) continue;  //ممكن منكملش بس كده احسن هعمل اوردر بالمنتجات المظبوطة 

                    var productItemOrdered = new ProductItemOrdered(item.ProductId, product.Name, product.Images.FirstOrDefault(p => p.IsMain)?.ImageUrl??"",product.Unit);
                    var orderItem = new OrderItem(productItemOrdered, product.Quantity, product.UnitPrice, product.Quantity * product.UnitPrice);
                    //product.Quantity if sell all offered quantity once بيع جملة بس
                    //item.Quantity if sell as buyer need بيع تجزئة  in this case i must enforce that available in DB > quantity > 0
                    orderItems.Add(orderItem);
                    farmerIds.Add(product.FarmerId);
                }
            }

            var farmerIdsCount = farmerIds.Distinct().Count();
            if (farmerIdsCount > 1 )
                return ApiResponse<OrderSummaryDto>.Fail("يجب ان يحتوي الطلب الواحد منتجات مزارع واحد فقط");

            if (farmerIdsCount == 0)
                return ApiResponse<OrderSummaryDto>.Fail("يجب ان يحتوي الطلب علي منتجات لم تنفذ وصالحة للبيع عدل في عربتك ثم اعد المحاولة");

            var farmerId = farmerIds.First();
            //3. Calculate the subtotal
            //var orderSubtotal = orderItems.Sum(orderItem => orderItem.UnitPriceAtOrder * orderItem.Quantity);
            var orderSubtotal = orderItems.Sum(orderItem => orderItem.Subtotal);

            //4. Get DeliveryMethod from the delivery method repository
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId ?? 4);
            if (deliveryMethod is null)
                return ApiResponse<OrderSummaryDto>.Fail("فشل الحصول علي طريقة التوصيل هذه");

            //5. Get User Main Address
            var addressSpec = new BaseSpecifications<Address>(ad => ad.UserId == buyerId && ad.IsDefault);
            var defaultBuyerAddress = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addressSpec);
            if (defaultBuyerAddress is null)
                return ApiResponse<OrderSummaryDto>.Fail("ضع عنوان رئيسي حتي يتم شحن الطلب عليه");

            var addressSpecForFarmer = new BaseSpecifications<Address>(ad => ad.UserId == farmerIds.FirstOrDefault() && ad.IsDefault);
            var defaultFarmerAddress = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addressSpecForFarmer);
            if (defaultFarmerAddress is null)
                return ApiResponse<OrderSummaryDto>.Fail("هذا المزارع لم يقم بوضع عنوان رئيسي");

            var orderAddress = new OrderAddress() 
            { 
                Street = defaultBuyerAddress.Street, 
                City =defaultBuyerAddress.City, 
                Governorate = defaultBuyerAddress.Governorate, 
                Country = defaultBuyerAddress.Country,
                Name = buyer.Name
            };

            var orderRepo = _unitOfWork.Repository<Order>();
            var orderSpecs = new BaseSpecifications<Order>(o => o.PaymentIntentId == basket.PaymentIntentId);
            var existingOrder = await orderRepo.GetByIdWithSpecAsync(orderSpecs);
            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntentAsync(dto.BasketId);
            }
                

            //6. Create an order
            var order = new Order(buyer.Email!, buyerId, orderSubtotal, dto.Notes, orderAddress, orderItems, deliveryMethod,basket.PaymentIntentId);
            order.FarmerId = farmerId;
            await orderRepo.AddAsync(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return ApiResponse<OrderSummaryDto>.Fail("حدثت مشكلة أثناء حفظ البيانات حاول لاحقاً");

            //7. Create Logistics record automatically
            var logistics = new Logistics
            {
                OrderId = order.Id,
                PickupAddressId = defaultFarmerAddress.Id,
                DeliveryAddressId = defaultBuyerAddress.Id,
                Status = LogisticsStatus.NotScheduled,
                EstimatedDelivery = GetEstimetedDeliveryTime(deliveryMethod)
            };
            await _unitOfWork.Repository<Logistics>().AddAsync(logistics);

            //8. Payment Record
            var payment = new Payment()
            {
                OrderId = order.Id,
                PayerId = buyerId,
                Amount = order.GetTotal(),
                Method = dto.PaymentMethod,
                Status = PaymentStatus.Unpaid,
                PaymentIntentId = basket.PaymentIntentId
            };
            await _unitOfWork.Repository<Payment>().AddAsync(payment);

            //9. Save to the database
            await _unitOfWork.CompleteAsync();

            var farmer = await _userManager.FindByIdAsync(farmerId);

            var orderResponse = _mapper.Map<OrderSummaryDto>(order);
            orderResponse.BuyerName = buyer.Name;
            orderResponse.Items = orderItems.Select(o => _mapper.Map<OrderItemResponseDto>(o)).ToList();
            orderResponse.LogisticsStatus = order.Logistics?.Status.ToString() ?? ""; // دايما هيرجع "" ابقي اتست كده
            if (farmer is not  null)
            await _emailService.SendAsync(
                farmer!.Email!,
                "طلب جديد",
                $"هناك طلب جديد علي منتجاتك ادخل علي حسابك لتأكيده او رفضه وفي حالة التأكيد ابدأ في تحضير الطلب");
            return ApiResponse<OrderSummaryDto>.Ok(orderResponse,"تم إنشاء الطلب بنجاح");
        }

        public async Task<ApiResponse<string>> CancelOrderAsync(string buyerId, int orderId)
        {
            var orderSpecs = new OrderSpecifications(o => o.Id == orderId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecs);

            if (order is null)
                return ApiResponse<string>.Fail("هذا الطلب غير موجود");

            if (order.BuyerId != buyerId)
                return ApiResponse<string>.Fail("هذا الطلب لايخص هذا التاجر");

            if (!OrderStatusValidator.IsValidTransition(order.Status, OrderStatus.Cancelled))
                return ApiResponse<string>.Fail($"لا يمكن الغاء الطلب ");

            order.Status = OrderStatus.Cancelled;
            order.Logistics!.Status = LogisticsStatus.Failed;           
            var productRepo = _unitOfWork.Repository<Product>();
            foreach (var item in order.Items)
            {
                var product = await productRepo.GetByIdAsync(item.ItemOrdered.ProductId);
                if (product is null) continue;
                product.Quantity += item.Quantity;
                product.Status = ProductStatus.Active;
            }
            //If payment was made → trigger refund(PaymentStatus = Refunded)
            // راجع عملية الدفع
            //if(order.PaymentStatus == PaymentStatus.Paid)
            //    //refund logic
            //    order.PaymentStatus = PaymentStatus.Refunded;
            order.Payment!.Status = PaymentStatus.Failed; // لو مكانتش اتدفعت

            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدثت مشكلة أثناء حفظ البيانات حاول لاحقاً");

            return ApiResponse<string>.Ok(string.Empty,"تم إالغاء الطلب بنجاح انتظر ارجاع المبلغ المدفوع في حالة الدفع مسبقاً");
        }

        //ممكن تحولها باجينيشن بسهولة غير بس الباراميترز وخليها تاخد الباجينيشن اوبجكت هنا وفي الكنترولر واستدعي الكونستركتور التاني لل 
        //order specs وباصيله الباجينيشن اوبجكت
        public async Task<ApiResponse<IReadOnlyList<OrderSummaryDto>>> GetOrdersForBuyerAsync(string buyerId)
        {
            var buyer = await _userManager.FindByIdAsync(buyerId);
            if (buyer == null)
                return ApiResponse<IReadOnlyList<OrderSummaryDto>>.Fail("هذا المستخدم غير موجود");

            var orderSpecs = new OrderSpecifications(o => o.BuyerId == buyerId);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpecs);
            if (!orders.Any())
                return ApiResponse<IReadOnlyList<OrderSummaryDto>>.Fail("لا يوجد طلبات لعرضها");

            var orderDtos = new List<OrderSummaryDto>();//orders.Select(o => _mapper.Map<OrderSummeryDto>(o)).ToList();
            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderSummaryDto>(order);
                orderDto.BuyerName = buyer.Name;
                orderDto.Items = order.Items.Select(it => _mapper.Map<OrderItemResponseDto>(it)).ToList();
                orderDto.LogisticsStatus = order.Logistics?.Status.ToString() ?? "";
                orderDtos.Add(orderDto);
                
            }
            return ApiResponse<IReadOnlyList<OrderSummaryDto>>.Ok(orderDtos, "تم الحصول علي الطلبات التي تريدها");
        }

        public async Task<ApiResponse<OrderResponseDto>> GetOrderDetailsAsync(string userId, int orderId)
        {
            var buyer = await _userManager.FindByIdAsync(userId);
            if (buyer == null)
                return ApiResponse<OrderResponseDto>.Fail("هذا المستخدم غير موجود");

            var orderSpec = new OrderSpecifications(o => o.Id == orderId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpec);

            if(order == null)
                return ApiResponse<OrderResponseDto>.Fail("هذا الطلب غير موجود");

            if(order.BuyerId != buyer.Id)
                return ApiResponse<OrderResponseDto>.Fail("هذا الطلب لا يخص هذا التاجر");

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            orderDto.BuyerName = buyer.Name;
            orderDto.Items = order.Items.Select(it => _mapper.Map<OrderItemResponseDto>(it)).ToList();
            orderDto.Logistics.LogisticsStatus = order.Logistics?.Status.ToString() ?? "";
            orderDto.Logistics.Notes = order.Logistics?.Notes ?? "";
            orderDto.Logistics.EstimatedDelivery = order.Logistics?.EstimatedDelivery;
            orderDto.Logistics.LogisticsId = order.Logistics?.Id??0;
            //////////////////////////
            orderDto.Payment.PaymentStatus = order.PaymentStatus.ToString();
            orderDto.Payment.PaymentMethod = order.Payment!.Method.ToString() ?? "";
            orderDto.Payment.PaymentIntentId = order.PaymentIntentId;
            orderDto.Payment.Id = order.Payment?.Id??0;

            return ApiResponse<OrderResponseDto>.Ok(orderDto,"تم الحصول علي الطلب بنجاح");
        }

        public async Task<ApiResponse<string>> ConfirmOrderAsync(string farmerId, int orderId)
        {
            var orderSpecs = new OrderSpecifications(o => o.Id == orderId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecs); 
            if(order == null)
                return ApiResponse<string>.Fail("هذا الطلب غير موجود");

           

            if (order!.FarmerId != farmerId)
                return ApiResponse<string>.Fail("لا يمكنك تأكيد طلب علي منتجات ليست ملكك");

            if(OrderStatusValidator.IsValidTransition(order!.Status,OrderStatus.Confirmed))
                  order!.Status = OrderStatus.Confirmed;
            else
                return ApiResponse<string>.Fail("هذا الطلب لا يمكنك تأكيده");

            var buyerEmail = order.BuyerEmail;
            //logistics and payment creation or edit if it created in place order which one i implmented
            order.Logistics!.Status = LogisticsStatus.Scheduled;
            //Update product quantity and status
             var productRepo = _unitOfWork.Repository<Product>();
            foreach (var item in order.Items)
            {
                var productSpec = new BaseSpecifications<Product>(p => p.Id == item.ItemOrdered.ProductId);
                var productFromDb = await productRepo.GetByIdWithSpecAsync(productSpec);
                if (productFromDb is null || productFromDb.Status != ProductStatus.Active) continue;
                productFromDb.Quantity = 0; //if sell all offered quantity once بيع جملة بس
                //item.Quantity if sell as buyer need بيع تجزئة  in this case i must enforce that available in DB > quantity > 0
                productFromDb.Status = ProductStatus.SoldOut;
            }

            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدثت مشكلة أثناء حفظ البيانات حاول لاحقاً");

            //email for admin and buyer
            await _emailService.SendAsync(
                buyerEmail,
                "تأكيد طلبك",
                $"تم تأكيد طلبك من المزارع تابع تحديثات الطلب من الموقع باستمرار");

            return ApiResponse<string>.Ok(string.Empty,"تم تأكيد الطلب بنجاح ابدأ في تجيزه وحدث حالة الطلب علي الموقع باستمرار حتي يتم تسليمه");

        }

        public async Task<ApiResponse<string>> RejectOrderAsync(string farmerId, int orderId, string reason)
        {
            var orderSpecs = new OrderSpecifications(o => o.Id == orderId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecs);
            if (order == null)
                return ApiResponse<string>.Fail("هذا الطلب غير موجود");

            var productId = order?.Items?.FirstOrDefault()?.ItemOrdered.ProductId ?? 0;
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<string>.Fail("هناك مشكلة في احدي المنتجات المطلوبة");

            if (product.FarmerId != farmerId)
                return ApiResponse<string>.Fail("لا يمكنك رفض طلب علي منتجات ليست ملكك");

            if (OrderStatusValidator.IsValidTransition(order!.Status, OrderStatus.Rejected))
                order!.Status = OrderStatus.Rejected;
            else
                return ApiResponse<string>.Fail("هذا الطلب لا يمكنك رفضه");

            var buyerEmail = order.BuyerEmail;
            //logistics and payment creation or edit if it created in place order which one i implmented
            order.Logistics!.Status = LogisticsStatus.Failed;
            order.Payment!.Status = PaymentStatus.Failed;
            order.Notes = $"سبب رفض الطلب من المزارع هو  {reason}";
            
            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدثت مشكلة أثناء حفظ البيانات حاول لاحقاً");

            //email for admin and buyer
            await _emailService.SendAsync(
                buyerEmail,
                "رفض طلبك",
                $"تم رفض طلبك من المزارع بسبب {reason}");

            return ApiResponse<string>.Ok(string.Empty, "تم إلغاء الطلب بنجاح ");
        }

        public async Task<ApiResponse<string>> UpdateOrderStatusAsync(string /*farmerId*/userId, int orderId, OrderStatus newStatus)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");
            var roles = await _userManager.GetRolesAsync(user);
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order is null)
                return ApiResponse<string>.Fail("هذا الطلب غير موجود");
            //check if this order for this farmer??

            if(!roles.Contains("Admin") && newStatus == OrderStatus.Delivered)
                return ApiResponse<string>.Fail("الادمن فقط هو من يستطيع الأشارة الي ان الطلب تم توصيله");

            if (OrderStatusValidator.IsValidTransition(order!.Status, newStatus))
                order!.Status = newStatus;
            else
                return ApiResponse<string>.Fail("لا يمكن تحديث حالة الطلب لهذه الحالة مباشرةً");
            
            if (order.Status == OrderStatus.Delivered)
            {
                var logisticsSpec = new BaseSpecifications<Logistics>(lo => lo.OrderId == orderId);
                var logistics = await _unitOfWork.Repository<Logistics>().GetByIdWithSpecAsync(logisticsSpec);
                if (logistics is not null)
                {
                    logistics.Status = LogisticsStatus.Delivered;
                    logistics.ActualDelivery = DateTime.UtcNow;
                }

                var paymentSpec = new BaseSpecifications<Payment>(p => p.OrderId == orderId);
                var payment = await _unitOfWork.Repository<Payment>().GetByIdWithSpecAsync(paymentSpec);
                if (payment is not null && payment.Method == PaymentMethod.CashOnDelivery)
                {
                    payment.Status = PaymentStatus.Paid;
                    payment.PaidAt = DateTime.UtcNow;
                }
                    

            }

            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدث خطأ أثناء حفظ البيانات حاول مرة اخرى لاحقاً");

            if(order.Status == OrderStatus.ReadyForPickup && !roles.Contains("Admin"))
            {
                await _emailService.SendAsync(
                   "3li3320043li@gmail.com",
                   "الطلب جاهز",
                   $"قام المزارع صاحب المعرف {userId} بتغير حالة الطلب واصبح جاهز للشحن يمكنك التواصل معه لشحن الطلب");
            }        
               
            return ApiResponse<string>.Ok(string.Empty, "تم تحديث حالة الطلب بنجاح");
        }

        public async Task<ApiResponse<DeliveryMethod>> GetDeliveryMethod(int deliveryMethodId)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            if (deliveryMethod is null)
                return ApiResponse<DeliveryMethod>.Fail("طريقة التوصيل هذه غير موجودة");
            return ApiResponse<DeliveryMethod>.Ok(deliveryMethod,"تم الحصول علي طريقة التوصيل");
        }

        public async Task<ApiResponse<IReadOnlyList<OrderSummaryDto>>> GetMyOrdersAsFarmerAsync(string farmerId)
        {
            //farmer
            var orderSpecs = new OrderSpecifications(o => o.FarmerId == farmerId);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpecs);
            if (!orders.Any())
                return ApiResponse<IReadOnlyList<OrderSummaryDto>>.Fail("لا يوجد طلبات لهذا المزارع لعرضها");

            var orderDtos = orders.Select(o => _mapper.Map<OrderSummaryDto>(o)).ToList();

            return ApiResponse<IReadOnlyList<OrderSummaryDto>>.Ok(orderDtos, "تم الحصول علي الطلبات التي تمت علي منتحاتك");
        }

        public async Task<ApiResponse<Pagination<OrderSummaryDto>>> GetAllOrdersAsync(OrderSpecParams specs)
        {
            var orderRepo = _unitOfWork.Repository<Order>(); // better than repeat this line two times
            var orderSpecs = new OrderSpecifications(specs);
            var orders = await orderRepo.GetAllWithSpecAsync(orderSpecs);

            if (!orders.Any())
                return ApiResponse<Pagination<OrderSummaryDto>>.Fail("لا يوجد طلبات بهذه الخصائص لعرضها");
            var countSpecs = new BaseSpecifications<Order>(orderSpecs.Criteria!);
            var count = await orderRepo.GetCountAsync(countSpecs);

            var orderDtos = orders.Select(o => _mapper.Map<OrderSummaryDto>(o)).ToList();

            var pagination = new Pagination<OrderSummaryDto>(specs.PageIndex,specs.pageSize,count,orderDtos);

            return ApiResponse<Pagination<OrderSummaryDto>>.Ok(pagination, "تم الحصول علي الطلبات بنجاح");
        }
        private DateTime? GetEstimetedDeliveryTime(DeliveryMethod deliveryMethod)
        {
            if(deliveryMethod.ShortName == "UPS1")
                return DateTime.Now.AddDays(2);
            else if (deliveryMethod.ShortName == "UPS2")
                return DateTime.Now.AddDays(4);
            else if (deliveryMethod.ShortName == "UPS3")
                return DateTime.Now.AddDays(7);
            else
                return DateTime.Now.AddDays(10);
        }

        
    }
}
