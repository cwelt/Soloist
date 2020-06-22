using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmCompositor : Compositor
    {
        ICollection<MelodyCandidate> NPointCrossover(IList<MelodyCandidate> participants, int n)
        {
            // assure N isn't too big 
            int numberOfBars = participants[0].Bars.Count;
            n = (n < numberOfBars) ? n : numberOfBars - 1;

            // initialization 
            int currentPosition = 0;
            Random random = new Random();
            List<IBar> offspring1Bars, offspring2Bars;
            MelodyCandidate parent1, parent2, temp, offspring1, offspring2;
            List<MelodyCandidate> offsprings = new List<MelodyCandidate>(2 * n);

            // generate n uniqe crossover points 
            List<int> possibleCrossoverPoints = Enumerable.Range(1, numberOfBars).ToList();
            int[] crossoverPoints;
            int randomIndex;
            if (n == numberOfBars - 1) // optimization for the case of full zigzag
                crossoverPoints = possibleCrossoverPoints.ToArray();
            else
            {
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
                crossoverPoints = crossoverPoints.OrderBy(num => num).ToArray();
            }

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


                    // do the actual crossover 
                    currentPosition = 0;
                    foreach (int crossoverPoint in crossoverPoints)
                    {
                        while (currentPosition < crossoverPoint)
                        {
                            offspring1Bars.Add(new Bar(parent1.Bars[currentPosition]));
                            offspring2Bars.Add(new Bar(parent2.Bars[currentPosition]));
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
                        offspring1Bars.Add(new Bar(parent1.Bars[k]));
                        offspring2Bars.Add(new Bar(parent2.Bars[k]));
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
    }
}
