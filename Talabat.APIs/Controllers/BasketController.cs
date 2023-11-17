using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : APIBaseController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            this._basketRepository = basketRepository;
            this._mapper = mapper;
        }

        // Get Or Recreate Basket
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string id)
        {
            var Basket = await _basketRepository.GetBasketAsync(id);
            //if (Basket is null)
            //    return new CustomerBasket(id);
            //else
            //    return Basket;
            return Basket is null ? new CustomerBasket(id) : Basket;


        }


        // Create Or Update
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpdatedBasket is null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Basket"));
            return Ok(CreatedOrUpdatedBasket);
        }


        // Delete Basket
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }


    }
}
