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

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _basketRepo = basketRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse<OrderSummeryDto>> PlaceOrderAsync(string buyerId, CreateOrderDto dto)
        {
            var buyer = await _userManager.FindByIdAsync(buyerId);
            if (buyer is null || !buyer.IsActive || !buyer.IsVerified)
                return ApiResponse<OrderSummeryDto>.Fail("هذا المستخدم غير موجود او حسابه غير نشط");

            //1. Get the basket from the basket repository
            var basket = await _basketRepo.GetBasketAsync(dto.BasketId);
            if (basket is null)
                return ApiResponse<OrderSummeryDto>.Fail("اختر منتجاتك من مزراع واحد في المرة واضفها الي السلة لاستكمال العملية");

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
            if (farmerIds.Distinct().Count() > 1)
                return ApiResponse<OrderSummeryDto>.Fail("يجب ان يحتوي الطلب الواحد منتجات مزارع واحد فقط");

            //3. Calculate the subtotal
            //var orderSubtotal = orderItems.Sum(orderItem => orderItem.UnitPriceAtOrder * orderItem.Quantity);
            var orderSubtotal = orderItems.Sum(orderItem => orderItem.Subtotal);

            //4. Get DeliveryMethod from the delivery method repository
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(dto.DeliveryMethodId);
            if (deliveryMethod is null)
                return ApiResponse<OrderSummeryDto>.Fail("فشل الحصول علي طريقة التوصيل هذه");

            //5. Get User Main Address
            var addressSpec = new BaseSpecifications<Address>(ad => ad.UserId == buyerId && ad.IsDefault);
            var defaultBuyerAddress = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addressSpec);
            if (defaultBuyerAddress is null)
                return ApiResponse<OrderSummeryDto>.Fail("ضع عنوان رئيسي حتي يتم شحن الطلب عليه");

            var addressSpecForFarmer = new BaseSpecifications<Address>(ad => ad.UserId == farmerIds.FirstOrDefault() && ad.IsDefault);
            var defaultFarmerAddress = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addressSpecForFarmer);
            if (defaultFarmerAddress is null)
                return ApiResponse<OrderSummeryDto>.Fail("هذا المزارع لم يقم بوضع عنوان رئيسي");

            var orderAddress = new OrderAddress() 
            { 
                Street = defaultBuyerAddress.Street, 
                City =defaultBuyerAddress.City, 
                Government = defaultBuyerAddress.Governorate, 
                Country = defaultBuyerAddress.Country,
                Name = buyer.Name
            };
            //6. Create an order
            var order = new Order(buyer.Email!, buyerId, orderSubtotal, dto.Notes, orderAddress, orderItems, deliveryMethod );
            await _unitOfWork.Repository<Order>().AddAsync(order);
            //7. Create Logistics record automatically
            //var logistics = new Logistics
            //{
            //    OrderId = order.Id,
            //    PickupAddressId = defaultFarmerAddress.Id,
            //    DeliveryAddressId = defaultBuyerAddress.Id,
            //    Status = LogisticsStatus.NotScheduled,
            //    EstimatedDelivery = GetEstimetedDeliveryTime(deliveryMethod)
            //};
            //await _unitOfWork.Repository<Logistics>().AddAsync(logistics);
            //8. Payment Record
            //9. Save to the database
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) 
                return ApiResponse<OrderSummeryDto>.Fail("حدثت مشكلة أثناء حفظ البيانات حاول لاحقاً");

            var orderResponse = _mapper.Map<OrderSummeryDto>(order);
            orderResponse.BuyerName = buyer.Name;
            orderResponse.Items = orderItems.Select(o => _mapper.Map<OrderItemResponseDto>(o)).ToList();
            orderResponse.LogisticsStatus = order.Logistics?.Status.ToString() ?? ""; // دايما هيرجع "" ابقي اتست كده
            return ApiResponse<OrderSummeryDto>.Ok(orderResponse,"تم إنشاء الطلب بنجاح");
        }

        //ممكن تحولها باجينيشن بسهولة غير بس الباراميترز وخليها تاخد الباجينيشن اوبجكت هنا وفي الكنترولر واستدعي الكونستركتور التاني لل 
        //order specs وباصيله الباجينيشن اوبجكت
        public async Task<ApiResponse<IReadOnlyList<OrderSummeryDto>>> GetOrdersForBuyerAsync(string buyerId)
        {
            var buyer = await _userManager.FindByIdAsync(buyerId);
            if (buyer == null)
                return ApiResponse<IReadOnlyList<OrderSummeryDto>>.Fail("هذا المستخدم غير موجود");

            var orderSpecs = new OrderSpecifications(o => o.BuyerId == buyerId);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpecs);
            if (!orders.Any())
                return ApiResponse<IReadOnlyList<OrderSummeryDto>>.Fail("لا يوجد طلبات لعرضها");

            var orderDtos = new List<OrderSummeryDto>();//orders.Select(o => _mapper.Map<OrderSummeryDto>(o)).ToList();
            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderSummeryDto>(order);
                orderDto.BuyerName = buyer.Name;
                orderDto.Items = order.Items.Select(it => _mapper.Map<OrderItemResponseDto>(it)).ToList();
                orderDto.LogisticsStatus = order.Logistics?.Status.ToString() ?? "";
                orderDtos.Add(orderDto);
                
            }
            return ApiResponse<IReadOnlyList<OrderSummeryDto>>.Ok(orderDtos, "تم الحصول علي الطلبات التي تريدها");
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

            return ApiResponse<OrderResponseDto>.Ok(orderDto,"تم الحصول علي الطلب بنجاح");
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
