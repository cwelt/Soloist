using CW.Soloist.CompositionService.Midi;
using CW.Soloist.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.Data.Repositories
{
    public class SongRepository : ISongRepostiory
    {
        private List<Song> _songs;
        public SongRepository()
        {
            _songs = GetSongsFromSampleData();
        }
        public IEnumerable<Song> GetAll()
        {
            return _songs;
        }

        public Song GetSong(int id)
        {
            return _songs.FirstOrDefault(song => song.Id == id);
        }

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
