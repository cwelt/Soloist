using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CW.Soloist.DataAccess.Repositories
{
    /// <summary>
    /// This class provides a generic implementation of the <see cref="IRepository{TEntity, TPKeyType}"/>
    /// interface for entities of type <typeparamref name="TEntity"/> which are uniquely 
    /// identified with a key of type <typeparamref name="TPKeyType"/>.
    /// <para> This class implementation is based on the entity framework, and actually 
    /// serves as a bridge which delegates the implementation to the EF. </para>
    /// </summary>
    /// <typeparam name="TEntity"> The type of entities managed in this repository.</typeparam>
    /// <typeparam name="TPKeyType"> The type of the primary key that uniquely identifies entities in this repostiory.</typeparam>
    public class Repository<TEntity, TPKeyType> : IRepository<TEntity, TPKeyType> where TEntity : class
    {
        #region Properties
        /// <summary> The delegated repository context in the EF repository manager context. </summary>
        protected DbContext Context { get; }

        /// <summary> The delegated repository in the entity framework context. </summary>
        private readonly DbSet<TEntity> _entities;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a repository of the given entity from the given context.
        /// </summary>
        /// <param name="context"></param>
        public Repository(DbContext context)
        {
            Context = context;
            _entities = context.Set<TEntity>();
        }
        #endregion

        #region Methods For Searching & Querying 

        public TEntity Get(TPKeyType id) => _entities.Find(id);

        public Task<TEntity> GetAsync(TPKeyType id) => _entities.FindAsync(id);
            
        public IEnumerable<TEntity> GetAll() => _entities.ToList();

        public Task<List<TEntity>> GetAllAsync()
        {
            return _entities.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) 
        {
            return _entities.Where(predicate);
        }
        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.Where(predicate).ToListAsync();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return _entities.SingleOrDefault(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return _entities.SingleOrDefaultAsync(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Methods For Adding & Removing Elements

        public void Add(TEntity entity) => _entities.Add(entity);
            
        public void AddRange(IEnumerable<TEntity> entities) => _entities.AddRange(entities);

        public void Remove(TEntity entity) => _entities.Remove(entity);
            
        public void RemoveRange(IEnumerable<TEntity> entities) => _entities.RemoveRange(entities);
        
        #endregion
    }
}
