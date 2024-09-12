using Domain.Base;
using Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetAsync(string Id, CancellationToken ct);
        Task<T> GetAsync(Expression<Func<T, bool>> expression, CancellationToken ct);
        IQueryable<T> ListAll(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity, CancellationToken ct);
        Task UpdateAsync(T entity, CancellationToken ct);
        Task DeleteAsync(string Id, CancellationToken ct);
        Task DeleteAllAsync(Expression<Func<T, bool>> expression, CancellationToken ct);
        Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken ct);
    }
}
