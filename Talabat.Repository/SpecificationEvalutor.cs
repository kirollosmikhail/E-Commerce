using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Talabat.Repository
{
    public static class SpecificationEvalutor<T> where T : BaseEntity
    {
        // Fun To Build Query


        // _dbContext.Set<T>().Where(P => P.Id == id).Include(P => P.ProductBrand).Include(P => P.ProductType);

        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery , ISpecifications<T> Spec)
        {
            var Query = inputQuery; // // _dbContext.Set<T>()
            
            if(Spec.Criteria is not null) // P => P.Id == id
            {
                Query = Query.Where(Spec.Criteria); //_dbContext.Set<T>().Where(P => P.Id == id)
            }

            // OrderBy , OrderByDesc
            if(Spec.OrderBy is not null)
            {
                // _dbcontext.product.Orderby(P=>P.price)
                Query = Query.OrderBy(Spec.OrderBy);
            }
            if(Spec.OrderByDescending is not null)
            {
                Query = Query.OrderByDescending(Spec.OrderByDescending);
            }

            // Pagination
            if(Spec.IsPaginationEnabled)
            {
                Query = Query.Skip(Spec.Skip).Take(Spec.Take);
            }




            // P => P.ProductBrand ,P => P.ProductType
            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
            //_dbContext.Set<T>().Where(P => P.Id == id).Include(P => P.ProductBrand)
            //_dbContext.Set<T>().Where(P => P.Id == id).Include(P => P.ProductBrand).Include(P => P.ProductType);
            return Query;

        }


    }
}
