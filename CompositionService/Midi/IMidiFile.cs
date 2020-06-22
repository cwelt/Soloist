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
        string FilePath { get; }

        /// <summary> The actual content the MIDI file. </summary>
        object Content { get;  }

        /// <summary> MIDI Sequence Name from the header track meta events. </summary>
        string Title { get; }

        /// <summary> BPM <a href="https://bit.ly/2LIuuVE">(Beats Per Minute).</a> </summary>
        byte BeatsPerMinute { get;  }

        /// <summary> Total number of bars in the MIDI file. </summary>
        int NumberOfBars { get; }

        /// <summary> The key signature from the MIDI meta events. </summary>
        IDuration KeySignature { get; }

        /// <summary> Global lowest pitch in the entire midi file. </summary>
        NotePitch? LowestPitch { get; }

        /// <summary> Global highest pitch in the entire midi file. </summary>
        NotePitch? HighestPitch { get; }

        /// <summary>
        /// Get local lowest and highest pitches contained in the given midi track.
        /// </summary>
        /// <param name="trackIndex"> One-based index of the track to be searched for pitch range.</param>
        /// <param name="lowestPitch"> The lowest pitch in the given track, or null, if track contains no notes.</param>
        /// <param name="highestPitch"> The highest pitch in the given track, or null, if track contains no notes. </param>
        void GetPitchRangeForTrack(int trackIndex, out NotePitch? lowestPitch, out NotePitch? highestPitch);

        /// <summary> The tracks contained in the MIDI file. </summary>
        ICollection<IMidiTrack> Tracks { get; }

        /// <summary> Starts playing the MIDI events containd in the file. </summary>
        void Play();

        /// <summary> Starts playing the MIDI events containd in the file. </summary>
        Task PlayAsync();


        /// <summary> Stops playing the MIDI data. </summary>
        void Stop();


        /// <summary>
        /// <para> 
        /// Removes the the requested track from the midi file, 
        /// and returns the removed track content as a bar sequence 
        /// in a music-theory representationthe containing the notes from the melody track 
        /// if requested, i.e., if the reference <paramref name="melodyBars"/> is properly initialized. 
        /// If it is set to null, then the requested track would still be removed from midi file, 
        /// but it won't be returned for further processing.
        /// </para>
        /// This can be useful when the midi file already contains a melody which we would
        /// like to remove and replace with a new one: the original one is removed from file and returned to caller,
        /// so it could be further processed if necessary and serve as a seed for generation of new melodies. 
        /// Inorder to later embed a new melody track instead of the one removed, 
        /// see the method for embeding a melody inside the midi file: 
        /// <see cref="EmbedMelody(IList{IBar}, MusicalInstrument, byte?)"/>
        /// </summary>
        /// <param name="trackNumber"> The zero-based position index of the melody track intended for removal from the midi file.</param>
        /// <param name="melodyBars"> Container for the extracted melody. 
        /// If set to null, then the midi track would be removed from the midi file with no
        /// further processing, and it's content would not be returned. 
        /// If set to a proper bar sequence, 
        /// then the melody contained in the removed midi track is returned 
        /// in and embedded this parameter decoded as musical notes in the bars' note collections. 
        /// For extracting the melody into the bar seuqence provided in this reference parameter, 
        /// the bars supplied in this parameter should be fully initialized with the exact amount 
        /// of bars and duration compatible to the midi's file structure. </param>
        /// <returns> If <paramref name="melodyBars"/> is to null, no result is returned. 
        /// If set, then the  melody from the removed track is returned as a music-theory melody
        /// embedded inside the <paramref name="melodyBars"/> reference parameter. </returns>
        void ExtractMelodyTrack(byte trackNumber, in IList<IBar> melodyBars = null);

        /// <summary> Converts a melody contained in a collection of bars
        ///  into a midi track and adds it to the midi file. </summary>
        /// <param name="melody"> List of notes divided into bars. </param>
        /// <param name="instrument">"> If specified, the new track would be inserted in this index position. </param>
        /// <param name="trackIndex"> Optional Index of the requested position to put the embedded melody track in.
        /// if no explict position is requested, leave this parameter null, and the new track would be appended.</param>
        void EmbedMelody(IList<IBar> melody, MusicalInstrument instrument = MusicalInstrument.JazzGuitar, byte? trackIndex = null);



        // <param name="P Midi <a href="https://bit.ly/30pmSzP">program number</a>  which represents a musical instrument.</param>


        /// <summary> Save midi file on local device. </summary>
        /// <param name="outputPath"> The path in which to save the midi file. </param>
        /// <param name="fileNamePrefix"> Optional prefix for the new midi file. </param>
        /// <returns> The full path of the saved file. </returns>
        string SaveFile(string outputPath = null, string fileNamePrefix = "");

        /// <summary> Returns the specified bar's duration. </summary>
        /// <param name="barIndex"> Zero-based index of the bar in the midi file.</param>
        /// <returns> Duration defined in the bar's key signature on the midi file. </returns>
        IDuration GetBarDuration(int barIndex);
    }
}
