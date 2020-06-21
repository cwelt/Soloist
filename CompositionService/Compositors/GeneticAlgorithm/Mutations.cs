using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmCompositor : Compositor
    {

        private protected virtual void ChordPitchMutation(MelodyCandidate melody, int barIndex)
        {
            ChangePitchForARandomNote(melody.Bars[barIndex], mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected virtual void ScalePitchMutation(MelodyCandidate melody, int barIndex)
        {
            ChangePitchForARandomNote(melody.Bars[barIndex], mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected virtual void DurationEqualSplitMutation(MelodyCandidate melody, int barIndex)
        {
            DurationSplitOfARandomNote(melody.Bars[barIndex], DurationSplitRatio.Equal);
        }

        private protected virtual void DurationAnticipationSplitMutation(MelodyCandidate melody, int barIndex)
        {
            DurationSplitOfARandomNote(melody.Bars[barIndex], DurationSplitRatio.Anticipation);
        }

        private protected virtual void DurationDelaySplitMutation(MelodyCandidate melody, int barIndex)
        {
            DurationSplitOfARandomNote(melody.Bars[barIndex], DurationSplitRatio.Delay);
        }

        #region ReverseChordNotesMutation()
        /// <summary>
        /// Reverses the order of a sequence of notes for a randomly selected
        /// chord in the given bar, or in a randomly selected bar if <paramref name="barIndex"/> is null.
        /// <para> The revese operation is made in place locally to the selected chord. 
        /// The chord notes are determined by the logic in 
        /// <see cref="Bar.GetOverlappingNotesForChord(IChord, out IList{int})"/>.</para>
        /// </summary>
        /// <param name="melody"> The candidate melody to operate on.</param>
        /// <param name="barIndex"> Index of the bar to do the reverse in. If no bar index supplied, then a random bar would be selected. </param>
        private protected virtual void ReverseChordNotesMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // initialize random generator 
            Random randomizer = new Random();

            // if no specific bar has been requested then set it randomly 
            int selectedBarIndex = barIndex ?? randomizer.Next(melody.Bars.Count);
            IBar selectedBar = melody.Bars[selectedBarIndex];

            // select a random chord from within the selected bar 
            int randomChordIndex = randomizer.Next(selectedBar.Chords.Count);
            IChord selectedChord = selectedBar.Chords[randomChordIndex];

            // create a single chord element sequence 
            IChord[] chordsToReverse = new[] { selectedChord };

            // delegate reverse operation to superclass 
            PermutateNotes(selectedBar, chordsToReverse, Permutation.Reversed);
        }
        #endregion

        #region ReverseBarNotesMutation()
        /// <summary>
        /// Reverses the order of the note sequence in the given bar, 
        /// or in a randomly selected bar if <paramref name="barIndex"/> is null.
        /// <para> The revese operation is made in place locally to each chord. 
        /// The chord notes are determined by the logic in 
        /// <see cref="Bar.GetOverlappingNotesForChord(IChord, out IList{int})"/>.</para>
        /// </summary>
        /// <param name="melody"> The candidate melody to operate on.</param>
        /// <param name="barIndex"> Index of the bar to do the reverse in. If no bar index supplied, then a random bar would be selected. </param>
        private protected virtual void ReverseBarNotesMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // if no specific bar has been requested then set it randomly 
            int selectedBarIndex = barIndex ?? new Random().Next(melody.Bars.Count);
            IBar selectedBar = melody.Bars[selectedBarIndex];

            // delegate reverse operation to superclass 
            PermutateNotes(selectedBar, permutation: Permutation.Reversed);
        }
        #endregion

        #region ReverseAllNotesMutation()
        /// <summary>
        /// Reverses the order of all note sequences in the given melody.
        /// <para> The revese operation is made in place locally to each chord. 
        /// The chord notes are determined by the logic in 
        /// <see cref="Bar.GetOverlappingNotesForChord(IChord, out IList{int})"/>.</para>
        /// </summary>
        /// <param name="melody"> The candidate melody to operate on.</param>
        private protected virtual void ReverseAllNotesMutation(MelodyCandidate melody)
        {
            // delegate reverse operation to superclass 
            foreach (IBar bar in melody.Bars)
            {
                PermutateNotes(bar, permutation: Permutation.Reversed);
            }
        }
        #endregion



        #region ToggleFromHoldNoteMutation()
        /// <summary>
        /// Replaces a random hold note with a concrete note pitch. 
        /// <para> This method selectes a random bar from within the melody 
        /// that contains a hold note, and replaces it with a "regular" note 
        /// by setting the pitch to the adjacent preceding note, if such exists,
        /// or to the adjacent succeeding note, otherwise. </para>
        /// </summary>
        /// <param name="melody"> The candidate melody which contains the bar sequence to operate on. </param>
        /// <returns> True if a note replacement has been made successfully, false otherwise. </returns>
        private protected virtual bool ToggleFromHoldNoteMutation(MelodyCandidate melody)
        {
            // find all bars which contain hold notes 
            IList<IBar> barsWithHoldNotes = melody.Bars
                .Where(bar => bar.Notes.Any(note => note.Pitch == NotePitch.HoldNote)).ToList();

            // assure there at least one bar found 
            if (!barsWithHoldNotes.Any())
                return false;

            // select a random bar from the collection found
            IBar selectedBar = barsWithHoldNotes[new Random().Next(barsWithHoldNotes.Count)];
            int selectedBarIndex = melody.Bars.IndexOf(selectedBar);

            // get the hold note from the selected bar 
            INote holdNote = selectedBar.Notes.First();
            int holdNoteIndex = selectedBar.Notes.IndexOf(holdNote);

            // find adjacent preceding or succeeding sounded note (not a rest or a hold note)
            int adjacentNoteIndex, adjacentNoteBarIndex;
            INote adjacentNote =
                GetPredecessorNote(melody.Bars, excludeRestHoldNotes: true, selectedBarIndex, holdNoteIndex, out adjacentNoteIndex, out adjacentNoteBarIndex)
                ??
                GetSuccessorNote(melody.Bars, excludeRestHoldNotes: true, selectedBarIndex, holdNoteIndex, out adjacentNoteIndex, out adjacentNoteBarIndex);

            // assure an adjacent note has been found   
            if (adjacentNote != null)
            {
                // replace the hold note with the pitch found 
                INote newNote = new Note(adjacentNote.Pitch, holdNote.Duration);
                selectedBar.Notes.RemoveAt(holdNoteIndex);
                selectedBar.Notes.Insert(holdNoteIndex, newNote);

                // indicate the change has been made successfully
                return true;
            }

            // indicate that no change has been made  
            else return false;
        }
        #endregion

        #region SyncopedNoteMutation()
        /// <summary>
        /// Syncopes a bar's first note by preceding it's start time to it's preceding bar,
        /// on behalf of the duration of it' preceding note (last note from preceding bar).
        /// <para> The bar containing the note to be syncoped must meet some requirments: 
        /// It cannot be empty, nor it's preceding bar either. It cann't start with a 
        /// hold note or a rest note, and it's first note and it's preceding note must 
        /// have durations with a denominator which is a power of two. This constraint is 
        /// held inorder to maintain balanced durations when making aritmetic operations 
        /// on the duration of the syncoped note and it's preceding note. If the requested 
        /// bar does not meet these constraints then a random bar which does is selected 
        /// instead. If no such bar is found then this method retuns false. otherwise it returns true.
        /// </para>
        /// </summary>
        /// <param name="melody"> The candidate melody which contains the bar sequence. </param>
        /// <param name="barIndex"> The index of the bar which contains the requested 
        /// note to be syncoped. If no index is mentioned or if the mentioned index is 
        /// invalid in terms of the syncope operation, then a random bar would be selected instead.</param>
        /// <returns> Ture if a successful syncope has been made, false otherwise. </returns>
        private protected virtual bool SyncopedNoteMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // initialization 
            IBar selectedBar = null;
            IBar precedingBar = null;
            int selectedBarIndex = -1;

            // if no specific legal bar index is requested then set it randomly  
            if (!barIndex.HasValue || !IsBarLegalForSyncope(melody.Bars[(int)barIndex], (int)barIndex))
            {
                // select only bars which meet the operation's requirements 
                IBar[] relevantBars = melody.Bars.Where(IsBarLegalForSyncope).ToArray();

                // assure a valid bar for the syncope operation has been found 
                if (!relevantBars.Any())
                    return false;

                // select a bar randomly from the bars found 
                int randomIndex = new Random().Next(relevantBars.Length);
                selectedBar = relevantBars[randomIndex];
                selectedBarIndex = melody.Bars.IndexOf(selectedBar);
            }
            else // selected the requested valid bar from the input parameter 
            {
                selectedBarIndex = (int)barIndex;
                selectedBar = melody.Bars[selectedBarIndex];
            }

            // fetch the first note from within the selected bar    
            INote originalNote = selectedBar.Notes[0];

            // fetch the preceding note (last note from the preceding bar)
            int precedingBarIndex = selectedBarIndex - 1;
            precedingBar = melody.Bars[precedingBarIndex];
            int precedingNoteIndex = melody.Bars[precedingBarIndex].Notes.Count - 1;
            INote precedingNote = melody.Bars[precedingBarIndex].Notes[precedingNoteIndex];

            /* replace bar's first note with a new hold syncoped note which will hold the 
             * pitch that would now be started early from the preceding bar */
            INote newNote = new Note(NotePitch.HoldNote, originalNote.Duration);
            selectedBar.Notes.RemoveAt(0);
            selectedBar.Notes.Insert(0, newNote);

            /* replace last note from preceding bar with new note that has the original's
             * note pitch, and make original note hold the note from preceding bar: */

            // case 1: preceding note's length is too short for splitting (8th note or shorter) - replace it directly
            if ((precedingNote.Duration.Numerator / (float)precedingNote.Duration.Denominator) <= Duration.EighthNoteFraction)
            {
                INote newPrecedingNote = new Note(originalNote.Pitch, precedingNote.Duration);
                precedingBar.Notes.RemoveAt(precedingNoteIndex);
                precedingBar.Notes.Insert(precedingNoteIndex, newPrecedingNote);
            }

            /* case 2: preceding note is long enough for splitting. 
             * split preceding note into two new notes:
             * the first would retain it's original pitch, and the second 
             * would contain the selected note's pitch from the succeeding bar 
             * inorder to form a syncope.
             * The duration of the second note would be set to an 8th note,
             * and the duration of the first note would be set to the remainding length. */
            else
            {
                INote newPrecedingNote1, newPrecedingNote2;
                //if (precedingNote.Duration.Denominator )
                Duration eigthDuration = new Duration(1, 8);
                newPrecedingNote1 = new Note(precedingNote.Pitch, precedingNote.Duration.Subtract(eigthDuration));
                newPrecedingNote2 = new Note(originalNote.Pitch, eigthDuration);

                // replace the old preceding note with the two new preceding notes 
                precedingBar.Notes.RemoveAt(precedingNoteIndex);
                precedingBar.Notes.Insert(precedingNoteIndex, newPrecedingNote1);
                precedingBar.Notes.Insert(precedingNoteIndex + 1, newPrecedingNote2);
            }

            // return true to indicate a successful change has been made 
            return true;

            #region Local Function - isBarLegalForSyncope()
            //local function for validation candidate bar for syncope
            bool IsBarLegalForSyncope(IBar bar, int index)
            {
                // assure the bar has a preceding bar before it 
                if (index <= 0 || index >= melody.Bars.Count)
                    return false;

                // assure the the bar and it's preceding bar are not empty 
                IBar predecessorBar = melody.Bars[index - 1];
                if (!bar.Notes.Any() || !predecessorBar.Notes.Any())
                    return false;

                // assure first note of bar is not a rest or hold note 
                if (bar.Notes[0].Pitch == NotePitch.HoldNote ||
                    bar.Notes[0].Pitch == NotePitch.RestNote)
                    return false;

                /* assure bar's first note and it's preceding note from
                 * last the preceding bar have a denominator which is a 
                 * power of two, inorder to keep the durations balanced */
                int predecessorNoteIndex = predecessorBar.Notes.Count - 1;
                INote predecessorNote = predecessorBar.Notes[predecessorNoteIndex];
                if (!bar.Notes[0].Duration.IsDenominatorPowerOfTwo() ||
                    !predecessorNote.Duration.IsDenominatorPowerOfTwo())
                    return false;

                // all required validations passed, return true 
                return true;
            }
            #endregion
        }
        #endregion

    }


}
