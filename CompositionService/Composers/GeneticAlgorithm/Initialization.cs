using System;
using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmComposer : Composer
    {
        #region RegisterInitializers()
        /// <summary>
        /// Register initialization methods that world be called to initialize the first generation
        /// of the candidate melodies population.
        /// </summary>
        protected virtual void RegisterInitializers()
        {
            _initializers = new Action<IEnumerable<IBar>>[]
            {
                ArpeggiatorInitializerAscending,
                ArpeggiatorInitializerDescending,
                ArpeggiatorInitializerChordZigzag,
                ArpeggiatorInitializerBarZigzag,
                ScaleratorInitializerAscending,
                ScaleratorInitializerDescending,
                ScaleratorInitializerChordZigzag,
                ScaleratorInitializerBarZigzag
            };
        }
        #endregion


        #region PopulateFirstGeneration()
        /// <summary>
        /// Initialize first generation of solution candidates. 
        /// </summary>
        protected virtual void PopulateFirstGeneration()
        {
            MelodyCandidate candidate, reversedCandidate, seedCandidate, reversedSeedCandidate;
            ICollection<MelodyCandidate> offspringCandidates;
            List<MelodyCandidate> crossoverParticipants;

            // initialize basic candidates with generic arpeggeio & scale note sequences 
            foreach (Action<IEnumerable<IBar>> initializer in _initializers)
            {
                // create a new empty candidate melody 
                candidate = new MelodyCandidate(_currentGeneration, ChordProgression);

                // initialize it with current iterated initializer 
                initializer(candidate.Bars);

                // duplicate the newly created candidate 
                reversedCandidate = new MelodyCandidate(_currentGeneration, candidate.Bars, true);

                // reverse the duplicated candidate note 
                ReverseAllNotesMutation(reversedCandidate);

                // add the new candidates to candidate collection 
                _candidates.Add(candidate);
                _candidates.Add(reversedCandidate);
            }


            if (Seed != null)
            {
                // encapsulate the bar collection from the seed in a candidate entity 
                seedCandidate = new MelodyCandidate(_currentGeneration, Seed, includeExistingMelody: true);
                reversedSeedCandidate = new MelodyCandidate(_currentGeneration, Seed, includeExistingMelody: true);

                // define the number of points for the crossover 
                int n = 1; // Seed.Count / 4;

                // initialize a list of offspring candidates that would be returned from the crossover
                List<MelodyCandidate> offSpringCandidatesList = new List<MelodyCandidate>();

                // crossover all the existing candidates with the seed candidate & it's reverse
                foreach (var generatedCandidate in _candidates)
                {
                    // crossover with seed itself 
                    crossoverParticipants = new List<MelodyCandidate> { seedCandidate, generatedCandidate };
                    offspringCandidates = NPointCrossover(crossoverParticipants, n);
                    offSpringCandidatesList.AddRange(offspringCandidates);

                    // crossover with reversed seed  
                    crossoverParticipants = new List<MelodyCandidate> { reversedSeedCandidate, generatedCandidate };
                    offspringCandidates = NPointCrossover(crossoverParticipants, n);
                    offSpringCandidatesList.AddRange(offspringCandidates);
                }
                // add seed and all crossover offsprings to the candidate list 
                _candidates.AddRange(offSpringCandidatesList);
                _candidates.AddRange(new[] { seedCandidate, reversedSeedCandidate });
            }
        }
        #endregion
    }
}
