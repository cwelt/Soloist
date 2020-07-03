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
        float EvaluateAdjacentPitchesDistance(MelodyCandidate candidate, byte maxInterval = 7)
        {
            // initialize counters 
            ulong totalNumOfIntervals = 0;
            ulong numOfExtremeIntervals = 0;
            float adjacentDistance = 0;
            float fitness = 0;

            // filter pitches which are not hold/rest notes
            var pitches = candidate.Bars
                .SelectMany(bar => bar.Notes
                    .Select<INote, NotePitch>(note => note.Pitch)
                    .Where(pitch => pitch != NotePitch.HoldNote &&
                                    pitch != NotePitch.RestNote))
                .ToArray();

            for (int i = 0; i < pitches.Length - 1; i++)
            {
                adjacentDistance = Math.Abs(pitches[i + 1] - pitches[i]);
                if (adjacentDistance > maxInterval)
                    numOfExtremeIntervals++;
            }

            totalNumOfIntervals = (ulong)pitches.Length - 1;

            fitness = (totalNumOfIntervals - numOfExtremeIntervals) / totalNumOfIntervals;

            return fitness;
        }
    }
}
