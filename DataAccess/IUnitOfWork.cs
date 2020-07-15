using CW.Soloist.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        ISongRepostiory Songs { get; }

        /// <summary>
        /// Updates the underlying database asynchroniously.
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateDbAsync();

        int UpdateDb();
    }
}
