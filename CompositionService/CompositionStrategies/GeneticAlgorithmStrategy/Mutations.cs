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

        private protected virtual void SyncopedNoteMutation(MelodyCandidate melody, int? barIndex)
        {
            // initialize random number generator 
            Random randomizer = new Random();
            
            // if no specific bar index is requested then set it randomly 
            barIndex = barIndex ?? randomizer.Next(melody.Bars.Count);

            // fetch the bar 
            IBar bar = melody.Bars[(int)barIndex];

            // get all notes from bar which are not silent rest notes or hold notes
            IList<INote> notes = bar.Notes.Where(note =>
            {
                return note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote;
            }).ToList();

            // select a random note from within the notes found  
            int noteIndex = randomizer.Next(bar.Notes.Count);
            INote selectedNote = bar.Notes[noteIndex];

            // find the note preceding 
            ;


        }
    }
}
