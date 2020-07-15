using CW.Soloist.CompositionService.Midi;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.Repositories
{
    public class SongRepository : Repository<Song, int>, ISongRepostiory
    {
        public SongRepository(SoloistContext context) 
            : base(context) { }


        private List<Song> GetSongsFromSampleData()
        {
            List<Song> songs = new List<Song>
            {
                new Song
                {
                    Title = "Evyatar Banai"
                }
            };
            return songs;
        }
    }
}
