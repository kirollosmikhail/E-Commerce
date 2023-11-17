using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
  
    public class ProductsController : APIBaseController
    {
        
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IMapper mapper,IUnitOfWork unitOfWork)
            
        {
            
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        // Get All Products
        // BaseUrl/api/Products -> Get
        [CachedAttribute(300)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
        {
            var Spec = new ProductWithBrandAndTypeSpecifications(Params);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
            var MappedProducts = _mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            ///var ReturnedObject = new Pagination<ProductToReturnDto>()
            ///{
            ///    PageIndex = Params.PageIndex,
            ///    PageSize = Params.PageSize,
            ///    Data = MappedProducts
            ///};
            ///return Ok(ReturnedObject);


            var CountSpec = new ProductWithFiltrationForCountSpec(Params);
            var Count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);
            return Ok(new Pagination<ProductToReturnDto>(Params.PageIndex, Params.PageSize, MappedProducts, Count));

        }


        // Get Product By Id
        // BaseUrl/api/Products/1 -> GET
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {
            var Spec = new ProductWithBrandAndTypeSpecifications(id);
            var Product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(Spec);
            if (Product is null) return NotFound(new ApiResponse(404));
            var MappedProducts = _mapper.Map<Product,ProductToReturnDto>(Product);
            return Ok(MappedProducts);
        }

        // baseurl/api/products/types
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var Types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }
        // baseurl/api/products/brands
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrand()
        {
            var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(Brands);
            
        }

    }
}
