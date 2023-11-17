using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region Without Specifications
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
                return (IReadOnlyList<T>)await _dbContext.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();

            else
                return await _dbContext.Set<T>().ToListAsync();
        }



        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
            //return await _dbContext.Set<T>().Where(P => P.Id == id).Include(P => P.ProductBrand).Include(P => P.ProductType);
        } 
        #endregion


        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
        {
            
            return await ApplySpecification(Spec).ToListAsync();
        }
         
        public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecification(Spec).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecification(Spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecifications<T> Spec)
        {
            return SpecificationEvalutor<T>.GetQuery(_dbContext.Set<T>(), Spec);
        }

        public async Task Add(T item)
            => await _dbContext.Set<T>().AddAsync(item);

        public void Delete(T item)
            => _dbContext.Set<T>().Remove(item);

        public void Update(T item)
            => _dbContext.Set<T>().Update(item);

    }
}
