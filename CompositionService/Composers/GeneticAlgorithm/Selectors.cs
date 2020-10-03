using System;
using System.Linq;
using System.Collections.Generic;


namespace CW.Soloist.CompositionService.Composers.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmComposer : Composer
    {

        #region SelectNextGeneration()
        /// <summary>
        /// Selects a partial set of candidate solutions to form the population 
        /// of solution candidates for the next generation.
        /// The selection process is based on the candidate's fitness (score).
        /// </summary>
        protected internal void SelectNextGeneration()
        {
            // delegate selection process to a dedicated selection method
            //RouletteWheelSelection();
            PlusSelection();
        }
        #endregion

        #region PlusSelection()
        /// <summary>
        /// <para>
        /// Filters out the current population of candidates by deterministically selecting only a limited amount 
        /// of candidates from two separate populations: 
        /// current generation candidate's population, and the elder candidate's population. 
        /// </para>
        /// This selection method makes a clear distinction between the candidates of current generation
        /// and all the rest of the candidates, and makes sure that the next generation would contain 
        /// approximately half of the "new born" candidates and half of the elder candidates. 
        /// </summary>
        private protected void PlusSelection()
        {
            // initialization 
            IEnumerable<MelodyCandidate> currentGenerationCandidates;
            IEnumerable<MelodyCandidate> bestCandidatesOfCurrentGeneration;
            IEnumerable<MelodyCandidate> elderGenerationCandidates;
            IEnumerable<MelodyCandidate> bestCandidatesOfElderGenerations;
            List<MelodyCandidate> nextGenerationCandidates;
            int currentGenerationCandidatesCount;
            int elderGenerationCandidatesCount;
            int candidateGroupMaxSize = MaxPopulationSize / 2;

            // get candidate from current generation (new born candidate)
            currentGenerationCandidates = _candidates
                .Where(c => c.Generation == _currentGeneration);

            // get the rest of the candidates (the elder ones - parents, grandparents, etc.)
            elderGenerationCandidates = _candidates.Except(currentGenerationCandidates);

            // sort the groups in descending order by their fitness evaluation
            currentGenerationCandidates = currentGenerationCandidates
                .OrderByDescending(c => c.FitnessGrade);

            elderGenerationCandidates = elderGenerationCandidates
                .OrderByDescending(c => c.FitnessGrade);

            // count number of elements in each group 
            currentGenerationCandidatesCount = currentGenerationCandidates.Count();
            elderGenerationCandidatesCount = elderGenerationCandidates.Count();

            /* select the best candidates from each group into a union 
             * which will become the basis for the next generation population */
            bestCandidatesOfCurrentGeneration = currentGenerationCandidates
                .Take(Math.Min(currentGenerationCandidatesCount / 2, candidateGroupMaxSize));

            bestCandidatesOfElderGenerations = elderGenerationCandidates
                .Take(Math.Min(elderGenerationCandidatesCount / 2, candidateGroupMaxSize));

            nextGenerationCandidates = bestCandidatesOfCurrentGeneration
                .Union(bestCandidatesOfElderGenerations)
                .ToList();

            /* if next generation population size isn't fully occupied, 
             * fill up the available places with the next best candidates */
            if (nextGenerationCandidates.Count < MaxPopulationSize)
            {
                int remainingPlaces = MaxPopulationSize - nextGenerationCandidates.Count;
                List<MelodyCandidate> remainingCandidates = _candidates
                    .Except(nextGenerationCandidates)
                    .OrderByDescending(c => c.FitnessGrade)
                    .ToList();

                while ((remainingPlaces > 0) && (remainingCandidates.Count > 0))
                {
                    nextGenerationCandidates.Add(remainingCandidates[0]);
                    remainingCandidates.RemoveAt(0);
                    remainingPlaces--;
                }
            }

            // replace current population with the selected population 
            _candidates.Clear();
            _candidates.AddRange(nextGenerationCandidates);
        }
        #endregion

        #region RouletteWheelSelection()
        /// <summary>
        /// Filters out the current population of candidates with an un-deterministic algorithm
        /// that simulates a roulette wheel spin, and gives the candidates proportional chances
        /// to be selected according to their proportional fitness.
        /// <para>
        /// This algorithm implements a proportional selection method which calculates a 
        /// proportional fitness for each candidate in relation to the overall fitness sum,
        /// and maps the candidates to a virtual roulette wheel, such that each candidate gets 
        /// a slice that is proportional in size to their fitness. 
        /// </para>
        /// <para>
        /// This selection method has the benefit of giving all candidates a chance to get 
        /// selected (thanks to the random selection), which helps avoiding a search
        /// lockdown on a local optimum, and at the same time gives better candidate a 
        /// better chance.
        /// </para>
        /// and randomly selected a number which is mapped to 
        /// 
        /// </summary>
        private protected void RouletteWheelSelection()
        {

            // data initialization 
            Random randomGenerator = new Random();
            double fitnessSum; // fitness sum of all remainning candidates 
            double randomSpinnedValue; // simulates arrow head after roulette spin 
            double accumulator; // accumulator for tracking slices on roulette
            MelodyCandidate selectedCandidate; // selected candidate on a given spin

            // initialize an empty list of next generation candidates  
            List<MelodyCandidate> nextGenerationCandidates = new List<MelodyCandidate>(MaxPopulationSize);

            // spin the roulette wheel until we have enough candidates for next generation
            while (nextGenerationCandidates.Count < MaxPopulationSize && _candidates.Count > 0)
            {
                // initialize the tracking accumulator 
                accumulator = 0;

                // sum up the total fitness of the entire unselected population 
                fitnessSum = _candidates.Sum(c => c.FitnessGrade);

                /* project the candidates with their proportional fitness 
                 * regarding the total sum of fitness */
                var fitnessesRoulette = _candidates
                    .Select(candidate => new
                    {
                        Candidate = candidate,
                        PropotionalFitness = candidate.FitnessGrade / fitnessSum
                    }).ToArray();

                // simulate the roulette wheel spin for current round 
                randomSpinnedValue = randomGenerator.NextDouble();

                // progress along the roulette wheel untill we pass the arrow head 
                int i;
                for (i = 0; i < fitnessesRoulette.Length && accumulator <= randomSpinnedValue; i++)
                    accumulator += fitnessesRoulette[i].PropotionalFitness;

                // move the selected candidate to the next generation population list 
                selectedCandidate = fitnessesRoulette[i - 1].Candidate;
                nextGenerationCandidates.Add(selectedCandidate);
                _candidates.Remove(selectedCandidate);
            }

            // replace current population with the next generation selected population 
            _candidates.Clear();
            _candidates.AddRange(nextGenerationCandidates);
        }
        #endregion
    }
}
