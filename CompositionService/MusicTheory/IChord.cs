using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a single musical chord instance. 
    /// <para>
    /// Each chord is represented by a chord root, a chord type and a chord duration.
    /// </para>
    /// </summary>
    internal interface IChord
    {
        /// <summary> Name of the note which is the root of the chord. </summary>
        NoteName ChordRoot { get; }

        /// <summary> Type of the chord (<see cref="ChordType"/>) </summary>
        ChordType ChordType { get; }

        /// <summary> The chords's duration (<see cref="INoteDuration"/>) .</summary>
        INoteDuration Duration { get; set; }


        /// <summary>
        /// Returns an enumerator that can be used to iterate through the collection 
        /// of all note pitches which correspond to this chord's root and type, 
        /// and fullfill the requested octave range constraint. 
        /// </summary>
        /// <param name="minOctave"> Minimum octave for note pitches.</param>
        /// <param name="maxOctave"> Maximum octave for note pitches</param>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection of notes in the chord.
        /// Only note pitches which are within the requested range are enumerated. 
        /// </returns>
        IEnumerable<NotePitch> GetNotes(int minOctave, int maxOctave);
    }
}
