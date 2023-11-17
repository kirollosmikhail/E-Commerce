using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    
    public class PaymentsController : APIBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        const string endpointSecret = "whsec_dd87e2241297945634b2015c876e4409f90e7c5617e766e088bb037101864fd5";

        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            this._paymentService = paymentService;
            this._mapper = mapper;
        }
        // Create Or Update EndPoint
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var CustomerBasket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (CustomerBasket is null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Basket"));

            var MappedBasket = _mapper.Map<CustomerBasket,CustomerBasketDto>(CustomerBasket);
            return Ok(MappedBasket);


        }
        [HttpPost("webhook")] // post => baseurl/api/payments/webhook
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, true);

                }
                

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

    }
}
