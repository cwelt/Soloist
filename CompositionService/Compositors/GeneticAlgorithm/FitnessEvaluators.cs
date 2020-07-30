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
        #region EvaluateFitness()
        /// <summary>
        /// Evaluates the candidate solution's fitness, i.e., grades (score).
        /// </summary>        
        protected internal void EvaluateFitness()
        {
            /* evaluate fitness for all "dirty" candidates, i.e., either new 
             * candidates from current generation which have not been evaluated yet,
             * or candidates that changed since last evaluation */
            foreach (MelodyCandidate candidate in _candidates.Where(c => c.IsDirty))
            {
                double adjacentPitchesGrade = EvaluateSmoothMovement(candidate);
                double extremeIntervalsGrade = EvaluateExtremeIntervals(candidate);
                double pitchVarietyGrade = EvaluatePitchVariety(candidate);
                double pitchRangeGrade = EvaluatePitchRange(candidate);
                double contourDirectionGrade = EvaluateContourDirection(candidate);
                double contourStabilityGrade = EvaluateContourStability(candidate);
                double syncopationsGrade = EvaluateSyncopation(candidate);
                double densityBalanceGrade = EvaluateDensityBalance(candidate);
                double accentedBeatsGrade = EvaluateAccentedBeats(candidate);

                candidate.FitnessGrade = Math.Round(digits: 7, value:
                    (EvaluatorsWeights.ExtremeIntervals * extremeIntervalsGrade) +
                    (EvaluatorsWeights.SmoothMovement * adjacentPitchesGrade) +
                    (EvaluatorsWeights.PitchVariety * pitchVarietyGrade) +
                    (EvaluatorsWeights.PitchRange * pitchRangeGrade) +
                    (EvaluatorsWeights.ContourDirection * contourDirectionGrade) +
                    (EvaluatorsWeights.ContourStability * contourStabilityGrade) +
                    (EvaluatorsWeights.Syncopation * syncopationsGrade) +
                    (EvaluatorsWeights.DensityBalance * densityBalanceGrade) + 
                    (EvaluatorsWeights.AccentedBeats * accentedBeatsGrade));
            }

            // clean IsDirty flag from all candidates 
            foreach (MelodyCandidate candidate in _candidates)
                candidate.IsDirty = false;
        }
        #endregion

        #region EvaluateSmoothMovement UPDATE DOCUMENTATION


        /// <summary>
        /// TODO: UPDATE DOCUMENTATION
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateSmoothMovement(MelodyCandidate candidate, PitchInterval maxInterval = PitchInterval.PerfectFifth)
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
            int adjacentDistance = 0;

            // initialize metrics 
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

            // calculate diatonic steps ratio metric
            diatonicStepsMetric = numOfDiatonicSteps / totalNumOfIntervals;

            // calculate chord steps ratio metric
            chordStepsMetric = numOfChordSteps / totalNumOfIntervals;

            // calculate chord steps ratio metric with weighted sum according to interval type
            intervalTypeMetric =
                ((numOfPerfectConsonants * 1.0) +
                (numOfImperfectConsonants * 0.8) +
                (numOfDissonants * 0.6) +
                (numOfTritones * 0.3) +
                (numOfBigLeaps * 0.1)) / totalNumOfIntervals;

            // calculate total weighted fitness according to the different metrics
            fitness = (0.50 * diatonicStepsMetric) +
                      (0.30 * chordStepsMetric) +
                      (0.20 * intervalTypeMetric);

            // return fitness result
            return fitness;
        }
        #endregion

        #region EvaluateExtremeIntervals()
        /// <summary> 
        /// Evaluates the ratio of extreme pitch intervals.
        /// <para> This evaluation checks the pitch interval between every two consecutive
        /// notes and check if the interval is considered extreme or not, in regard to the 
        /// <paramref name="maxInterval"/> parameter: If the interval is higher than the 
        /// max interval parameter, the distance between the notes would be considered as
        /// an extreme distance. </para>
        /// <para> Besides counting the amount of existing extreme intervals, 
        /// this evaluation takes into account the extreme level, i.e., how large in 
        /// terms of semitones the interval is, so a two octave interval is considered twice 
        /// as extreme than a one octave interval. </para>
        /// <para> The overall fitness is a weighted sum of the ratio between the non-extreme 
        /// intervals to the total number of consecutive notes intervals, and the invert ratio
        /// of total semitone distance in the entire melody and the max pitch range. </para>
        /// </summary>
        /// <param name="maxInterval"> Max interval that is considered "okay" between two 
        /// consecutive notes. Any interval that exceeds this one would be considered extreme.
        /// </param>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateExtremeIntervals(MelodyCandidate candidate, PitchInterval maxInterval = PitchInterval.PerfectFifth)
        {
            // initialize counters & accumuators  
            ulong totalNumOfIntervals = 0;
            ulong numOfExtremeIntervals = 0;
            ulong overallDeviation = 0;
            int adjacentDistance = 0;

            // init max pitch range 
            int pitchRange = MaxPitch - MinPitch;

            // initialize metrics 
            double extremeIntervalMetric;
            double overallDeviationMetric;

            // convert max interval nullable int to int
            int maxDistance = (int)maxInterval;

            // retrieve and filter all pitches which are not hold/rest notes
            NotePitch[] pitches = candidate.Bars.GetAllPitches().ToArray();

            // accumulate extreme adjacent notes pitch distance 
            for (int i = 0; i < pitches.Length - 1; i++)
            {
                // calculate pitch distance between the two adjacent notes 
                adjacentDistance = Math.Abs(pitches[i + 1] - pitches[i]);

                // if this pitch is extreme, update counter & deviation accumulator 
                if (adjacentDistance > maxDistance)
                {
                    numOfExtremeIntervals++;
                    overallDeviation += (ulong)(adjacentDistance - maxDistance);
                }

            }

            // set total number of intervals 
            totalNumOfIntervals = (ulong)pitches.Length - 1;

            // calculate ratio of extreme intervals which exceed the requested max interval
            extremeIntervalMetric = (totalNumOfIntervals - numOfExtremeIntervals) / (float)totalNumOfIntervals;

            // calculate metric of overal deviation from max interval  
            overallDeviationMetric = (numOfExtremeIntervals == 0) ? 1
                : numOfExtremeIntervals / overallDeviation;

            // return weighted fitness according to the two different metrics
            return (0.4 * extremeIntervalMetric) + (0.6 * overallDeviationMetric);
        }
        #endregion

        #region EvaluatePitchVariety()
        /// <summary>
        /// Evaluates fitness according to the variety of distinct pitches in use.
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluatePitchVariety(MelodyCandidate candidate)
        {
            // initialization 
            int numOfTotalPitches = 0;
            int numOfDistinctPitches = 0;
            double distinctPitchesRatio = 0;
            double barFitness, totalFitness = 0;
            IEnumerable<NotePitch> barPitches;

            // caluclate fitness for the individual bars 
            foreach (IBar bar in candidate.Bars)
            {
                // get pure pitches for current bar (discard hold & rest notes)
                barPitches = bar.GetAllPitches();

                // count number of overall pitches and distinct pitches 
                numOfTotalPitches = barPitches.Count();
                numOfDistinctPitches = barPitches.Distinct().Count();

                // assure there is at least one pitch in current bar 
                if (numOfTotalPitches == 0)
                    continue;

                // calculate ratio between num of distinct pitches to overall num of pitches
                distinctPitchesRatio = ((double)(numOfDistinctPitches)) / numOfTotalPitches;

                // set current bar fitness proportionally to number of bars 
                barFitness = distinctPitchesRatio / candidate.Bars.Count;

                // update total fitness accumulator 
                totalFitness += barFitness;
            }

            // return accumulated fitness 
            return totalFitness;
        }
        #endregion

        #region EvaluatePitchRange()
        /// <summary>
        /// Evaluates fitness according to the melody's pitch range. 
        /// This fitness is calculated as the ration between the user's requested pitch range, 
        /// and the actual candidate's melody pitch range.
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluatePitchRange(MelodyCandidate candidate)
        {
            // get user preference for pitch range 
            int requestedRange = MaxPitch - MinPitch;

            // calculate the actual pitch range in this candidate's melody 
            IEnumerable<NotePitch> allPitches = candidate.Bars.GetAllPitches();

            NotePitch actualMaxPitch = allPitches.Max(p => p);
            NotePitch actualMinPitch = allPitches.Min(p => p);

            int actualRange = actualMaxPitch - actualMinPitch;

            // return fitness as ration between the actual range and requested range 
            return (double)actualRange / requestedRange;
        }
        #endregion

        #region EvaluateContourDirection()
        /// <summary>
        /// Evaluates fitness according to the melody's contour direction.
        /// <para> 
        /// Melodies which tend to have more directional flow, i.e., 
        /// sequences of ascending and descending notes, would generally score higher. 
        /// </para>
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateContourDirection(MelodyCandidate candidate)
        {
            // initialization 
            int numberOfUpsInBar = 0;
            int numberOfDownsInBar = 0;
            int totalNumberOfUps = 0;
            int totalNumberOfDowns = 0;
            int totalNumberOfIntervals = 0;

            NotePitch prevPitch, currentPitch;
            NotePitch[] barPitches;

            double barFitness = 0;
            double totalBarsFitness = 0;
            double overallFitness = 0;
            double weightedFitness = 0;

            int barContourDirectionRadius = 0;
            int overallContourDirectionRadius = 0;

            // calculate micro fitness in bar level 
            foreach (var bar in candidate.Bars)
            {
                // initialization 
                barFitness = 0;
                numberOfUpsInBar = 0;
                numberOfDownsInBar = 0;

                // get pure pitches for current bar (discard hold & rest notes)
                barPitches = bar.GetAllPitches().ToArray();

                // assure there is at least one pitch in current bar 
                if (barPitches.Length <= 1)
                    continue;

                // count number of ups in downs in bar 
                for (int i = 1; i < barPitches.Length; i++)
                {
                    prevPitch = barPitches[i - 1];
                    currentPitch = barPitches[i];
                    if (currentPitch - prevPitch >= 0)
                        numberOfUpsInBar++;
                    else numberOfDownsInBar++;
                }

                // calculate direction radius and fitness of current bar 
                barContourDirectionRadius = Math.Abs(numberOfUpsInBar - numberOfDownsInBar);
                barFitness = (double)barContourDirectionRadius / (barPitches.Length - 1);

                // update the total bar fitness 
                totalBarsFitness += barFitness / candidate.Bars.Count;

                // update accumulator counters 
                totalNumberOfUps += numberOfUpsInBar;
                totalNumberOfDowns += numberOfDownsInBar;
                totalNumberOfIntervals += barPitches.Length - 1;
            }

            // calculate macro fitness in candidate level 
            overallContourDirectionRadius = Math.Abs(totalNumberOfUps - totalNumberOfDowns);
            overallFitness = (double)overallContourDirectionRadius / totalNumberOfIntervals;

            // return a weighted fitness combined from micro & macro fitnesses 
            weightedFitness = (0.75 * totalBarsFitness) + (0.25 * overallFitness);
            return weightedFitness;
        }
        #endregion

        #region EvaluateContourStability()
        /// <summary>
        /// Evaluates fitness according to the melody's contour direction stability.
        /// <para> 
        /// Melodies which tend to have more directional flow consecutively, i.e., 
        /// sequences of ascending and descending notes ine after the other,
        /// would generally score higher. 
        /// </para>
        /// <para>
        /// This evaluation differs from <see cref="EvaluateContourDirection(MelodyCandidate)"/>,
        /// by evaluating consecutive sequences of directional intervals, assuring the ups and 
        /// downs are not randomly distributed, but rather stable consistent. 
        /// For current implementation, only directional sequences of 3 notes or more 
        /// are taken into account for positive evaluation.
        /// </para>
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateContourStability(MelodyCandidate candidate)
        {
            // init sign holders of previous and current interval direction (up/down)
            int currentIntervalSign = 1;
            int prevDirectionSign = 1;

            // init counters for sequences of consecutive notes in the same direction 
            int directionalSequenceCounter = 0;
            int directionalIntervalAccumulator = 0;

            // init individual pitch placeholders and get sequence of all pitches 
            NotePitch prevPitch, currentPitch;
            NotePitch[] pitches = candidate.Bars.GetAllPitches().ToArray();

            // assure there is at least one interval (at least two pitches)
            if (pitches.Length <= 1)
                return 0;

            // initially set the previous sign to the first one 
            if (pitches[1] - pitches[0] >= 0)
                prevDirectionSign = 1; // ascending direction  
            else prevDirectionSign = -1; // descending direction 

            // scan all intervals 
            for (int i = 1; i < pitches.Length; i++)
            {
                // update previous and current pitches 
                prevPitch = pitches[i - 1];
                currentPitch = pitches[i];

                // calculate the current interval direction (up/down)
                currentIntervalSign = currentPitch - prevPitch;

                // if the direction hasn't changed, just update sequence counter  
                if (currentIntervalSign * prevDirectionSign >= 0)
                {
                    directionalSequenceCounter++;
                }
                else // direction has changed 
                {
                    // if last sequence was long enough, add it to accumulator
                    if (directionalSequenceCounter >= 3)
                        directionalIntervalAccumulator += directionalSequenceCounter;

                    // reset directional sequence counter 
                    directionalSequenceCounter = 1;

                    // reset direction according to last interval 
                    prevDirectionSign *= -1;
                }
            }

            /* return ratio between the total number of consecutive directional intervals 
             * and the overall number of intervals in the candidate's melody */
            return (double)directionalIntervalAccumulator / (pitches.Length - 1);
        }
        #endregion

        #region EvaluateSyncopation()
        /// <summary>
        /// Evaluates fitness according to the amount of existing syncopations. 
        /// This fitness is calculated as the ratio between the amount of existing syncopations
        /// in the melody, and the total amount of real pitched notes, i.e., not hold and rest
        /// notes. 
        /// <para>
        /// This evaluation considers a note to be a syncopation if it a pitch note,
        /// it starts on an "off-beat" (not on beginning or middle of bar), 
        /// and it's length is a quarter beat length or longer.
        /// </para>
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateSyncopation(MelodyCandidate candidate)
        {
            // initialization 
            IBar bar;
            INote note;
            float barDuration, noteDuration;
            float noteStartTime = 0;
            uint syncopationCounter = 0;

            // scan all bars of the subject candidate 
            for (int i = 0; i < candidate.Bars.Count; i++)
            {
                // get current bar's duration  
                bar = candidate.Bars[i];
                barDuration = (float)bar.TimeSignature.Numerator / bar.TimeSignature.Denominator;

                // reset note start time in relation to it's containing bar 
                noteStartTime = 0;

                // scan all notes in current bar 
                for (int j = 0; j < bar.Notes.Count; j++)
                {
                    // fetch current note 
                    note = bar.Notes[j];

                    // get the duration note in context of the melody it resides in 
                    noteDuration = note.GetDurationInContext(bar, candidate.Bars, j, i);

                    /* consider a note to be syncoped if it is a quarter beat or longer,
                     * it does not start on an offbeat of the bar, 
                     * and it is not a hold note or a rest note*/
                    if (noteDuration >= Duration.QuaterNoteFraction &&
                        bar.IsOffBeatNote(noteStartTime, barDuration) &&
                        note.Pitch != NotePitch.RestNote &&
                        note.Pitch != NotePitch.HoldNote)
                        syncopationCounter++;
                }
            }

            // return fitness as ratio between the number of syncopes and total "real" notes
            return (float)syncopationCounter / candidate.Bars
                .SelectMany(b => b.Notes)
                .Where(n => n.Pitch != NotePitch.RestNote &&
                            n.Pitch != NotePitch.HoldNote)
                .Count();
        }
        #endregion

        #region EvaluateDensityBalance()
        /// <summary>
        /// Evaluates fitness according to the note density balance across the melodie's bars. 
        /// <para> This fitness function objective is to assure the amount of notes in each bar 
        /// are more or less balanced, and mitigate obscure sounding phrases of which  one bar
        /// is very dense and another is very sparse, which in general leads to an un pleasant 
        /// drastic change in feel.  
        /// </para>
        /// <para> Inorder to acheive this objective, the evaluation calculates the 
        /// average of notes in each bar and the standard deviation, and gives a high score
        /// to candidate of which their average and standard deviation are close.</para>
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateDensityBalance(MelodyCandidate candidate)
        {
            // calculate averge number of notes in a bar 
            int numOfNotesInBarAverage = (int)Math.Round(candidate.Bars.Average(b => b.Notes.Count));

            // calculate the bar set standard deviation from the average 
            double mutualStandardDeviation = Math.Sqrt(candidate.Bars
                .Select(b => Math.Pow((numOfNotesInBarAverage - b.Notes.Count), 2))
                .Average());

            // return inversed ratio of distance of standard deviarion from average 
            return 1 - (mutualStandardDeviation / numOfNotesInBarAverage);
        }
        #endregion

        #region EvaluateAccentedBeats()
        /// <summary> 
        /// Evaluates accented beats in each bar, in terms of the accented pitches and their 
        /// preceding notes, regarding the transition they create towards the accented beats.
        /// <para> In general, accented beats (for example beats 1 and 3 on a 4/4 bar) sound 
        /// specially well when the note played on them is one of the underlying chord notes. </para>
        /// <para> Moreover, when the preceding note creats a tension which is solved under the 
        /// accented beat (for example a transition of half a tone, or a perfect fifth when there is 
        /// a tritone interval in the background), the accented note sounds so much better. </para>
        /// <para> This fitness function objective is to find such good accented beats with good
        /// leading preceding notes and award this candidate accordingly, in relation to the overall 
        /// amount of strong accented notes and their leading transitions. </para>
        /// </summary>
        /// <param name="candidate"> The melody candidate to evaluate. </param>
        /// <returns> The fitness outcome score for the requested evaluation. </returns>
        private protected double EvaluateAccentedBeats(MelodyCandidate candidate)
        {
            // initialization 
            IBar bar;
            IChord chord;
            int noteIndex = 0;
            double weightedSum;
            INote note, precedingNote;
            IList<int> chordNotesIndices;
            IEnumerable<NotePitch> chordMappedPitches;

            // total accented beats (beats that are not off-beats)
            int strongBeatsCounter = 0;

            // interval between a strong beat note & it's preceding note 
            int transitionDistance = 0;

            // chord notes that fall on strong beats 
            int goodPrecedingNotesCounter = 0;

            // offbeats that lead to a strong beat via a minor/major second, or perfect 4th/5th
            int goodNotesOnStrongBeatsCounter = 0; 

            // start iterating all bars 
            for (int barIndex = 0; barIndex < candidate.Bars.Count; barIndex++)
            {
                // fetch next bar 
                bar = candidate.Bars[barIndex];

                // iterate over all chords in bar 
                for (int chordIndex = 0; chordIndex < bar.Chords.Count; chordIndex++)
                {
                    // fetch next chord 
                    chord = bar.Chords[chordIndex];

                    // get the good sounding pitches under this chord 
                    chordMappedPitches = chord.GetArpeggioNotes(MinPitch, MaxPitch);

                    // get the actual notes played under this chord 
                    bar.GetOverlappingNotesForChord(chordIndex, out chordNotesIndices);

                    // check if the actual notes are good 
                    for (int i = 0; i < chordNotesIndices.Count; i++)
                    {
                        // fetch next note under the current chord 
                        noteIndex = chordNotesIndices[i];
                        note = bar.Notes[noteIndex];

                        // the test is relevant only if this is an accented beat (NOT an off-beat)
                        if (!note.IsOffBeatNote(bar))
                        {
                           // update the total amount of strong accented beats 
                            strongBeatsCounter++;

                            // bonus the candidate if current pitch sounds good under current chord 
                            if (chordMappedPitches.Contains(note.Pitch))
                                goodNotesOnStrongBeatsCounter++;

                            // fetch preceding note to if it owns a bonus for a good transition
                            precedingNote = candidate.Bars.GetPredecessorNote(true, barIndex, noteIndex, out int prevBarIndex, out int prevNoteIndex);

                            // assure a preceding note has been found 
                            if (precedingNote != null)
                            {
                                // calculate interval distance between current note and it's predecessor
                                transitionDistance = Math.Abs((byte)note.Pitch - (byte)precedingNote.Pitch);

                                // bonus if this a dominant or smooth transition towards the strong beat note
                                if (transitionDistance <= (byte)PitchInterval.MajorSecond 
                                    || (transitionDistance == (byte)PitchInterval.PerfectFourth && (byte)precedingNote.Pitch < (byte)note.Pitch)
                                    || (transitionDistance == (byte)PitchInterval.PerfectFifth && (byte)precedingNote.Pitch > (byte)note.Pitch))
                                    goodPrecedingNotesCounter++;
                            }
                        }
                    }
                }
            }

            // calculate weighted sum of bonuses for the good strong beats and their preceding notes 
            weightedSum = (goodNotesOnStrongBeatsCounter * 0.5) + (goodPrecedingNotesCounter * 0.5);

            // return evaluation grade as ratio between the total awarded bonus and overall strong beats
            return weightedSum / strongBeatsCounter;
        }
        #endregion
    }

    #region MelodyEvaluatorsWeights
    public class MelodyEvaluatorsWeights
    {
        public double SmoothMovement { get; set; } = 20;
        public double ExtremeIntervals { get; set; } = 25;
        public double PitchVariety { get; set; } = 15;
        public double PitchRange { get; set; } = 15;
        public double ContourDirection { get; set; } = 5;
        public double ContourStability { get; set; } = 15;
        public double Syncopation { get; set; } = 20; // 40
        public double DensityBalance { get; set; } = 15;
        public double AccentedBeats { get; set; } = 25; // 50 
    }
    #endregion
}
