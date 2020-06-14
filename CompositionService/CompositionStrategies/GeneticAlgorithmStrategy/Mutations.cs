using CW.Soloist.CompositionService.CompositionStrategies.UtilEnums;
using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    public partial class GeneticAlgorithmCompositor : Compositor
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

        /// <summary>
        /// Replaces a random hole note with a concrete note pitch. 
        /// </summary>
        /// <param name="melody"></param>
        private protected virtual void ToggleFromHoldNoteMutation(MelodyCandidate melody)
        {
            // find all bars which contain hold notes 
            IList<IBar> barsWithHoldNotes = melody.Bars
                .Where(bar => bar.Notes.Any(note => note.Pitch == NotePitch.HoldNote)).ToList();

            // assure there at least one bar found 
            if (barsWithHoldNotes.Count == 0)
                return;

            // select a random bar from the collection found
            IBar selectedBar = barsWithHoldNotes[new Random().Next(barsWithHoldNotes.Count)];
            int selectedBarIndex = melody.Bars.IndexOf(selectedBar);

            // get the rest note from the selected bar 
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
            }
        }

        private protected virtual void SyncopedNoteMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // initialization 
            Random randomizer = new Random();
            IBar selectedBar, precedingBar;
            int selectedBarIndex;

            // if no specific legal bar index is requested then set it randomly  
            if (!barIndex.HasValue || barIndex <= 0 || barIndex >= melody.Bars.Count
                || melody.Bars[(int)barIndex - 1].Notes.Count == 0)
            {
                /* select only bars which don't start with a rest note or a hold note, 
                 * and succeed a non-empty bar */
                IBar[] relevantBars = melody.Bars.Where((IBar bar, int index) =>
                {
                    return bar.Notes[0].Pitch != NotePitch.RestNote &&
                           bar.Notes[0].Pitch != NotePitch.HoldNote &&
                           index > 0 &&
                           melody.Bars[index - 1].Notes.Count > 0;
                }).ToArray();

                int randomIndex = randomizer.Next(relevantBars.Length);
                selectedBar = relevantBars[randomIndex];
                selectedBarIndex = melody.Bars.IndexOf(selectedBar);
            }
            else
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
             * note pitch: if the preceding note is short enough (shorter than 1/8 beat),
             * replace it directly, otherwise, split preceding note accordingly. */

            // case 1: preceding note's length is short enough (8th note or shorter)
            if ((precedingNote.Duration.Numerator / (float)precedingNote.Duration.Denominator) <= 0.125)
            {
                INote newPrecedingNote = new Note(originalNote.Pitch, precedingNote.Duration);
                precedingBar.Notes.RemoveAt(precedingNoteIndex);
                precedingBar.Notes.Insert(precedingNoteIndex, newPrecedingNote);
                return;
            }

            /* case 2: split preceding note into two new notes:
             * the first would contain the original pitch, and the second 
             * would contain the selected note's pitch to cause a syncope.
             * The duration of the second note would be set to an 8th note, 
              * The duration of the first note would set to the remainder.   */
            else
            {
                Duration eigthDuration = new Duration(1, 8);
                INote newPrecedingNote1 = new Note(precedingNote.Pitch, precedingNote.Duration.Subtract(eigthDuration));
                INote newPrecedingNote2 = new Note(originalNote.Pitch, eigthDuration);

                // replace the old preceding note with the two new preceding notes 
                precedingBar.Notes.RemoveAt(precedingNoteIndex);
                precedingBar.Notes.Insert(precedingNoteIndex, newPrecedingNote1);
                precedingBar.Notes.Insert(precedingNoteIndex + 1, newPrecedingNote2);
            }
        }
    }
}
