using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Factory for creating midi files and midi tracks instances,
    /// and providing them as abstract interfaces, 
    /// abstracting from clients the specific implementations they use,
    /// inversing the controll of dependencies ("Don't call us, We'll call you..."),
    /// such that high-level entites of the clients depened only on the abstract interfaces 
    /// and not on the concrete implementation that uses one third library or another. 
    /// </summary>
    internal static class MusicTheoryFactory
    {
        #region CreateDuration

        /// <summary>
        /// Constructs an instance of <see cref="IDuration"/> 
        /// with a duration of <paramref name="numerator"/> / <paramref name="denominator"/>.
        /// </summary>
        /// <param name="numerator">The number of beats in the duration.</param>
        /// <param name="denominator">Duration of a single beat in the duration</param>
        /// <param name="reduceToLowestTerms">If set, reduces the numerator and denominator do their lowest terms.</param>
        /// <returns></returns>
        internal static IDuration CreateDuration(byte numerator = 1, byte denominator = 4, bool reduceToLowestTerms = true)
        {
            return new Duration(numerator, denominator, reduceToLowestTerms);
        }


        /// <summary>
        /// constructs a <see cref="IDuration"/> instance 
        /// based on the <paramref name="duration"/> parameter values.
        /// </summary>
        /// <param name="duration"> The duration to base the values on. </param>
        /// <param name="reduceToLowestTerms">If set, reduces the numerator and denominator do their lowest terms.</param>
        /// <returns></returns>
        internal static IDuration CreateDuration(IDuration duration, bool reduceToLowestTerms = true)
        {
            return new Duration(duration, reduceToLowestTerms);
        }

        #endregion

        #region CreateNote

        /// <summary>
        /// Construct a note with given <paramref name="pitch"/> and a 
        /// duration composed with the quotient of the numerator 
        /// divided by the denominator. 
        /// </summary>
        /// <param name="pitch"> MIDI absolute pitch value. </param>
        /// <param name="duration"> Duration of new note. </param>
        internal static INote CreateNote(NotePitch pitch, IDuration duration) => new Note(pitch, duration);


        /// <summary>
        /// Constructs a new <see cref="INote"/> instance with 
        /// the given <paramref name="pitch"/> and <paramref name="duration"/>.
        /// </summary>
        /// <param name="pitch">MIDI absolute pitch value.</param>
        /// <param name="numerator">Duration numerator.</param>
        /// <param name="denominator">Duration denominator.</param>
        internal static INote CreateNote(NotePitch pitch, byte numerator, byte denominator) => new Note(pitch, numerator, denominator);


        /// <summary> Constructs a note with given<paramref name="pitch"/> and default duration. </summary>
        /// <param name="pitch">MIDI Absolute pitch of the constructed note.</param>
        internal static INote CreateNote(NotePitch pitch) => new Note(pitch);


        /// <summary> Constructs a note with the given note properties. </summary>
        /// <param note="pitch"> The <see cref="INote"/> instance to copy the properties from.</param>
        internal static INote CreateNote(INote note) => new Note(note);

        #endregion

        #region CreateChord
        /// <summary>
        /// Constructs a new <see cref="IChord"/> instance with the given
        /// <paramref name="root"/>, <paramref name="type"/> and <paramref name="duration"/>. 
        /// </summary>
        /// <param name="root"> The name of the note which is the chord's root. </param>
        /// <param name="type"> The type of the chord (<see cref="ChordType"/>).</param>
        /// <param name="duration"> The Duration of the chord (<see cref="IDuration"/>).</param>
        /// <returns> An <see cref="IChord"/> instance with the given <paramref name="root"/>, <paramref name="type"/> and <paramref name="duration"/>. </returns>
        internal static IChord CreateChord(NoteName root, ChordType type, IDuration duration)
        {
            return new Chord(root, type, duration);
        }
        #endregion

        #region CreateBar

        /// <summary>
        /// Creates a IBar instance based on the given time signature, chord progression and note sequence.
        /// </summary>
        /// <param name="timeSignature"> The bar's time signature.</param>
        /// <param name="chords"> The chord progression of the bar. </param>
        /// <param name="notes"> The note sequence played in this bar. </param>
        /// <returns> An IBar instance based on the given time signature, chord progression and note sequence. </returns>
        internal static IBar CreateBar(IDuration timeSignature, IList<IChord> chords, IList<INote> notes)
        {
            return new Bar(timeSignature, chords, notes);
        }

        /// <summary> Creates a IBar instance based with default values. </summary>
        /// <returns> An IBar instance initialized with default values. </returns>
        internal static IBar CreateBar() => new Bar();

        /// <summary>
        /// Creates a IBar instance based on the given time signature with empty chord and note collections.
        /// </summary>
        /// <returns> An IBar instance based on the given time signature, with empty chord and note collections.</returns>
        internal static IBar CreateBar(IDuration timeSignature) => new Bar(timeSignature);

        /// <summary>
        /// Creates a IBar instance based on the given time signature and chord progression,
        /// with an empty note sequence.
        /// </summary>
        /// <param name="timeSignature"> The bar's time signature.</param>
        /// <param name="chords"> The chord progression of the bar. </param>
        /// <returns> An IBar instance woth the given time signature and chord progression, and empty note sequence. </returns>
        internal static IBar CreateBar(IDuration timeSignature, IList<IChord> chords) => new Bar(timeSignature, chords);

        /// <summary> Creates a IBar instance based on the given bar properties. </summary>
        /// <param name="bar"> The bar to base the construction on.</param>
        /// <returns> An IBar based on the given bar properties.. </returns>
        internal static IBar CreateBar(IBar bar) => new Bar(bar);

        #endregion
    }
}
