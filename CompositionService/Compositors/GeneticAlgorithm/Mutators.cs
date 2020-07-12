using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmCompositor : Compositor
    {
       
        private protected virtual void RegisterMutators()
        {
            _singleBarMutations = new Action<MelodyCandidate, int?>[] 
            {
                ChordPitchMutation,
                ScalePitchMutation,

                DurationSplitMutation,

                SwapTwoNotesMutation,

                ReverseChordNotesMutation,
                ReverseBarNotesMutation,

                ToggleFromHoldNoteMutation,
                ToggleToHoldNoteMutation,

                SyncopedNoteMutation,
            };
        }
        
        
        #region Mutate()
        /// <summary>
        /// Alter a candidate's solution state.
        /// </summary>
        protected internal void Mutate()
        {
            Random random = new Random();
            int randomIndex;
            Action<MelodyCandidate, int?> barMutation;

            IEnumerable<MelodyCandidate> newbies, elders, candidatesForMutation;

            newbies = _candidates.Where(c => c.Generation == _currentGeneration);

            elders = _candidates.Except(newbies).Shuffle();

            candidatesForMutation = newbies.Union(elders.Take(elders.Count() / 2));

            foreach (var candidate in candidatesForMutation)
            {
                for (int i = 0; i < candidate.Bars.Count; i++)
                {
                    randomIndex = random.Next(_singleBarMutations.Length);
                    barMutation = _singleBarMutations[randomIndex];
                    barMutation(candidate, i);
                }
            }
        }
        #endregion


        private protected virtual void ChordPitchMutation(MelodyCandidate melody, int? barIndex)
        {
            int selectedBarIndex = barIndex ?? new Random().Next(melody.Bars.Count);
            ChangePitchForARandomNote(melody.Bars[selectedBarIndex], mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected virtual void ScalePitchMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            ChangePitchForARandomNote(melody.Bars[index], mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected virtual void DurationSplitMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            Action<MelodyCandidate, int?>[] durationSplitters =
            {
                DurationEqualSplitMutation,
                DurationAnticipationSplitMutation,
                DurationDelaySplitMutation
            };

            int randomIndex = new Random().Next(durationSplitters.Length);
            Action<MelodyCandidate, int?> durationSplitMutation = durationSplitters[randomIndex];
            durationSplitMutation(melody, barIndex);
        }

        private protected virtual void DurationEqualSplitMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            DurationSplitOfARandomNote(melody.Bars[index], DurationSplitRatio.Equal);
        }

        private protected virtual void DurationAnticipationSplitMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            DurationSplitOfARandomNote(melody.Bars[index], DurationSplitRatio.Anticipation);
        }

        private protected virtual void DurationDelaySplitMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            DurationSplitOfARandomNote(melody.Bars[index], DurationSplitRatio.Delay);
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
        /// <para> This method selectes a bar from within the melody 
        /// that contains a hold note, and replaces it with a "regular" note 
        /// by setting the pitch to the adjacent preceding note, if such exists,
        /// or otherwise, to the adjacent succeeding note. </para>
        /// </summary>
        /// <param name="melody"> The candidate melody which contains the bar sequence to operate on. </param>
        /// <param name="barIndex"> An index of specific requested bar to operate on, which 
        /// contains a hold note. If set to null, or if requested bar does not contain any 
        /// hold notes, then some other bar which contains a hold note would be selected, 
        /// if such bar exists. </param>
        private protected virtual void ToggleFromHoldNoteMutation(MelodyCandidate melody, int? barIndex)
        {
            // delegate actual mutation to base class 
            ToggleAHoldNote(melody.Bars, barIndex);
        }
        #endregion

        #region ToggleToHoldNoteMutation()
        private protected virtual void ToggleToHoldNoteMutation(MelodyCandidate melody, int? barIndex)
        {
            // intialize random generator 
            Random random = new Random();

            // fetch the requested bar randomly select one if no specific bar is requested 
            barIndex = barIndex ?? random.Next(melody.Bars.Count);
            IBar selectedBar = melody.Bars[(int)barIndex];

            // fetch potential notes for the toggle (filter rest, hold & first notes in bar)
            IList<INote> relevantNotes = selectedBar.Notes.Where((note, noteIndex) =>
                noteIndex > 0 &&
                note.Pitch != NotePitch.RestNote &&
                note.Pitch != NotePitch.HoldNote).ToList();

            // assure at least one note as found 
            if (!relevantNotes.Any())
                return;

            // fetch a random note from the potential notes found
            int randomNoteIndex = random.Next(relevantNotes.Count);
            INote selectedNote = relevantNotes[randomNoteIndex];

            // get original index of the selected note in the original sequence 
            int originalNoteIndex = selectedBar.Notes.IndexOf(selectedNote);

            // replace selected note with a hold note
            INote holdNote = new Note(NotePitch.HoldNote, selectedNote.Duration);
            selectedBar.Notes.RemoveAt(originalNoteIndex);
            selectedBar.Notes.Insert(originalNoteIndex, holdNote);
        }
        #endregion

        #region SyncopedNoteMutation()
        /// <summary> <inheritdoc cref="Compositor.SyncopizeANote(IList{IBar}, int?)"/></summary>
        /// <param name="melody"> The candidate melody which contains the bar sequence to operate on. </param>
        /// <param name="barIndex"> <inheritdoc cref="Compositor.SyncopizeANote(IList{IBar}, int?)"/> "</param>
        private protected virtual void SyncopedNoteMutation(MelodyCandidate melody, int? barIndex = null)
        {
            SyncopizeANote(melody.Bars, barIndex);
        }
        #endregion

        #region SwapTwoNotesMutation()
        /// <summary>
        /// Swaps the positions of two notes in the given bar, 
        /// or in a randomly selected bar if <paramref name="barIndex"/> is null.
        /// </summary>
        /// <param name="melody"> The candidate melody to operate on.</param>
        /// <param name="barIndex"> Index of the bar to do the swap in. If no bar index supplied, then a random bar would be selected. </param>
        private protected virtual void SwapTwoNotesMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // initialization 
            INote note1, note2;
            int note1Index, note2Index;
            Random random = new Random();

            // if no specific bar has been requested then set it randomly 
            int selectedBarIndex = barIndex ?? random.Next(melody.Bars.Count);
            IBar bar = melody.Bars[selectedBarIndex];

            // assure selected bar contains at least two notes 
            if (bar.Notes.Count < 2)
                return;

            // select two random notes from within the selected bar 
            note1Index = random.Next(1, bar.Notes.Count);
            note2Index = random.Next(0, note1Index); // assure selected indices are distinct 
            note1 = bar.Notes[note1Index];
            note2 = bar.Notes[note2Index];

            // swap the notes positions in the bar   
            bar.Notes.RemoveAt(note1Index);
            bar.Notes.Insert(note1Index, note2);
            bar.Notes.RemoveAt(note2Index);
            bar.Notes.Insert(note2Index, note1);
        }
        #endregion

    }


}
