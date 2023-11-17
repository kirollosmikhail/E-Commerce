using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    
    public class BuggyController : APIBaseController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(100);
            if (product == null) return NotFound(new ApiResponse(404));
            return Ok(product);

        }
        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);
            var ProductToReturn = product.ToString; // error
            // will throw exception [null reference exception]
            return Ok(ProductToReturn);

        }
        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }
        [HttpGet("BadRequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }

    }
}
