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
        /// <summary>
        /// <para>Evaluates pitch distances between adjacent notes.</para>
        /// This evaluation checks the pitch interval between every two consecutive
        /// notes and check if the interval is considered extreme or not, in regard to the 
        /// <paramref name="maxInterval"/> parameter: If the interval is higher than the 
        /// max interval parameter, the distance between the notes would be considered as
        /// an extreme distance. The overall fitness is the ratio between the non-extreme 
        /// intervals to the total number of consecutive notes intervals. 
        /// </summary>
        /// <param name="candidate"> Melody candidate for evaluation. </param>
        /// <param name="maxInterval"> Max interval that is considered "okay" between two 
        /// consecutive notes. Any interval that exceeds this one would be considered extreme.
        /// </param>
        /// <returns></returns>
        float EvaluateAdjacentPitchesInterval(MelodyCandidate candidate, byte maxInterval = 7)
        {
            // initialize counters 
            ulong totalNumOfIntervals = 0;
            ulong numOfExtremeIntervals = 0;
            float adjacentDistance = 0;
            float fitness = 0;

            // retrieve and filter all pitches which are not hold/rest notes
            NotePitch[] pitches = candidate.Bars.GetAllPitches().ToArray();

            // accumulate extreme adjacent notes pitch distance 
            for (int i = 0; i < pitches.Length - 1; i++)
            {
                // calculate pitch distance between the two adjacent notes 
                adjacentDistance = Math.Abs(pitches[i + 1] - pitches[i]);

                // update accumulator if interval exceeds the max interval parameter
                if (adjacentDistance > maxInterval)
                    numOfExtremeIntervals++;
            }

            // set fitness for the extreme interval ratio 
            totalNumOfIntervals = (ulong)pitches.Length - 1;
            fitness = (totalNumOfIntervals - numOfExtremeIntervals) / totalNumOfIntervals;

            // return result
            return fitness;
        }
    }
}
