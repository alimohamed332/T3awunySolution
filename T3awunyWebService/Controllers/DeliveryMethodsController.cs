using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Identity.Client;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.DeliveryMethods;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryMethodsController : ControllerBase
    {
        private readonly IDeliveryMethodService _deliveryMethodService;

        public DeliveryMethodsController(IDeliveryMethodService deliveryMethodService)
        {
            _deliveryMethodService = deliveryMethodService;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<DeliveryMethodResponseDto>>>> GetAll()
        {
            var result = await _deliveryMethodService.GetAll();
            if (!result.IsSuccess)   // always successed
                return NotFound(result);
            return Ok(result);
        }
        [HttpGet("{deliveryMethodId}")]
        public async Task<ActionResult<ApiResponse<DeliveryMethodResponseDto>>> GetDeliveryMethod(int deliveryMethodId)
        {
            var result = await _deliveryMethodService.GetDeliveryMethod(deliveryMethodId);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<DeliveryMethodResponseDto>>> CreateDeliveryMethod([FromBody] CreateDeliveryMethodDto dto)
        {
            var result = await _deliveryMethodService.CreateDeliveryMethod(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpPut("{deliveryMethodId}")]
        public async Task<ActionResult<ApiResponse<DeliveryMethodResponseDto>>> UpdateDeliveryMethod(int deliveryMethodId,[FromBody] UpdateDeliveryMethodDto dto)
        {
            var result = await _deliveryMethodService.UpdateDeliveryMethod(deliveryMethodId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpDelete("{deliveryMethodId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDeliveryMethod(int deliveryMethodId)
        {
            var result = await _deliveryMethodService.DeleteDeliveryMethod(deliveryMethodId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
