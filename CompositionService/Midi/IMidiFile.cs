using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Provides a high level interface for a <a href="https://bit.ly/3bRVzQT"> SMF (Standard MIDI File) </a>.
    /// </summary>
    public interface IMidiFile
    {
        /// <summary> Absolute physical path of the MIDI file. </summary>
        string FilePath { get; set; }

        /// <summary> MIDI Sequence Name from the header track meta events. </summary>
        string Title { get; set; }

        /// <summary> BPM <a href="https://bit.ly/2LIuuVE">(Beats Per Minute).</a> </summary>
        byte BeatsPerMinute { get;  }

        /// <summary> Total number of bars in the MIDI file. </summary>
        int NumberOfBars {get; }

        /// <summary> The key signature from the MIDI meta events. </summary>
        string KeySignature { get; }

        /// <summary> The tracks contained in the MIDI file. </summary>
        ICollection<IMidiTrack> Tracks { get; set; }

        /// <summary> Starts playing the MIDI events containd in the file. </summary>
        void Play();

        /// <summary> Stops playing the MIDI data. </summary>
        void Stop();


                /// <summary>
        /// <para> 
        /// Remove the the requested track from the midi file, 
        /// and return it's content in a music-theory enumerable bar collection representation.
        /// </para>
        /// This can be useful when the midi file already contains a melody which we would
        /// like to replace with a new one: the original one is removed and returned,
        /// so it could be further processed if necessary and serve as a seed for generation of new melodies. 
        /// Inorder to embed a new melody track instead of the removed one, <see cref="EmbedMelody(IEnumerable{IBar}, string, byte)"/>
        /// </summary>
        /// <param name="trackNumber"> The number of the melody track in the midi file.</param>
        /// <returns> The melody track encoded as a music-theory melody. </returns>
        IEnumerable<IBar> ExtractMelody(byte trackNumber);


        /// <summary>
        /// Converts a melody contained in an enumerable collection of bars 
        /// into a midi track and adds it to the midi file. 
        /// </summary>
        /// <param name="melody"></param>
        /// <param name="melodyTrackName"></param>
        /// <param name="instrumentId"></param>
        void EmbedMelody(IEnumerable<IBar> melody, string melodyTrackName = "Melody", byte instrumentId = 64);

    }
}
