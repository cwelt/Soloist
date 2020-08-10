using System.Collections.Generic;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;

namespace CW.Soloist.DataAccess.Repositories
{
    /// <summary>
    /// Repository for managing song entities. 
    /// This class is intended for extending the default general functionality 
    /// of the basic repository implementation, such as custom queries for songs. 
    /// </summary>
    public class SongRepository : EFRepository<Song, int>, ISongRepostiory
    {
        // Constructor 
        public SongRepository(ApplicationDbContext context) : base(context) { }
    }
}
