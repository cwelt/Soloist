using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies
{

    /// <summary>
    /// Abstract compositor strategy class. 
    /// Subclasses of this class implement concrete composition strategies. 
    /// <para><remarks>
    /// Composition strategies are used by the <see cref="Composition"/> context class.
    /// This class is the abstract strategy class in the strategy design pattern.
    /// </remarks></para>
    /// </summary>
    public abstract class Compositor
    {
        /// <summary> Melody seed to base on the composition of the new melody. </summary>
        internal IEnumerable<INote> Seed { get; }

        /// <summary> The outcome of the <see cref="Compose"/> method. </summary>
        internal IEnumerable<INote> ComposedMelody { get; private set; }

        /// <summary> The playback's harmony. </summary>
        internal IEnumerable<IBar> ChordProgression { get; }

        /// <summary> Default duration denominator for a single note. </summary>
        internal int DefaultDuration { get; set; } = 8;

        /// <summary> Minimum octave of note pitch range for the composition. </summary>
        public byte MinOctave { get; } = 3;

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        public byte MaxOctave { get; } = 5;

        /// <summary> Compose a solo-melody over a given playback. </summary>
        /// <param name="chordProgression"> The chords of the song in the playback. </param>
        /// <param name="seed"> Optional existing melody on which to base the composition on.</param>
        /// <returns> The composition of solo-melody</returns>
        internal abstract IEnumerable<IBar> Compose(IEnumerable<IBar> chordProgression, IEnumerable<IBar> seed = null);

        // TODO: Add parameter for source of pitches (chord, scale, or all chromtaic scale note)
        #region ChangePitchForARandomNote() 
        /// <summary><para> Selects a random note in a bar and changes it's pitch.</para>    
        /// For example, if the chord that is played in parallel to the randomly selected note is C major, 
        /// then one of the pitches C (do), E (mi), or G (sol) would be selected in a random octave
        /// in the range of 
        /// and a new note with the randomly selected pitch would replace the old note.
        /// The new note will preserve the original note's duration. 
        /// </summary>
        /// <param name="bar"> The bar in which to make the pitch replacement. </param>
        private protected virtual void ChangePitchForARandomNote(IBar bar)
        {
            // initialize random number generator 
            Random randomizer = new Random();

            // select a random chord from within the bar 
            int randomChordIndex = randomizer.Next(0, bar.Chords.Count);
            IChord chord = bar.Chords[randomChordIndex];

            // get current notes in bar that are played in parallel to selected chord 
            IList<INote> currentNotes = bar.GetOverlappingNotesForChord(chord);

            // assure there is at least one note to replace 
            if (currentNotes.Count > 0)
            {
                // select a random note from within the current notes to be replaced 
                int randomNoteIndex = randomizer.Next(0, currentNotes.Count);
                INote oldNote = currentNotes[randomNoteIndex];

                // get pitches that define the selected chord from within specified range
                NotePitch[] notePitches = chord.GetArpeggioNotes(MinOctave, MaxOctave)
                    .Where(pitch => pitch != oldNote.Pitch).ToArray();

                // select a random pitch for the new note 
                int randomPitchIndex = randomizer.Next(0, notePitches.Length);
                NotePitch newPitch = notePitches[randomPitchIndex];

                // replace old note with new not with the selected pitch 
                INote newNote = new Note(newPitch, oldNote.Duration);
                int indexOfOldNoteInBar = bar.Notes.IndexOf(oldNote);
                bar.Notes.RemoveAt(indexOfOldNoteInBar);
                bar.Notes.Insert(indexOfOldNoteInBar, newNote);

            }
        }
        #endregion
    }

}
