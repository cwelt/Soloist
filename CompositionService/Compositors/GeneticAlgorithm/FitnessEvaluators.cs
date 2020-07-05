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
        double EvaluateAdjacentIntervals(MelodyCandidate candidate, PitchInterval maxInterval = PitchInterval.PerfectFifth)
        {
            // initialize counters 
            ulong totalNumOfIntervals = 0;
            ulong numOfDiatonicSteps = 0;
            ulong numOfChordSteps = 0;
            ulong numOfDissonants = 0;
            ulong numOfPerfectConsonants = 0;
            ulong numOfImperfectConsonants = 0;
            ulong numOfTritones = 0;
            ulong numOfBigLeaps = 0;
            ulong numOfExtremeIntervals = 0;
            int adjacentDistance = 0;

            // initialize metrics 
            double bigLeapIntervalMetric;
            double extremeIntervalMetric;
            double diatonicStepsMetric;
            double chordStepsMetric;
            double intervalTypeMetric;


            double fitness = 0;
            PitchInterval interval;
            int maxDistance = (int)maxInterval;

            // retrieve and filter all pitches which are not hold/rest notes
            NotePitch[] pitches = candidate.Bars.GetAllPitches().ToArray();

            // accumulate extreme adjacent notes pitch distance 
            for (int i = 0; i < pitches.Length - 1; i++)
            {
                // calculate pitch distance between the two adjacent notes 
                adjacentDistance = Math.Abs(pitches[i + 1] - pitches[i]);

                // update extreme interval counter if necessary 
                if (adjacentDistance > maxDistance)
                    numOfExtremeIntervals++;

                // update relevant counters according to step size 
                Enum.TryParse(adjacentDistance.ToString(), out interval);
                switch (interval)
                {
                    case PitchInterval.Unison:
                        numOfPerfectConsonants++;
                        break;
                    case PitchInterval.MinorSecond:
                    case PitchInterval.MajorSecond:
                        numOfDissonants++;
                        numOfDiatonicSteps++;

                        break;
                    case PitchInterval.MinorThird:
                    case PitchInterval.MajorThird:
                        numOfChordSteps++;
                        numOfImperfectConsonants++;
                        break;
                    case PitchInterval.PerfectFourth:
                        numOfPerfectConsonants++;
                        break;
                    case PitchInterval.Tritone:
                        numOfTritones++;
                        break;
                    case PitchInterval.PerfectFifth:
                        numOfChordSteps++;
                        numOfPerfectConsonants++;
                        break;
                    case PitchInterval.MinorSixth:
                    case PitchInterval.MajorSixth:
                        numOfImperfectConsonants++;
                        break;
                    case PitchInterval.MinorSeventh:
                    case PitchInterval.MajorSeventh:
                        numOfDissonants++;
                        break;
                    case PitchInterval.Octave:
                        numOfChordSteps++;
                        numOfPerfectConsonants++;
                        break;
                    default:
                        numOfBigLeaps++;
                        break;
                }
            }

            // set total number of intervals 
            totalNumOfIntervals = (ulong)pitches.Length - 1;

            // calculate big leap interval ratio metric 
            bigLeapIntervalMetric = (totalNumOfIntervals - numOfExtremeIntervals) / (float)totalNumOfIntervals;

            // calculate diatonic steps ratio metric
            diatonicStepsMetric = numOfDiatonicSteps / totalNumOfIntervals;

            // calculate chord steps ratio metric
            chordStepsMetric = numOfChordSteps / totalNumOfIntervals;

            // calculate chord steps ratio metric with weighted sum according to interval type
            intervalTypeMetric =
                ((numOfPerfectConsonants * 1.0) +
                (numOfImperfectConsonants * 0.8) +
                (numOfDissonants * 0.6) +
                (numOfTritones * 0.4) +
                (numOfBigLeaps * 0.2)) / totalNumOfIntervals;

            // calculate total weighted fitness according to the different metrics
            fitness = (0.40 * diatonicStepsMetric) +
                      (0.30 * chordStepsMetric) +
                      (0.20 * bigLeapIntervalMetric) +
                      (0.10 * intervalTypeMetric);

            // return fitness result
            return fitness;
        }
    }
}
