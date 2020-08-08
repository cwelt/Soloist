using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.Repositories
{
    /// <summary>
    /// <para> Generic interface which represents a repository for managing entites 
    /// of a certain type (the type described by the <typeparamref name="TEntity"/>
    /// parameter. </para>
    /// The repository interface reflects the 
    /// <a href="https://bit.ly/2PB7sls"> repository pattern </a>  
    /// described by martin fowler in his book
    /// <a href="https://bit.ly/3gE9AVm"> Patterns of Enterprise Application Architecture </a>. 
    /// </summary>
    /// <typeparam name="TEntity"> The type of domain object entites managed in this repository. </typeparam>
    /// <typeparam name="TPKeyType"> The type of the primary key that uniquely identifies domain object entities in this repository. </typeparam>
    public interface IRepository<TEntity, TPKeyType> where TEntity : class
    {
        #region Methods For Searching & Querying 
        #region Get
        /// <summary>
        /// Gets the entity that is identifed by the given id.
        /// </summary>
        /// <param name="id"> The id that identifies the requested entity.</param>
        /// <returns> the entity that is identifed by the given id. </returns>
        TEntity Get(TPKeyType id);
        #endregion

        #region GetAsync
        /// <inheritdoc cref="Get(TPKeyType)"/>
        Task<TEntity> GetAsync(TPKeyType id);
        #endregion

        #region GetAll
        /// <summary>
        /// Get all entities from this repository
        /// </summary>
        /// <returns> An enumerable sequence of entities from this repository. </returns>
        IEnumerable<TEntity> GetAll();
        #endregion

        #region GetAllAsync
        /// <inheritdoc cref="GetAll"/>
        Task<List<TEntity>> GetAllAsync();
        #endregion

        #region Find
        /// <summary>
        /// Gets all entities from this repository which satisfy a given condition.
        /// </summary>
        /// <param name="predicate"> The function to check the condition that the entities are required to satisfy.</param>
        /// <returns> An enumerable sequence of all entities from this repository which satisfy the given condition. </returns>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region FindAsync
        /// <inheritdoc cref="Find(Expression{Func{TEntity, bool}})"/>
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region SingleOrDefault
        /// <summary>
        /// Returns the only entity from this repository that satisfies a specified 
        /// condition or a default value if no such entity exists.
        /// </summary>
        /// <param name="predicate"> The function to check the condition that the entity is required to satisfy.</param>
        /// <returns> The single entity from this repository that satisfies the 
        /// condition in the predicate, or default(TEntity) if no such element is found.
        ///</returns>
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region SingleOrDefaultAsync
        /// <inheritdoc cref="SingleOrDefault(Expression{Func{TEntity, bool}})"/>
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #endregion

        #region Methods For Adding & Removing Elements

        #region Add
        /// <summary> Adds the given entity to this repositoy. </summary>
        /// <param name="entity"> The entity to add to this repository. </param>
        void Add(TEntity entity);
        #endregion

        #region AddRange
        /// <summary> Adds the given sequence of entities to this repository. </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<TEntity> entities);
        #endregion

        #region Remove
        /// <summary> Removes the given entity from this repository. </summary>
        /// <param name="entity"> The enetity to remove from this repository. </param>
        void Remove(TEntity entity);
        #endregion

        #region RemoveRange
        /// <summary>
        /// Removes the all entities in the given sequence from this repostiory.
        /// </summary>
        /// <param name="entities"> The sequence of entities to remove. </param>
        void RemoveRange(IEnumerable<TEntity> entities);
        #endregion

        #endregion
    }
}
