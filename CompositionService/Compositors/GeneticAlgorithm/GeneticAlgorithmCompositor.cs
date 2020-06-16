using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    /// <summary> <para> Compose a solo-melody over a given playback by implementing a genetic algorithm. </para>
    /// This class implements a concrete composition strategy (<see cref="Compositor"/>)
    /// for use by a <see cref="Composition"/> context instance. </summary>
    internal partial class GeneticAlgorithmCompositor : Compositor
    {
        private IList<MelodyCandidate> _candidates;
        private protected Action<MelodyCandidate, int?>[] _mutations;

        



        public GeneticAlgorithmCompositor()
        {
            _mutations = new Action<MelodyCandidate, int?>[10];

        }


        /// <inheritdoc/>
        internal override IEnumerable<IBar> Compose(IEnumerable<IBar> chordProgression, IEnumerable<IBar> seed = null)
        {
            // get first generatiion 
            InitializeFirstGeneration();

            int i = 0;
            bool terminateCondition = false;

            while (!terminateCondition)
            {
                // mix and combine pieces of different individuals 
                Crossover();

                // modify parts of individuals 
                Mutate(chordProgression);

                // rate each individual 
                EvaluateFitness();

                // natural selection 
                SelectNextGeneration();

                //MelodyGenome.CurrentGeneration++;
                if (++i == 1)
                    terminateCondition = true;
            }

            // TODO: convert internal genome representation of each candidate in to a MIDI track chunk representation
            return chordProgression;
        }


        /// <summary>
        /// Initialize first generation of solution candidates. 
        /// </summary>
        protected internal void InitializeFirstGeneration()
        {
            Console.WriteLine($"In {this.GetType().FullName}, \ninitializing first generation");
        }

        /// <summary>
        /// Crossover two solution candidates and generate new solutions. 
        /// </summary>
        protected internal void Crossover()
        {

        }

        /// <summary>
        /// Alter a candidate's solution state.
        /// </summary>
        protected internal void Mutate(IEnumerable<IBar> melody)
        {
            ArppegiateUp(melody);
            
            
            Random random = new Random();
            int NumberOfBars = melody.Count();
            int randomBarIndex = random.Next(NumberOfBars);

            var melodyCandidate = new MelodyCandidate
            {
                Bars = melody.ToList()
            };

            


            for (int i = 0; i < melodyCandidate.Bars.Count && false; i++)
            {
                SyncopedNoteMutation(melodyCandidate);
                PermutateNotes(melodyCandidate.Bars[random.Next(NumberOfBars)], permutation: Permutation.Shuffled);
                ChordPitchMutation(melodyCandidate, random.Next(NumberOfBars));
                DurationEqualSplitMutation(melodyCandidate, random.Next(NumberOfBars));
                PermutateNotes(melodyCandidate.Bars[random.Next(NumberOfBars)], permutation: Permutation.Reversed);
                ToggleFromHoldNoteMutation(melodyCandidate);
                ScalePitchMutation(melodyCandidate, random.Next(NumberOfBars));
                PermutateNotes(melodyCandidate.Bars[random.Next(NumberOfBars)], permutation: Permutation.Shuffled);
                SyncopedNoteMutation(melodyCandidate);
                DurationDelaySplitMutation(melodyCandidate, random.Next(NumberOfBars));
                ChordPitchMutation(melodyCandidate, random.Next(NumberOfBars));
                SyncopedNoteMutation(melodyCandidate);
                PermutateNotes(melodyCandidate.Bars[random.Next(NumberOfBars)], permutation: Permutation.SortedDescending);
            }


            //ReverseChordNotesMutation(melodyCandidates);
            //SyncopedNoteMutation(melodyCandidates, 3);
            //ToggleFromHoldNoteMutation(melodyCandidates); 

        }

        /// <summary>
        /// Evaluate the candidate solution's fitness.
        /// </summary>        
        protected internal void EvaluateFitness()
        {

        }


        /// <summary>
        /// Select the best evaluated solutions. 
        /// </summary>
        protected internal void SelectNextGeneration()
        {

        }
    }
}
