﻿using CW.Soloist.CompositionService.MusicTheory;
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

        private protected Action<MelodyCandidate, int?>[] _singleBarMutations;
        private protected Action<MelodyCandidate>[] _entireMelodyMutations;

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
        private protected override void InitializeCompositionParams(IList<IBar> chordProgression, IList<IBar> melodyInitializationSeed = null, OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium, NotePitch minPitch = NotePitch.E2, NotePitch maxPitch = NotePitch.E6)
        {
            base.InitializeCompositionParams(chordProgression, melodyInitializationSeed, overallNoteDurationFeel, minPitch, maxPitch);
            _currentGeneration = 0;
            _candidates = new List<MelodyCandidate>(120);
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
                if (++i == 32)
                    terminateCondition = true;
            }


            //foreach (var c in _candidates)
            //Console.WriteLine($"Generation: {c.Generation}, Grade: {c.FitnessGrade}");

            var composedMelodies = _candidates
                .OrderByDescending(c => c.FitnessGrade)
                .Select(c => c.Bars);
            return composedMelodies;
        }
        #endregion
    }
}
