using System;
using System.IO;
using System.Collections.Generic;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;


namespace CW.Soloist.DataAccess.DomainModels
{
    public class Song
    {
        #region Properties
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
        #endregion

        #region SetPlaybackName
        /// <summary>
        /// Sets the <see cref="MidiPlaybackFileName"/> property based on the given 
        /// reference name, or on the original midi file name, if no reference name is 
        /// supplied. The playback name is set by adding a playback identifier token between 
        /// the original name from the reference and the file extension.  
        /// </summary>
        /// <param name="referenceName">Fully qualified file name (including file extension) 
        /// for a reference, or null, if the reference should be taken from the original 
        /// midi file (<see cref="MidiFileName"/>).</param>
        public void SetPlaybackName(string referenceName = null)
        {
            // if no reference is requested, set it to original midi file name 
            referenceName = referenceName ?? MidiFileName;

            // get original file name components 
            string extension = Path.GetExtension(referenceName);
            string nameWithOutExtension = Path.GetFileNameWithoutExtension(referenceName);

            // set name of the playback name property 
            MidiPlaybackFileName = nameWithOutExtension + "_playback" + extension;
        }
        #endregion
    }


    #region SongFileType Enumeration
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
    #endregion
}
