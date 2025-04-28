using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Server.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected PlannerNPContext _context;
        protected DbSet<T> _dbSet;

        public GenericRepository(PlannerNPContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Delete(T entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>>? filter = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                ([','], StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T?> GetById(params object[] id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}
