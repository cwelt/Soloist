using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    /// <summary> <para> Compose a solo-melody over a given playback by implementing a genetic algorithm. </para>
    /// This class implements a concrete composition strategy (<see cref="Compositor"/>)
    /// for use by a <see cref="Composition"/> context instance. </summary>
    internal partial class GeneticAlgorithmCompositor : Compositor
    {
        private protected uint _currentGeneration;
        private protected List<MelodyCandidate> _candidates;
        private protected const int MaxPopulationSize = 60;

        private protected Action<IEnumerable<IBar>>[] _initializers;

        private protected Dictionary<Action<MelodyCandidate, int?>, double> _barMutations;
        private protected Action<MelodyCandidate>[] _entireMelodyMutations;
        private protected const double MutationProbabilityStep = 0.025;
        private protected const double MaxNumberOfIterations = 50;
        private protected const double MinMutationProbability = 0.05;

        internal protected MelodyEvaluatorsWeights EvaluatorsWeights { get; private set; }

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        public GeneticAlgorithmCompositor()
        {

            // register initializers for first generation
            RegisterInitializers();

            // register bar mutators 
            RegisterMutators();

            // register mutator which mutate the entire melody 
            _entireMelodyMutations = new Action<MelodyCandidate>[] {
                ReverseAllNotesMutation,
            };
        }


        #endregion

        #region InitializeCompositionParams()
        private protected override void InitializeCompositionParams(IList<IBar> chordProgression, IList<IBar> melodyInitializationSeed = null, OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium, NotePitch minPitch = NotePitch.E2, NotePitch maxPitch = NotePitch.E6, params object[] additionalParams)
        {
            // delegate the general initialization to base class 
            base.InitializeCompositionParams(chordProgression, melodyInitializationSeed, overallNoteDurationFeel, minPitch, maxPitch);

            // initialize custom setting of the genetic algorithm 
            _currentGeneration = 0;
            _candidates = new List<MelodyCandidate>(120);

            if (additionalParams.Length > 0 && additionalParams[0] is MelodyEvaluatorsWeights)
                EvaluatorsWeights = additionalParams[0] as MelodyEvaluatorsWeights;
            else EvaluatorsWeights = new MelodyEvaluatorsWeights();
        }
        #endregion


        #region GenerateMelody()
        /// <inheritdoc/>
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            // get first generatiion 
            PopulateFirstGeneration();

            int i = 0;
            bool terminateCondition = false;

            while (!terminateCondition)
            {
                // update generation counter 
                _currentGeneration++;

                // mix and combine pieces of different individuals 
                Crossover();

                // modify parts of individuals 
                Mutate();
                
                // rate each individual 
                EvaluateFitness();

                // natural selection 
                SelectNextGeneration();

                //MelodyGenome.CurrentGeneration++;
                if (++i == MaxNumberOfIterations || _candidates.Select(c => c.FitnessGrade).Max() >= 92
                    && _currentGeneration > 30)
                {
                    terminateCondition = true;
                }

                Console.WriteLine($"Generation: {_currentGeneration}, " +
                    $"Average: {_candidates.Select(c => c.FitnessGrade).Average() : 0.00}\t" +
                    $"Best: {_candidates.Select(c => c.FitnessGrade).Max() : 0.00}\t" +
                    $"Lowest: { _candidates.Select(c => c.FitnessGrade).Min() : 0.00}\n");
            }


            

            var composedMelodies = _candidates
                .OrderByDescending(c => c.FitnessGrade)
                .Select(c => c.Bars);
            return composedMelodies;
        }
        #endregion
    }
}
