using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CW.Soloist.DataAccess.Repositories
{
    public class Repository<TEntity, TPKeyType> : IRepository<TEntity, TPKeyType> where TEntity : class
    {
        protected DbContext Context { get; }
        private readonly DbSet<TEntity> _entities;

        public Repository(DbContext context)
        {
            Context = context;
            _entities = context.Set<TEntity>();
        }

        public TEntity Get(int id) => _entities.Find(id);
           
        public IEnumerable<TEntity> GetAll() => _entities.ToList();

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) 
        {
            return _entities.Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.SingleOrDefault(predicate);
        }

        public void Add(TEntity entity) => _entities.Add(entity);
            
        public void AddRange(IEnumerable<TEntity> entities) => _entities.AddRange(entities);

        public void Remove(TEntity entity) => _entities.Remove(entity);
            
        public void RemoveRange(IEnumerable<TEntity> entities) => _entities.RemoveRange(entities);
    }
}
