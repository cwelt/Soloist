using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.Repositories
{
    public interface IRepository<TEntity, TPKeyType> where TEntity : class 
    {
        // methods for searching and querying the repository 
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);


        // methods for adding elements to the repository 
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        // methods for removing elements from the repository 
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
