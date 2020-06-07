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
        public byte MinOctave { get; } = 4;

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        public byte MaxOctave { get; } = 6;

        /// <summary> Compose a solo-melody over a given playback. </summary>
        /// <param name="chordProgression"> The chords of the song in the playback. </param>
        /// <param name="seed"> Optional existing melody on which to base the composition on.</param>
        /// <returns> The composition of solo-melody</returns>
        internal abstract IEnumerable<IBar> Compose(IEnumerable<IBar> chordProgression, IEnumerable<IBar> seed = null);

        #region ChangePitchForARandomNote() 
        /// <summary>
        /// <para> Selects a random note in a bar and changes it's pitch to another note.</para>    
        /// <para> The new pitch would be selected randomly from a list which is determined by the 
        /// <paramref name="mappingSource"/> parameter. 
        /// For example, if the chord that is played in parallel to the randomly selected note is C major, 
        /// and mapping source is set to chord, then one of the pitches C (do), E (mi), or G (sol) 
        /// would be selected, otherwise, if mapping source is set to scale, then possible pitches
        /// are C (do), D (re), E (mi), F (fa), G (sol), A (la) and B (si). 
        /// In short, scale mapping has a larger variety of possible new ptches. </para>
        /// <para> Pitch selected is in the range between <see cref="MinOctave"/> and <see cref="MaxOctave"/>.
        /// In addition to this global range, it is possible to define another constraint 
        /// which determines "how far" at most the new pitch can be relative to the original 
        /// pitch. The default is one octave at most, see <paramref name="octaveRadius"/>. </para>
        /// <para>The new note with the randomly selected pitch will replace the original old note,
        /// but will preserve the original note's duration. </para>
        /// </summary>
        /// <remarks> 
        /// No pitch change is made to rest and hold notes. Incase the randomly 
        /// selected chord has no relevant notes played in parallel, or if the notes found 
        /// are not in the specified octave range, then no action would be taken and 
        /// the bar would remain intact. Therefore it is possible that some calls to this 
        /// method would sometimes make no change whatsoever. Incase a change is made 
        /// the method returns true, otherwise, it returns false. 
        /// </remarks>
        /// <param name="bar"> The bar in which to make the pitch replacement in. </param>
        /// <param name="mappingSource"> Determines whether the pitches should be selected from the chord arpeggio notes or from a scale mapped to the chord.</param>
        /// <param name="octaveRadius"> Determines how many octaves at most could the new pitch be from the old pitch (excluding the note itself, i.e, one octave is 11 semi-tones).</param>
        /// <returns> True if a change has been made, or false if no change has been made, see remarks.</returns>
        private protected virtual bool ChangePitchForARandomNote(IBar bar, ChordNoteMappingSource mappingSource = ChordNoteMappingSource.Chord, byte octaveRadius = 1)
        {
            // initialize random number generator 
            Random randomizer = new Random();

            // select a random chord from within the bar 
            int randomChordIndex = randomizer.Next(0, bar.Chords.Count);
            IChord chord = bar.Chords[randomChordIndex];

            // get non-rest-hold notes in bar that are played in parallel to selected chord 
            IList<INote> currentNotes = bar.GetOverlappingNotesForChord(chord)
                .Where(note => note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote)
                .ToList();

            // assure there is at least one note to replace under the selected chord 
            if (currentNotes.Count == 0)
                return false;

            // select a random note from within the current notes to be replaced 
            int randomNoteIndex = randomizer.Next(0, currentNotes.Count);
            INote oldNote = currentNotes[randomNoteIndex];
            int oldPitch = (int)oldNote.Pitch;

            // get candidate pitches according to mapping source and octave range
            IEnumerable<NotePitch> candidateNotes;
            if (mappingSource == ChordNoteMappingSource.Chord)
                candidateNotes = chord.GetArpeggioNotes(MinOctave, MaxOctave);
            else candidateNotes = chord.GetScaleNotes(MinOctave, MaxOctave);

            // filter candidats according to specified octave radius
            int pitchUpperBound = oldPitch + MusicTheoryServices.SemitonesInOctave-1;
            int pitchLowerBound = oldPitch - (MusicTheoryServices.SemitonesInOctave-1);
            NotePitch[] filteredPitches = candidateNotes
                .Where(pitch => pitch != oldNote.Pitch && (int)pitch >= pitchLowerBound && (int)pitch <= pitchUpperBound)
                .ToArray();

            // assure there is at least one pitch in the specified range
            if (filteredPitches.Length == 0)
                return false;

            // select a random pitch for the new note 
            int randomPitchIndex = randomizer.Next(0, filteredPitches.Length);
            NotePitch newPitch = filteredPitches[randomPitchIndex];

            // replace old note with new note with the selected pitch 
            INote newNote = new Note(newPitch, oldNote.Duration);
            int indexOfOldNoteInBar = bar.Notes.IndexOf(oldNote);
            bar.Notes.RemoveAt(indexOfOldNoteInBar);
            bar.Notes.Insert(indexOfOldNoteInBar, newNote);

            // indicate that a change has been made
            return true;
        }
        #endregion
    }

}
