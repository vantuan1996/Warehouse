using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.DbContext;
namespace Warehouse.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetById(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
             await _dbSet.AddAsync(entity);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }
        public void RemoveRange(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }
    }
}
