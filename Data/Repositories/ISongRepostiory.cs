using CW.Soloist.Data.Models;
using System.Collections.Generic;

namespace CW.Soloist.Data.Repositories
{
    public interface ISongRepostiory
    {
        IEnumerable<Song> GetAll();

        Song GetSong(int id);
    }
}