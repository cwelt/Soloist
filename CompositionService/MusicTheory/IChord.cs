using System.Collections.Generic;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a single musical chord instance. 
    /// <para>
    /// Each chord is represented by a chord root, a chord type and a chord duration.
    /// </para>
    /// </summary>
    public interface IChord
    {
        /// <summary> Name of the note which is the root of the chord. </summary>
        NoteName ChordRoot { get; }

        /// <summary> Type of the chord (<see cref="ChordType"/>) </summary>
        ChordType ChordType { get; }

        /// <summary> The chords's duration (<see cref="IDuration"/>) .</summary>
        IDuration Duration { get; set; }


        /// <summary>
        /// Returns an enumerator that can be used to iterate through the collection 
        /// of all note pitches which correspond to this chord's root and type, 
        /// and full fill the requested range constraint. 
        /// </summary>
        /// <param name="minOctave"> Minimum octave for note pitches.</param>
        /// <param name="maxOctave"> Maximum octave for note pitches</param>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection of notes in the chord.
        /// Only note pitches which are within the requested range are enumerated. 
        /// </returns>
        IEnumerable<NotePitch> GetArpeggioNotes(int minOctave, int maxOctave);

        /// <summary> <inheritdoc cref="GetArpeggioNotes(int, int)"/> </summary>
        /// <param name="minPitch"> Lowest bound for note pitches height.</param>
        /// <param name="maxPitch"> Heighest bound for note pitches height. </param>
        /// <returns> <inheritdoc cref="GetArpeggioNotes(int, int)"/> </returns>
        IEnumerable<NotePitch> GetArpeggioNotes(NotePitch minPitch, NotePitch maxPitch);


        /// <summary>
        /// Returns an enumerator that can be used to iterate through the collection 
        /// of all note pitches which correspond to this chord's root and type mapping scale, 
        /// and fullfill the requested range constraint. 
        /// </summary>
        /// <param name="minOctave"> Minimum octave for note pitches.</param>
        /// <param name="maxOctave"> Maximum octave for note pitches</param>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection of notes in the scale mapped to this chord.
        /// Only note pitches which are within the requested range are enumerated. 
        /// </returns>
        IEnumerable<NotePitch> GetScaleNotes(int minOctave, int maxOctave);

        /// <summary> <inheritdoc cref="GetScaleNotes(int, int)"/> </summary>
        /// <param name="minPitch"> Lowest bound for note pitches height.</param>
        /// <param name="maxPitch"> Highest bound for note pitches height</param>
        /// <returns> <inheritdoc cref="GetScaleNotes(int, int)"/> </returns>
        IEnumerable<NotePitch> GetScaleNotes(NotePitch minPitch, NotePitch maxPitch);
    }
}
