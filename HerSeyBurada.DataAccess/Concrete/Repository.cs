using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HerSeyBurada.Core.Concrete;
using HerSeyBurada.DataAccess.Abstract;

namespace HerSeyBurada.DataAccess.Concrete
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly HerSeyBuradaDbContext _context;

        public Repository(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            entity.GuncellenmeTarihi = DateTime.UtcNow;
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            entity.IsDeleted = true;
            Update(entity);
        }
    }
}