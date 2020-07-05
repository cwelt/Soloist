using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmCompositor : Compositor
    {

        #region SelectNextGeneration()
        /// <summary>
        /// <para>
        /// Selects a partial set of candidate solutions for the next generation.
        /// </para>
        /// The selection process is based on the the candidte's fitness (score).
        /// </summary>
        protected internal void SelectNextGeneration()
        {
            PlusSelection();
        }
        #endregion

        /// <summary>
        /// <para>
        /// Filters out the current population of candidates by determinsticly selecting only a limited amount 
        /// of candidates from two separate populations: 
        /// current generation candidates population, and the elder candidates population. 
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
            int maxPopulationSize = MaxNumberOfCandidates / 2;

            // get candidate from current generation 
            currentGenerationCandidates = _candidates
                .Where(c => c.Generation == _currentGeneration);

            // get the rest of the candidates (the elder ones)
            elderGenerationCandidates = _candidates.Except(currentGenerationCandidates);

            // sort the populations in descending order by their fitness 
            currentGenerationCandidates.OrderByDescending(c => c.FitnessGrade);
            elderGenerationCandidates.OrderByDescending(c => c.FitnessGrade);

            // count number of elements in each population 
            currentGenerationCandidatesCount = currentGenerationCandidates.Count();
            elderGenerationCandidatesCount = elderGenerationCandidates.Count();


            /* select the best candidates from each population into a union 
             * which will become the basis for the next generation population */
            bestCandidatesOfCurrentGeneration = currentGenerationCandidates
                .Take(Math.Min(currentGenerationCandidatesCount / 2, maxPopulationSize));

            bestCandidatesOfElderGenerations = elderGenerationCandidates
                .Take(Math.Min(elderGenerationCandidatesCount / 2, maxPopulationSize));

            nextGenerationCandidates = bestCandidatesOfCurrentGeneration
                .Union(bestCandidatesOfElderGenerations)
                .ToList();

            /* if next generation population size isn't fully occupied, 
             * fill up the available places with the next best candidates */
            if (nextGenerationCandidates.Count < maxPopulationSize)
            {
                int remainingPlaces = maxPopulationSize - nextGenerationCandidates.Count;
                List<MelodyCandidate> remainingCandidates = _candidates
                    .Except(bestCandidatesOfCurrentGeneration)
                    .Except(bestCandidatesOfElderGenerations)
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
    }
}
