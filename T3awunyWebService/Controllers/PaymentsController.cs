using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.V2.Core;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Core.Entities.BasketModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private const string endpointSecret = "whsec_cuNajXZAJT1X8da4I7GPlJyNBsdGJXEj"; // Hosted one 
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize("TraderOnly")]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<ApiResponse<CustomerBasket>>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var result = await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);

            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], endpointSecret);

                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                switch (stripeEvent.Type)
                {
                    case "payment_intent_succeeded":
                        await _paymentService.UpdatePaymentStatusToFailOrSuccess(paymentIntent.Id, true);
                        break;
                    case "payment_intent.payment_failed":
                        await _paymentService.UpdatePaymentStatusToFailOrSuccess(paymentIntent.Id, false);
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [Authorize("AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Payment>>> GetPayments()
        {
            var result = await _paymentService.GetPaymentsAsync();
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<IReadOnlyList<Payment>>> GetPayments(int orderId)
        {
            var result = await _paymentService.GetPaymentByOrderAsync(orderId);

            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpPatch("{orderId}/change-status")]
        public async Task<ActionResult<ApiResponse<string>>> ChangePaymentStatus(int orderId, PaymentStatus status)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(orderId, status);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
