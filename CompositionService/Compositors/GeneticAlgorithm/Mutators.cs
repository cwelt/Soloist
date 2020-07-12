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
            _barMutations = new Dictionary<Action<MelodyCandidate, int?>, double>
            {
                [ChordPitchMutation]            = 1F,
                [ScalePitchMutation]            = 1F,
                [DurationSplitMutation]         = 0.25,
                [DurationUnifyMutation]         = 0.25,
                [SwapTwoNotesMutation]          = 1F,
                [ReverseChordNotesMutation]     = 1F,
                [ReverseBarNotesMutation]       = 1F,
                [ToggleFromHoldNoteMutation]    = 1F,
                [ToggleToHoldNoteMutation]      = 1F,
                [SyncopedNoteMutation]          = 0.8
            };
        }


        #region Mutate()
        /// <summary>
        /// Alter the state of candidate solutions.
        /// </summary>
        protected internal void Mutate()
        {
            // initialization 
            int randomIndex;
            Random random = new Random();
            Action<MelodyCandidate, int?> mutationMethod;
            double mutationProbability, randomProbability;
            IEnumerable<MelodyCandidate> newbies, elders, candidatesForMutation;

            // fetch the new born candidates from current generation  
            newbies = _candidates.Where(c => c.Generation == _currentGeneration).Shuffle();

            // fetch the elder existing candidates from previous generations
            elders = _candidates.Except(newbies).Shuffle();

            // union some candidates from both populations, new born and elders
            candidatesForMutation = newbies
                .Take((int)(newbies.Count() * 0.75))
                .Union(elders.Take(elders.Count() / 2));

            // modify each candidate individually 
            foreach (var candidate in candidatesForMutation)
            {
                // modify each bar individually 
                for (int i = 0; i < candidate.Bars.Count; i++)
                {
                    // select a random mutation from the bar mutation dictionary 
                    randomIndex = random.Next(_barMutations.Count);
                    mutationMethod = _barMutations.Keys.ElementAt(randomIndex);
                    mutationProbability = _barMutations.Values.ElementAt(randomIndex);

                    /* conditionlly invoke selected mutation method according to it's
                     * mutation probability and a randomly selected probability */
                    randomProbability = random.NextDouble();
                    if (mutationProbability > randomProbability)
                        mutationMethod(candidate, i);
                }

                // mark the current candidate as "dirty" for fitness evalutation
                candidate.IsDirty = true;
            }

            // update mutation probabilities 
            for (int i = 0; i < _barMutations.Count; i++)
            {
                var mutationEntry = _barMutations.ElementAt(i);
                if (_barMutations[mutationEntry.Key] - MutationProbabilityStep >= MinMutationProbability)
                    _barMutations[mutationEntry.Key] -= MutationProbabilityStep;
                else _barMutations[mutationEntry.Key] = MinMutationProbability;
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


        #region DurationUnifyMutation()
        /// <summary>
        /// Randomly selects two consecutive notes in the given bar, or in a randomly selected
        /// bar if <paramref name="barIndex"/> is null, and unifies thw two consecutive notes 
        /// into one note by removing the consecutive note entirely and adding it's 
        /// duration length to the first note. 
        /// </summary>
        /// <param name="melody"> The candidate melody to operate on.</param>
        /// <param name="barIndex"> Index of the bar to do the unification in. If no bar index supplied, then a random bar would be selected. </param>
        private protected virtual void DurationUnifyMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // initialization 
            INote note1, note2, newNote;
            IDuration newDuration;
            int note1Index, note2Index;
            Random random = new Random();

            // if no specific bar has been requested then set it randomly 
            int selectedBarIndex = barIndex ?? random.Next(melody.Bars.Count);
            IBar bar = melody.Bars[selectedBarIndex];

            // assure selected bar contains at least two notes 
            if (bar.Notes.Count < 2)
                return;

            // randomly select two consecutive notes from within the selected bar 
            note1Index = random.Next(1, bar.Notes.Count);
            note2Index = note1Index - 1;
            note1 = bar.Notes[note1Index];
            note2 = bar.Notes[note2Index];

            /* create a new note with the pitch of the first note and overall duration 
             * duration of the sum of the two notes durations */
            newDuration = note1.Duration.Add(note2.Duration);
            newNote = new Note(note1.Pitch, newDuration);

            // replace the two consecutive note with the new note 
            bar.Notes.RemoveAt(note1Index);
            bar.Notes.Insert(note1Index, newNote);
            bar.Notes.RemoveAt(note2Index);
        }
        #endregion

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
            NoteDurationSplit(melody.Bars[index], DurationSplitRatio.Equal);
        }

        private protected virtual void DurationAnticipationSplitMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            NoteDurationSplit(melody.Bars[index], DurationSplitRatio.Anticipation);
        }

        private protected virtual void DurationDelaySplitMutation(MelodyCandidate melody, int? barIndex)
        {
            int index = barIndex ?? new Random().Next(melody.Bars.Count);
            NoteDurationSplit(melody.Bars[index], DurationSplitRatio.Delay);
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
        /// Swaps the positions of two chord notes in the given bar,  
        /// or in a randomly selected bar if <paramref name="barIndex"/> is null.
        /// <para>A random chord in the selected bar is selected, and two randomly
        /// selected notes which are played under this chord's time span are swapped
        /// by their positions.</para>
        /// </summary>
        /// <param name="melody"> The candidate melody to operate on.</param>
        /// <param name="barIndex"> Index of the bar to do the swap in. If no bar index supplied, then a random bar would be selected. </param>
        private protected virtual void SwapTwoNotesMutation(MelodyCandidate melody, int? barIndex = null)
        {
            // initialization 
            INote note1, note2;
            IList<int> noteIndices;
            Random random = new Random();
            int note1Index, note2Index, randomIndex1, randomIndex2, chordIndex;

            // if no specific bar has been requested then set it randomly 
            int selectedBarIndex = barIndex ?? random.Next(melody.Bars.Count);
            IBar bar = melody.Bars[selectedBarIndex];

            chordIndex = random.Next(bar.Chords.Count);
            bar.GetOverlappingNotesForChord(chordIndex, out noteIndices);

            // assure selected chord in bar contains at least two notes 
            if (noteIndices.Count < 2)
                return;

            // select two random notes from withing the selected chord notes  
            randomIndex1 = random.Next(1, noteIndices.Count);
            randomIndex2 = random.Next(0, randomIndex1);
            note1Index = noteIndices[randomIndex1];
            note2Index = noteIndices[randomIndex2];
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
