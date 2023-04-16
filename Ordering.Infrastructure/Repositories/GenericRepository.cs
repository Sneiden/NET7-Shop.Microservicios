using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
    {
        private readonly OrderContext orderContext;

        public GenericRepository(OrderContext orderContext)
        {
            this.orderContext = orderContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await orderContext.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T,bool>> predicate)
            => await orderContext.Set<T>().Where(predicate).ToListAsync();

        public async Task<IReadOnlyList<T>> GetAsync(int offset, int limit, 
            Expression<Func<T,bool>>? predicate, 
            Func<IQueryable<T>,IOrderedQueryable<T>>? orderBy, 
            params string[] includeStrings)
        {
            IQueryable<T> querty = orderContext.Set<T>();

            querty = querty.Skip(offset).Take(limit);

            querty = includeStrings.Aggregate(querty, (current, itemInclude) => current.Include(itemInclude));

            if (predicate is not null)
                querty =querty.Where(predicate);

            if (orderBy is not null)
                return await orderBy(querty).ToListAsync();

            return await querty.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int Id)
            => await orderContext.Set<T>().SingleAsync(x=>x.Id==Id);

        public async Task<T> AddAsync(T entity)
        {
            await orderContext.Set<T>().AddAsync(entity);
            await orderContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            orderContext.Entry(entity).State = EntityState.Modified;
            await orderContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            orderContext.Set<T>().Remove(entity);
            await orderContext.SaveChangesAsync();
        }

        //en esta parte se podria agregar borrado logico generico
        //el borrado logico sirve para contemplar o no, los registros con algun campo que defina si esta activo o no
    }
}
