using CW.Soloist.DataAccess.EntityFramework;
using CW.Soloist.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess
{
    /// <summary>
    /// This class provies a concrete implementation to the <see cref="IUnitOfWork"/>
    /// interface by reusing the existing standard functionality provided in the 
    /// Entity Framwork.  
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork
    {
        /// <summary> The application's underlying EF DbContext.</summary>
        private readonly ApplicationDbContext _dbContext;

        // repositories of entities managed by this unit of work interface.
        public ISongRepostiory Songs { get; private set; }

        /// <summary>
        /// Constructs a unit of work using the injected application's EF DbContext.
        /// </summary>
        /// <param name="dbContext"> The underlying EF DbContext that is to be injected. </param>
        public EFUnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Songs = new SongRepository(_dbContext);
        }


        /// <inheritdoc cref="IUnitOfWork.RegisterDirty(object)"/>
        public void RegisterDirty(object dirtyEntity)
        {
            _dbContext.Entry(dirtyEntity).State = EntityState.Modified;
        }


        /// <inheritdoc cref="IUnitOfWork.CommitChanges"/>
        public int CommitChanges() => _dbContext.SaveChanges();


        /// <inheritdoc cref="IUnitOfWork.CommitChangesAsync"/>
        public async Task<int> CommitChangesAsync() => await _dbContext.SaveChangesAsync();

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
