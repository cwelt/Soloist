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

        private protected virtual void ToggleHoldNote(MelodyCandidate melody, int barIndex)
        {
            // find all bars which contain hold notes 
            IList<IBar> barsWithHoldNotes = melody.Bars
                .Where(bar => bar.Notes.FirstOrDefault(note => note.Pitch == NotePitch.HoldNote) != null).ToList();

            // assure there at least one bar found 
            if (barsWithHoldNotes.Count() == 0)
                return;

            // select a random bar from the collection found
            IBar selectedBar = barsWithHoldNotes[new Random().Next(barsWithHoldNotes.Count)];
            int selectedBarIndex = melody.Bars.IndexOf(selectedBar);

            // get the rest note from the selected bar 
            INote holdNote = selectedBar.Notes.First();
            int holdNoteIndex = selectedBar.Notes.IndexOf(holdNote);

            // find the real pitch of the note that this hold note holds
            NotePitch? notePitch = GetPredecessorNotePitch(melody.Bars, selectedBarIndex, holdNoteIndex);

            // if no preceding notes, get pitch from succeeding note or default to silent rest note 
            if(!notePitch.HasValue)
                notePitch = GetSuccessorNotePitch(melody.Bars, selectedBarIndex, holdNoteIndex) ?? NotePitch.RestNote;

            // replace the hold note with the pitch found 
            INote newNote = new Note((NotePitch)notePitch, holdNote.Duration);
            selectedBar.Notes.RemoveAt(holdNoteIndex);
            selectedBar.Notes.Insert(holdNoteIndex, newNote);
        }
    }
}
