﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSeconds;
       

        public CachedAttribute(int ExpireTimeInSeconds)
        {
            _expireTimeInSeconds = ExpireTimeInSeconds;
            
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();


            var CacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var CachedResponse =   await CacheService.GetCachedResponse(CacheKey);
            if(!string.IsNullOrEmpty(CachedResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = CachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var ExecutedEndPointContext =   await next.Invoke(); // will Execute EndPoint
            if( ExecutedEndPointContext.Result is OkObjectResult result )
            {
                await CacheService.CacheResponseAsync(CacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSeconds));

            }


        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            
            keyBuilder.Append(request.Path); // api/products ... etc
            foreach(var (key, value) in request.Query.OrderBy(X=>X.Key))
            {
                // Sort = Name
                // Page Index = 1 
                // Page Size = 5

                keyBuilder.Append($"|{key}-{value}");

                // api/products|Sort-Name|PageIndex-1|PageSize-5

            }
            return keyBuilder.ToString();


        }
    }
}
