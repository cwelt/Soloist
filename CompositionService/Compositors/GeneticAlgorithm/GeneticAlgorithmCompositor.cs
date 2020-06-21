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
        private uint _currentGeneration = 1;
        private protected Action<MelodyCandidate, int?>[] _mutations;

        public GeneticAlgorithmCompositor()
        {
            _mutations = new Action<MelodyCandidate, int?>[10];

        }


        /// <inheritdoc/>
        internal override IList<IBar> Compose(
            IList<IBar> chordProgression,
            IList<IBar> melodyInitializationSeed = null,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E6)
        {
            // initialize general parameters for the algorithm 
            _candidates = new List<MelodyCandidate>(120);
            _currentGeneration = 1;
            ChordProgression = chordProgression;
            Seed = melodyInitializationSeed;
            MinPitch = minPitch;
            MaxPitch = maxPitch;

            switch (overallNoteDurationFeel)
            {
                case OverallNoteDurationFeel.Slow:
                    DefaultDurationFraction = Duration.QuaterNoteFraction;
                    DefaultDurationDenomniator = Duration.QuaterNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Medium:
                default:
                    DefaultDurationFraction = Duration.EighthNoteFraction;
                    DefaultDurationDenomniator = Duration.EighthNoteDenominator; 
                    break;
                case OverallNoteDurationFeel.Intense:
                    DefaultDurationFraction = Duration.SixteenthNoteFraction;
                    DefaultDurationDenomniator = Duration.SixteenthNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Extreme:
                    DefaultDurationFraction = Duration.ThirtySecondNoteFraction;
                    DefaultDurationDenomniator = Duration.ThirtySecondNoteDenominator;
                    break;
            }
            DefaultDuration = new Duration(1, DefaultDurationDenomniator);

            // get first generatiion 
            PopulateFirstGeneration();

            int i = 0;
            bool terminateCondition = false;

            while (!terminateCondition)
            {
                // mix and combine pieces of different individuals 
                Crossover();

                // modify parts of individuals 
                Mutate(_candidates[0].Bars);

                // rate each individual 
                EvaluateFitness();

                // natural selection 
                SelectNextGeneration();

                //MelodyGenome.CurrentGeneration++;
                if (++i == 1)
                    terminateCondition = true;
            }

            // TODO: convert internal genome representation of each candidate in to a MIDI track chunk representation
            return _candidates[0].Bars;
        }


        /// <summary>
        /// Initialize first generation of solution candidates. 
        /// </summary>
        protected internal void PopulateFirstGeneration()
        {
            Console.WriteLine($"In {this.GetType().FullName}, \ninitializing first generation");
            MelodyCandidate candidate;
            if (Seed != null)
            {
                candidate = new MelodyCandidate(_currentGeneration, Seed, includeExistingMelody: true);
            }
            else
            {
                candidate = new MelodyCandidate(_currentGeneration, ChordProgression);
            }

            _candidates.Add(candidate);
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

            var melodyCandidate = _candidates[0];

            for (int i = 0; i < melodyCandidate.Bars.Count; i++)
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
