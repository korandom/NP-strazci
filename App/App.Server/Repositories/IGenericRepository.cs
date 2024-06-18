using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;

namespace App.Server.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            string includeProperties = "");
        T GetById(object id);
        void Insert(T entity);
        void Update(T entityToUpdate);
        void Delete(object id);
        void Delete(T entityToDelete);
    }
}
