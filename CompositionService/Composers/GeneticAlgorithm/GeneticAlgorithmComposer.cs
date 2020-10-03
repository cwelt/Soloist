using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using CW.Soloist.CompositionService.Enums;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.GeneticAlgorithm
{
    /// <summary> <para> Compose a solo-melody over a given playback by implementing a genetic algorithm. </para>
    /// This class implements a concrete composition strategy (<see cref="Composer"/>)
    /// for use by a <see cref="CompositionContext"/> context instance. </summary>
    internal partial class GeneticAlgorithmComposer : Composer
    {
        #region Fields and Properties 

        /// <summary> Current iteration of the genetic algorithm. </summary>
        private protected uint _currentGeneration;

        /// <summary> The candidate generated melodies that are competeing to be selected. </summary>
        private protected List<MelodyCandidate> _candidates;

        /// <summary> Upper bound of the melody candidate population. </summary>
        private protected const int MaxPopulationSize = 50;

        /// <summary> Delegates of melody initialization methods. </summary>
        private protected Action<IEnumerable<IBar>>[] _initializers;

        /// <summary> Delegates of melody mutation methods and their probability to execute. </summary>
        private protected Dictionary<Action<MelodyCandidate, int?>, double> _barMutations;

        /// <summary> Step size between iterations for reducing probabilty of executing a mutation method. </summary>
        private protected const double MutationProbabilityStep = 0.025;

        /// <summary> Upper bound of generations for the algorithm to execute. </summary>
        private protected const int MaxNumberOfIterations = 120;

        /// <summary> Low bound of generations for the algorithm to execute. </summary>
        private protected const int MinNumberOfIterations = 50;

        /// <summary> Low bound of probability for a mutation method to execute. </summary>
        private protected const double MinMutationProbability = 0.05;

        /// <summary> Grade value that is high enough to stop the algorithms execution. </summary>
        private protected const double CuttingEvaluationGrade = 92;

        /// <summary> Evaluation methods set of propotional weights for their significance. </summary>
        internal protected MelodyEvaluatorsWeights EvaluatorsWeights { get; private set; }
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        public GeneticAlgorithmComposer()
        {

            // register initializers for first generation
            RegisterInitializers();

            // register bar mutators 
            RegisterMutators();
        }


        #endregion

        #region InitializeCompositionParams()
        private protected override void InitializeCompositionParams(IList<IBar> chordProgression, IList<IBar> melodyInitializationSeed = null, OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium, NotePitch minPitch = NotePitch.E2, NotePitch maxPitch = NotePitch.E6, params object[] additionalParams)
        {
            // delegate the general initialization to base class 
            base.InitializeCompositionParams(chordProgression, melodyInitializationSeed, overallNoteDurationFeel, minPitch, maxPitch);

            // initialize custom settings of the genetic algorithm 
            _currentGeneration = 0;
            _candidates = new List<MelodyCandidate>(MaxPopulationSize);

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

                // Check if termination conditions are met 
                if (++i == MaxNumberOfIterations 
                    || (_candidates.Select(c => c.FitnessGrade).Max() >= CuttingEvaluationGrade
                                                && _currentGeneration > MinNumberOfIterations))
                {
                    terminateCondition = true;
                }

                // Debugging statistics 
                if (Environment.UserInteractive)
                {
                    Console.WriteLine($"Generation: {_currentGeneration}, " +
                        $"Average: {_candidates.Select(c => c.FitnessGrade).Average(): 0.0000}\t" +
                        $"Best: {_candidates.Select(c => c.FitnessGrade).Max(): 0.0000}\t" +
                        $"Lowest: { _candidates.Select(c => c.FitnessGrade).Min(): 0.0000}\n");
                }
            }

            // return the result 
            IEnumerable<IList<IBar>> composedMelodies = _candidates
                .OrderByDescending(c => c.FitnessGrade)
                .Select(c => c.Bars);
            return composedMelodies;
        }
        #endregion
    }
}
