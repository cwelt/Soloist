using System;
using System.Threading.Tasks;
using CW.Soloist.DataAccess.Repositories;

namespace CW.Soloist.DataAccess.UnitOfWork
{
    /// <summary>
    /// This interface represents a partial high-level abstraction of the 
    /// object-relational behavioral pattern known as the 
    /// <a href="https://martinfowler.com/eaaCatalog/unitOfWork.html">
    ///'unit of work</a>, as described in Martin Fowler's book:
    /// <a href="https://bit.ly/3kAuW8L"> Patterns of Enterprise Application Architecture</a>.
    /// This pattern is responsible for keeping track of in-memory changes that  
    /// should be mapped back to the physical records in the database once a commit 
    /// is requested. 
    /// <para> 
    /// Moreover, this pattern serves as a bridge or gateway to the underlying database,
    /// by managing a set of repositories - one repository for each managed entity,
    /// so the datastore is abstracted away and all access is done indirectly through
    /// the relevant repositories managed by the unit of work. 
    ///  </para>
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // repositories of entities managed by this unit of work interface.
        ISongRepostiory Songs { get; }

        /// <summary> Registers a tracked entity as dirty so it's corresponding 
        /// record would get updated in the database in th end of the unit of 
        /// work when a request to commit the changes is made. </summary>
        /// <param name="dirtyEntity"> The tracked dirty entity instance. </param>
        void RegisterDirty(object dirtyEntity);

        /// <summary> Updates the underlying database.</summary>
        /// <returns> The number of successfully updated records in the underlying datastore. </returns>
        int CommitChanges();

        /// <summary> Updates the underlying database asynchroniously.</summary>
        /// <returns> The number of successfully updated records in the underlying datastore. </returns>
        Task<int> CommitChangesAsync();
    }
}
