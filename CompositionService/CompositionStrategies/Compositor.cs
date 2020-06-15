using CW.Soloist.CompositionService.CompositionStrategies.ArpeggiatorStrategy;
using CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy;
using CW.Soloist.CompositionService.CompositionStrategies.UtilEnums;
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
        internal IEnumerable<IBar> Seed { get; }

        /// <summary> The outcome of the <see cref="Compose"/> method. </summary>
        internal IEnumerable<IBar> ComposedMelody { get; private set; }

        /// <summary> The playback's harmony. </summary>
        internal IEnumerable<IBar> ChordProgression { get; private set; }

        /// <summary> Default duration denominator for a single note. </summary>
        internal byte DefaultDuration { get; set; } = 8;

        internal byte ShortestDuration { get; } = 16;


        /// <summary> Minimum octave of note pitch range for the composition. </summary>
        public byte MinOctave { get; } = 4;

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        public byte MaxOctave { get; } = 5;

        /// <summary> Minimum octave of note pitch range for the composition. </summary>
        public byte MinPitch { get; } = 40; //E2

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        public byte MaxPitch{ get; } = 88; // E6


        /// <summary> Compose a solo-melody over a given playback. </summary>
        /// <param name="chordProgression"> The chords of the song in the playback. </param>
        /// <param name="seed"> Optional existing melody on which to base the composition on.</param>
        /// <returns> The composition of solo-melody</returns>
        internal abstract IEnumerable<IBar> Compose(IEnumerable<IBar> chordProgression, IEnumerable<IBar> seed = null);

        internal static Compositor CreateCompositor(CompositionStrategy strategy = CompositionStrategy.GeneticAlgorithmStrategy)
        {
            switch (strategy)
            {
                case CompositionStrategy.GeneticAlgorithmStrategy:
                    return new GeneticAlgorithmCompositor();
                case CompositionStrategy.ArpeggiatorStrategy:
                    return new ArpeggiatorCompositor();
                case CompositionStrategy.ScaleStrategy:
                    throw new NotImplementedException();
                case CompositionStrategy.CustomStrategy:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

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
        /// pitch. The default is seven semitones at most, see <paramref name="semitoneRadius"/>. </para>
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
        /// <param name="semitoneRadius"> Determines how many semitones at most could the new pitch be from the exisiting one.</param>
        /// <returns> True if a change has been made, or false if no change has been made, see remarks.</returns>
        private protected virtual bool ChangePitchForARandomNote(IBar bar, ChordNoteMappingSource mappingSource = ChordNoteMappingSource.Chord, byte semitoneRadius = 5)
        {
            // initialize random number generator 
            Random randomizer = new Random();

            // select a random chord from within the bar 
            int randomChordIndex = randomizer.Next(0, bar.Chords.Count);
            IChord chord = bar.Chords[randomChordIndex];

            // get non-rest-hold notes in bar that are played in parallel to selected chord 
            IList<INote> currentNotes = bar.GetOverlappingNotesForChord(chord, out IList<int> notesIndices)
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

            // filter candidats according to specified semitone radius
            int pitchUpperBound = oldPitch + semitoneRadius;
            int pitchLowerBound = oldPitch - semitoneRadius;
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

            // print to log 
            Console.WriteLine($"chord {chord.ChordRoot}{chord.ChordType} - old: {oldNote.Pitch}, new: {newPitch}");

            // indicate that a change has been made
            return true;
        }
        #endregion

        #region DurationSplitOfARandomNote()
        /// <summary>
        /// Replaces a random note in the given bar with two new shorter notes 
        /// which preserve the original note's pitch, but have a new duration
        /// which sums up together to the original's note duration. 
        /// <para> The duration split is done according to the <paramref name="ratio"/>
        /// parameter. </para>
        /// <para> Incase the note's in the bar are too short to be splited, no action 
        /// is made and the method returns false. Otherwise, it return true. </para>
        /// </summary>
        /// <remarks> The shortest possible duration is determined by the corresponding 
        /// input parameter to the compositor's constructor. 
        /// parameter </remarks>
        /// <param name="bar"> The bar in which to make the note's duration split. </param>
        /// <param name="ratio"> The ratio of the duration split. </param>
        /// <returns> True if a split has been made, false otherwise. </returns>
        private protected virtual bool DurationSplitOfARandomNote(IBar bar, DurationSplitRatio ratio)
        {
            // extract denominator from the given ratio 
            int ratioDenominator = ((ratio == DurationSplitRatio.Equal) ? 2 : 4);

            // find candidate notes which are long enough for the given split ratio 
            INote[] candidateNotes = bar.Notes
                .Where(note => note.Duration.Denominator <= ShortestDuration / ratioDenominator)
                .ToArray();

            // assure there is at least one candidate note which long enough for splitting  
            if (candidateNotes.Length == 0)
                return false;

            // select a random note from within the bar candidate notes 
            int randomNoteIndex = new Random().Next(candidateNotes.Length);
            INote existingNote = candidateNotes[randomNoteIndex];

            // set the two new durations for the two new notes after the split
            IDuration firstDuration, secondDuration;
            byte existingNumerator = existingNote.Duration.Numerator;
            byte existingDenominator = existingNote.Duration.Denominator;
            byte newDenominator = (byte)(existingDenominator * ratioDenominator);

            switch (ratio)
            {
                case DurationSplitRatio.Equal:
                default:
                    firstDuration = new Duration(existingNumerator, newDenominator);
                    secondDuration = new Duration(firstDuration);
                    break;
                case DurationSplitRatio.Anticipation:
                    firstDuration = new Duration(existingNumerator, newDenominator);
                    secondDuration = new Duration((byte)(existingNumerator * 3), newDenominator);
                    break;
                case DurationSplitRatio.Delay:
                    firstDuration = new Duration((byte)(existingNumerator * 3), newDenominator);
                    secondDuration = new Duration(existingNumerator, newDenominator);
                    break;
            }

            // create two new notes with preseted durtions 
            INote firstNote = new Note(existingNote.Pitch, firstDuration);
            INote secondNote = new Note(existingNote.Pitch, secondDuration);

            // replace the existing note with the two new notes 
            int originalNoteIndex = bar.Notes.IndexOf(existingNote);
            bar.Notes.RemoveAt(originalNoteIndex);
            bar.Notes.Insert(originalNoteIndex, firstNote);
            bar.Notes.Insert(originalNoteIndex + 1, secondNote);

            // indicate that a change has been made
            return true;
        }
        #endregion


        #region GetPredecessorNote()
        /// <summary> 
        /// <para>Gets the note in the melody which preceds the note at the given indices.</para>
        /// If <paramref name="excludeRestHoldNotes"/> is set, then hold and rest notes  
        /// would be bypassed, and the first preceding note which is not a rest or hold note 
        /// would be returned. If no preceding note is found then null is returned. 
        /// </summary>
        /// <param name="melodyBars"> The bar sequence which contains the melody notes. </param>
        /// <param name="excludeRestHoldNotes">If set, rest notes and hold notes would be discarded during search for a preceding note.</param>
        /// <param name="barIndex"> Index of the bar containing the given note. </param>
        /// <param name="noteIndex"> Index of the note of whom it's predecessor is wanted. </param>
        /// <param name="precedingNoteBarIndex"> Index of the bar which contains the preceding note.</param>
        /// <param name="precedingNoteIndex">Index of the preceding note inside his containing note sequence.</param>
        /// <returns> Preceding note in the melody, or null if no predecessor note is found. </returns>
        private protected INote GetPredecessorNote(IList<IBar> melodyBars, bool excludeRestHoldNotes, int barIndex, int noteIndex, out int precedingNoteBarIndex, out int precedingNoteIndex)
        {
            // initialization 
            INote note = null;
            int startingNoteIndex = 0;

            /* start scanning backwards from current bar & current note:
             * outer loop is for bars, inner loop for notes in the individual bars */
            for (int i = barIndex; i >= 0; i--)
            {
                /* in current bar start searching right before the given note.
                 * in the rest of the bars start from the right edge end of the bar. */
                startingNoteIndex = ((i == barIndex) ? (noteIndex - 1) : (melodyBars[i].Notes.Count - 1));
                for (int j = startingNoteIndex; j >= 0; j--)
                {
                    note = melodyBars[i].Notes[j];
                    if (excludeRestHoldNotes || (note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote))
                    {
                        // set out params with the indices values and return the preceding note 
                        precedingNoteBarIndex = i;
                        precedingNoteIndex = j;
                        return note;
                    }
                }
            }

            // incase no preceding note is found, set the output accordingly 
            precedingNoteBarIndex = -1;
            precedingNoteIndex = -1;
            return null;
        }
        #endregion



        #region GetSuccessorNote()
        /// <summary> 
        /// <para>Gets the note in the melody which succeeds the note at the given indices.</para>
        /// If <paramref name="excludeRestHoldNotes"/> is set, then hold and rest notes  
        /// would be bypassed, and the first succeeding note which is not a rest or hold note 
        /// would be returned. If no succeeding note is found then null is returned. 
        /// </summary>
        /// <param name="melodyBars"> The bar sequence which contains the melody notes. </param>
        /// <param name="excludeRestHoldNotes">If set, rest notes and hold notes would be discarded during search for a preceding note. </param>
        /// <param name="barIndex"> Index of the bar containing the given note. </param>
        /// <param name="noteIndex"> Index of the note of which it's predecessor is wanted. </param>
        /// <param name="succeedingNoteBarIndex">Index of the bar which contains the successor note. </param>
        /// <param name="succeedingNoteIndex">Index of the successor note inside his containing note sequence. </param>
        private protected INote GetSuccessorNote(IList<IBar> melodyBars, bool excludeRestHoldNotes, int barIndex, int noteIndex, out int succeedingNoteBarIndex, out int succeedingNoteIndex)
        {
            // initialization 
            INote note = null;
            int startingNoteIndex = 0;

            // start scanning forwards from current bar & current note 
            for (int i = barIndex; i < melodyBars.Count; i++)
            {
                /* in current bar start searching right after the given note.
                 * in the rest of the bars start from the right beginning of the bar. */
                startingNoteIndex = ((i == barIndex) ? (noteIndex + 1) : 0);
                for (int j = startingNoteIndex; j < melodyBars[i].Notes.Count; j++)
                {
                    note = melodyBars[i].Notes[j];
                    if (excludeRestHoldNotes || (note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote))
                    {
                        // set out params with the indices values and return the succeeding note 
                        succeedingNoteBarIndex = i;
                        succeedingNoteIndex = j;
                        return note;
                    }
                }
            }
            // incase no succeeding note is found, set the output accordingly 
            succeedingNoteBarIndex = -1;
            succeedingNoteIndex = -1;
            return null;
        }
        #endregion

        #region PermutateNotes()
        /// <summary>
        /// Generates a permutation of the bar's note sequence and replaces the original
        /// sequence with the permutated sequence (for example, shuffled or reversed order).
        /// <para>The available permutations are defined in <see cref="Permutation"/>, 
        /// and the requested permutation should be mentioned in the <paramref name="permutation"/>
        /// input parameter. If no explicit permutation is requested, a default one would take place. </para>
        /// <para> Inorder to maintain the consonance of the notes under the chords which 
        /// are played underthem in parallel, the permutation is done for each chord
        /// individually, i.e., "inplace" in the bounds of the chord associated notes indices. </para>
        /// <para> The <paramref name="chords"/> defines a subsequence of bar's chord sequence
        /// to take action on. If no explicit chords are requested then the method will 
        /// default to permutate the entire bar.</para>
        /// </summary>
        /// <param name="bar"> The bar to operate on.</param>
        /// <param name="chords"> Subsequence of the chords in the bar that are
        /// the target of the permutation. If not set, then the default is to 
        /// permutate all chord in the bar, i.e., the notes of all chords in the bar.</param>
        /// <param name="permutation"> Kind of permutation to apply on the order of the note sequence. </param>
        private protected virtual void PermutateNotes(IBar bar, IEnumerable<IChord> chords = null, Permutation permutation = Permutation.Shuffled)
        {
            /* if no subset of chords has been requested, 
             * set it to the entire bar chord sequence */ 
            chords = chords ?? bar.Chords;

            foreach (IChord chord in chords)
            {
                // get selected chord's notes and their indices 
                IList<INote> chordNotes = bar.GetOverlappingNotesForChord(chord, out IList<int> chordNotesIndices);

                // get a permutation of the chord's note sequence 
                IList<INote> permutatedChordNotes;
                switch (permutation)
                {
                    case Permutation.Shuffled:
                    default:
                        chordNotes.Shuffle();
                        permutatedChordNotes = chordNotes;
                        break;
                    case Permutation.Reversed:
                        permutatedChordNotes = chordNotes.Reverse().ToList();
                        break;
                    case Permutation.SortedAscending:
                        permutatedChordNotes = chordNotes.OrderBy(note => note.Pitch).ToList();
                        break;
                    case Permutation.SortedDescending:
                        permutatedChordNotes = chordNotes.OrderByDescending(note => note.Pitch).ToList();
                        break;
                }

                // apply the permutation to the bar's note sequence 
                int noteIndex;
                for (int i = 0; i < chordNotesIndices.Count; i++)
                {
                    noteIndex = chordNotesIndices[i];
                    bar.Notes.RemoveAt(noteIndex);
                    bar.Notes.Insert(noteIndex, permutatedChordNotes[i]);
                }
            }
        }
        #endregion
    }
}
