using System;
using System.Linq;
using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmComposer : Composer
    {
        #region Crossover()
        /// <summary>
        /// Slices and mixes solution candidates and generates new solutions which are the outcome 
        /// offspring of the candidates that participated in the crossover process of slice and mix. 
        /// </summary>
        protected void Crossover()
        {
            // initialize 
            int randomIndex1, randomIndex2, numOfCandidates, numOfCrossoverPoints;
            MelodyCandidate candidate1, candidate2;
            List<MelodyCandidate> crossoverParticipants;
            ICollection<MelodyCandidate> newCandidates;
            Random random = new Random();

            /* crossover about 1/6 participants from the population to create 
             * approxiamtly 1/3 */
            for (int i = 0; i < MaxPopulationSize / 4; i++)
            {
                // fetch number of candidates 
                numOfCandidates = _candidates.Count;

                // select two participating candidates randomly 
                randomIndex1 = random.Next(numOfCandidates / 2);
                randomIndex2 = random.Next(numOfCandidates / 2, numOfCandidates);
                candidate1 = _candidates[randomIndex1];
                candidate2 = _candidates[randomIndex2];

                // pack the crossover participants in a collection  
                crossoverParticipants = new List<MelodyCandidate> { candidate1, candidate2 };

                // select a random number of crossover points between 1 to 3 
                numOfCrossoverPoints = random.Next(1, 4);

                /* do the crossover, 
                 * either with a wise selection of crossover points (80% probability)
                 * or a random selection (20% probability). */
                if (random.NextDouble() > 0.2)
                {
                    newCandidates = NPointCrossover(
                        participants: crossoverParticipants,
                        n: numOfCrossoverPoints,
                        optimizeCrossoverPointsSelection: true);
                }

                else
                {
                    newCandidates = NPointCrossover(
                        crossoverParticipants,
                        numOfCrossoverPoints,
                        optimizeCrossoverPointsSelection: false);
                }

                // add the new born baby solution candidates to the candidate population 
                _candidates.AddRange(newCandidates);
            }
        }
        #endregion

        #region NPointCrossover
        /// <summary>
        /// Implements a crossover between two or more candidate participants in N distinct points.
        /// </summary>
        /// <param name="participants"> Collection of candidates that are intended to paricipate 
        /// as parents in the crossover proccess. </param>
        /// <param name="n"> The number of the crossover points for slicing the candidates. </param>
        /// <param name="optimizeCrossoverPointsSelection"> If set to true, an optimized wised 
        /// selection of the n crossover points is made, by selecting points which reduce the intervals 
        /// between the bars around the crossover points. If set to false, the crossover points 
        /// are selected randomly. </param>
        /// <returns> New candidate solutions which are the offsprings of the participating candidates. 
        /// The participating candidates are not changed or modified during the proccess. </returns>
        protected ICollection<MelodyCandidate> NPointCrossover(
            IList<MelodyCandidate> participants,
            int n,
            bool optimizeCrossoverPointsSelection = true)
        {
            // assure N isn't too big 
            int numberOfBars = participants[0].Bars.Count;
            n = (n < numberOfBars) ? n : numberOfBars - 1;

            // initialization 
            int currentPosition = 0;
            List<IBar> offspring1Bars, offspring2Bars;
            MelodyCandidate parent1, parent2, temp, offspring1, offspring2;
            List<MelodyCandidate> offsprings = new List<MelodyCandidate>(2 * n);

            // generate n uniqe crossover points 
            int[] crossoverPoints;

            // crossover each pair of parent particpants 
            for (int i = 0; i < participants.Count; i++)
            {
                // set first parent 
                parent1 = participants[i];

                // pair him with each other parent participant 
                for (int j = i + 1; j < participants.Count; j++)
                {
                    // set second parent participant
                    parent2 = participants[j];

                    // iniate new empty twin offsprings 
                    offspring1Bars = new List<IBar>(numberOfBars);
                    offspring2Bars = new List<IBar>(numberOfBars);

                    // select n crossover points, either wisely or randomly
                    crossoverPoints = optimizeCrossoverPointsSelection ?
                        SelectOptimizedCrossoverPoints(parent1, parent2, n) :
                        SelectRandomCrossoverPoints(ChordProgression.Count, n);

                    // do the actual crossover 
                    currentPosition = 0;
                    foreach (int crossoverPoint in crossoverPoints)
                    {
                        while (currentPosition < crossoverPoint)
                        {
                            offspring1Bars.Add(MusicTheoryFactory.CreateBar(parent1.Bars[currentPosition]));
                            offspring2Bars.Add(MusicTheoryFactory.CreateBar(parent2.Bars[currentPosition]));
                            currentPosition++;
                        }

                        // swap parents roll for the crossover switch 
                        temp = parent1;
                        parent1 = parent2;
                        parent2 = temp;
                    }

                    // complete filling rest of candidate bars from the other parent
                    for (int k = currentPosition; k < numberOfBars; k++)
                    {
                        offspring1Bars.Add(MusicTheoryFactory.CreateBar(parent1.Bars[k]));
                        offspring2Bars.Add(MusicTheoryFactory.CreateBar(parent2.Bars[k]));
                    }

                    // create two new twin offsprings based on the pre-filled bars from the crossover
                    offspring1 = new MelodyCandidate(_currentGeneration, offspring1Bars, true);
                    offspring2 = new MelodyCandidate(_currentGeneration, offspring2Bars, true);

                    // add the new born offsprings to the result list 
                    offsprings.Add(offspring1);
                    offsprings.Add(offspring2);
                }
            }
            return offsprings;
        }
        #endregion

        #region SelectOptimizedCrossoverPoints
        /// <summary>
        /// Utility method for selecting crossover points in a way that minimizes the interval outcome 
        /// in the transition point after the crossover. 
        /// <para>
        /// A radnomly selected crossover point could cause an unexpected extreme interval between the 
        /// bars of the candidate arund the crosspoint, because the participants might carry melodies
        /// on significantly different pitch ranges. 
        /// By selecting the points wisely, this side-effect is mitigated substantialy.
        /// </para>
        /// </summary>
        /// <param name="parent1"> The first crossover participant. </param>
        /// <param name="parent2"> The second crossover participant. </param>
        /// <param name="n"> The required number of crossover points. </param>
        /// <returns> Array containing n indices of the recommended crossover points. </returns>
        protected int[] SelectOptimizedCrossoverPoints(MelodyCandidate parent1, MelodyCandidate parent2, int n)
        {
            // get all non-empty bars except the first one, and project their indices as well 
            var allNonEmptyBarsButFirstWithBarIndex = parent1.Bars
                .Except(new[] { parent1.Bars.First() })
                .Where(b => b.Notes.Any())
                .Select((bar, barIndex) => new
                {
                    Bar = bar,
                    BarIndex = barIndex + 1,
                });

            /* for each bar, project the interval distance between the last note 
             * from the preceding bar to the first note of the current bar */
            var barIndicesWithTransitionInterval = allNonEmptyBarsButFirstWithBarIndex
                .Select(x => new
                {
                    BarIndex = x.BarIndex,
                    FirstInterval = Math.Abs((int)parent1.Bars[x.BarIndex].Notes
                        .First()?.Pitch - (int)parent2.Bars[x.BarIndex - 1].Notes.Last()?.Pitch),
                    SecondInterval = Math.Abs((int)parent1.Bars[x.BarIndex - 1].Notes
                        .Last()?.Pitch - (int)parent2.Bars[x.BarIndex].Notes.First()?.Pitch)
                });

            // project the indices of the bars with lowest interval outcome 
            var orderedBarIndicesByMinimumInterval = barIndicesWithTransitionInterval
                .OrderBy(x => x.FirstInterval + x.SecondInterval)
                .Take(n)
                .Select(x => x.BarIndex);

            // return the selected indices ordered by index in ascending order 
            return orderedBarIndicesByMinimumInterval
                .OrderBy(crossoverPointIndex => crossoverPointIndex)
                .ToArray();
        }
        #endregion

        #region SelectRandomCrossoverPoints
        /// <summary>
        /// Utility method for selecting n distinct crossover points randomly.
        /// </summary>
        /// <param name="numberOfBars"> The total number of bars in each participant. </param>
        /// <param name="n"> The required number of crossover points. </param>
        /// <returns> Array containing n indices of randomly selected crossover points. </returns>
        protected int[] SelectRandomCrossoverPoints(int numberOfBars, int n)
        {
            // initializiation  
            int randomIndex;
            int[] crossoverPoints;
            Random random = new Random();

            // generate a range of all possible crossover points 
            List<int> possibleCrossoverPoints = Enumerable.Range(1, numberOfBars).ToList();

            // optimization for the case of full zigzag
            if (n == numberOfBars - 1)
                return possibleCrossoverPoints.ToArray();

            // generate n random crossover points 
            crossoverPoints = new int[n];
            for (int i = 0; i < n; i++)
            {
                // select a random point
                randomIndex = random.Next(1, possibleCrossoverPoints.Count);
                crossoverPoints[i] = possibleCrossoverPoints[randomIndex];

                // remove seleted point from possible options to ensure uinque instances 
                possibleCrossoverPoints.RemoveAt(randomIndex);
            }

            // sort crossover points by ascending order 
            return crossoverPoints.OrderBy(num => num).ToArray();
        }
        #endregion
    }
}
