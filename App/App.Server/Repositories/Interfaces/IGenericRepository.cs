using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;

namespace App.Server.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            string includeProperties = "");
        Task<T?> GetById(params object[] id);
        void Add(T entity);
        void Update(T entityToUpdate);
        void Delete(T entityToDelete);
    }
}
