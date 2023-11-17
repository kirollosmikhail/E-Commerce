using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        //_dbContext.Products.where(P=>P.id==id).Include(P=>P.ProductBrand).Include(P=>P.ProductType)

        // Sign for property for where condition [where(P=>P.id==id)]
        public Expression<Func<T,bool>> Criteria { get; set; }

        // Sign For Property For List Of Include [Include(P=>P.ProductBrand).Include(P=>P.ProductType)]

        public List<Expression<Func<T,object>>> Includes { get; set; }


        // OrderBy
        public Expression<Func<T,object>> OrderBy { get; set; }

        // OrderByDesc
        public Expression<Func<T,object>> OrderByDescending { get; set; }

        // Skip
        public int Skip { get; set; }
        // Take
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }

    }
}
