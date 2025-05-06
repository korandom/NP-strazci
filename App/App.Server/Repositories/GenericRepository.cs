using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Server.Repositories
{
    /// <summary>
    /// Generic repository providing basic CRUD operations for app data entities.
    /// </summary>
    /// <typeparam name="T">Entity type (must be a reference type).</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected PlannerNPContext _context;
        protected DbSet<T> _dbSet;

        public GenericRepository(PlannerNPContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Deletes an entity from the database context.
        /// If the entity is detached, it is first attached to the context.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public void Delete(T entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Retrieves a list of entities from the database, optionally filtered and including related entities.
        /// </summary>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="includeProperties"> Comma-separated list of navigation properties to include (e.g. "Category,Tags"). </param>
        /// <returns>A task with a result of the list of entities.</returns>

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

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key values of the entity.</param>
        /// <returns>
        /// A task with the result being the entity if found, otherwise null.
        /// </returns>
        public virtual async Task<T?> GetById(params object[] id) 
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Adds a new entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Updates an existing entity in the database context.
        /// </summary>
        /// <param name="entityToUpdate">The entity with updated values.</param>
        public void Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}
