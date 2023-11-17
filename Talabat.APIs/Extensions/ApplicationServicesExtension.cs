using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection Services)
        {

            //builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            //builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
            //Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            Services.AddScoped<IPaymentService, PaymentService>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            
            Services.AddAutoMapper(typeof(MappingProfiles));
            Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    //ModelState => Dicionary [KeyValuePair]
                    // Key => Name of Param
                    // value = Errors

                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                               .SelectMany(P => P.Value.Errors)
                                               .Select(E => E.ErrorMessage)
                                               .ToArray();

                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors

                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });
            return Services;
        }


    }
}
