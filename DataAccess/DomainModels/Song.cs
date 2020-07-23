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
        public string MidiPlaybackFileName { get; set; }
        public string ChordsFileName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool IsPublic { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }

    /// <summary>
    /// Enumeration that classifies the persisted files for a given song resource:
    /// <list type="number">
    /// <item><strong>Chord File</strong> - The chord progression file. </item>
    /// <item><strong>MIDI File</strong> - The original MIDI file. </item>
    /// <item><strong>MIDI Playback</strong> - The MIDI file without the melody track. </item>
    /// </list>
    /// </summary>
    public enum SongFileType
    {
        ChordProgressionFile,
        MidiOriginalFile,
        MidiPlaybackFile
    }
}
