using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAll();

        Task<T?> GetById(Guid id);
        Task<T> GetByIdAsync(string id);

        Task Add(T entity);
        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<List<T>> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
        void RemoveRange(List<T> obj);
        Task<int> CountAsync();
    }
}
