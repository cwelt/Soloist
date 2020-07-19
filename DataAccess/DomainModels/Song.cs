using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.DomainModels
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public IMidiFile Midi { get; set; }
        public MelodyTrackIndex? MelodyTrackIndex { get; set; }
        public IEnumerable<IChord> Chords { get; set; }
        public string MidiFileName { get; set; }
        public string ChordsFileName { get; set; }
    }
}
