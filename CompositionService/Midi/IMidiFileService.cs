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
    public interface IMidiFileService
    {
        /// <summary> Absolute physical path of the MIDI file. </summary>
        string FilePath { get; }

        /// <summary> MIDI Sequence Name from the header track meta events. </summary>
        string Title { get; }

        /// <summary> BPM <a href="https://bit.ly/2LIuuVE">(Beats Per Minute).</a> </summary>
        byte BeatsPerMinute { get;  }

        /// <summary> Total number of bars in the MIDI file. </summary>
        int NumberOfBars {get; }

        /// <summary> The key signature from the MIDI meta events. </summary>
        IDuration KeySignature { get; }

        /// <summary> The tracks contained in the MIDI file. </summary>
        ICollection<IMidiTrack> Tracks { get; }

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
        /// Inorder to embed a new melody track instead of the removed one, <see cref="EmbedMelody(IList{IBar}, string, byte, byte?)"/>
        /// </summary>
        /// <param name="trackNumber"> The number of the melody track in the midi file.</param>
        /// <param name="melodyBars"> The melody contained in the specified track is returned 
        /// in this parameter decoded as music notes in the bar's note collections. 
        /// The bars supplied in this parameter should be fully initialized with the amount of bars 
        /// and duration compatible to the midi's file structure. </param>
        /// <returns> The melody track encoded as a music-theory melody. </returns>
        void ExtractMelody(byte trackNumber, in IList<IBar> melodyBars);


        /// <summary> Converts a melody contained in a collection of bars
        ///  into a midi track and adds it to the midi file. </summary>
        /// <param name="melody"> List of notes divided into bars. </param>
        /// <param name="melodyTrackName"> Name that would be given to the new midi track. </param>
        /// <param name="instrument"> Midi <a href="https://bit.ly/30pmSzP">program number</a>  which represents a musical instrument.</param>
        /// <param name="trackIndex"> If specified, the new track would be inserted in this index position. </param>
        void EmbedMelody(IList<IBar> melody, string melodyTrackName = "Melody", MusicalInstrument instrument = MusicalInstrument.JazzGuitar, byte? trackIndex = null);


        /// <summary> Save midi file on local device. </summary>
        /// <param name="outputPath"> The path in which to save the midi file. </param>
        /// <param name="fileNamePrefix"> Optional prefix for the new midi file. </param>
        void SaveFile(string outputPath = null, string fileNamePrefix = "");

        /// <summary> Returns the specified bar's duration. </summary>
        /// <param name="barIndex"> Zero-based index of the bar in the midi file.</param>
        /// <returns> Duration defined in the bar's key signature on the midi file. </returns>
        IDuration GetBarDuration(int barIndex);
    }
}
