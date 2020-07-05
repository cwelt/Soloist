using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
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
        private protected const int MaxNumberOfCandidates = 60;
        private protected Action<MelodyCandidate, int?>[] _mutations;
        private protected Action<IEnumerable<IBar>>[] _initializers;

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        public GeneticAlgorithmCompositor()
        {
            _mutations = new Action<MelodyCandidate, int?>[10];

            // register initializers for first generation
            _initializers = new Action<IEnumerable<IBar>>[] {
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

        #region InitializeCompositionParams()
        private protected override void InitializeCompositionParams(IList<IBar> chordProgression, IList<IBar> melodyInitializationSeed = null, OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium, NotePitch minPitch = NotePitch.E2, NotePitch maxPitch = NotePitch.E6)
        {
            base.InitializeCompositionParams(chordProgression, melodyInitializationSeed, overallNoteDurationFeel, minPitch, maxPitch);
            _currentGeneration = 0;
            _candidates = new List<MelodyCandidate>(120);
        }
        #endregion

        #region PopulateFirstGeneration()
        /// <summary>
        /// Initialize first generation of solution candidates. 
        /// </summary>
        protected internal void PopulateFirstGeneration()
        {
            MelodyCandidate candidate, seedCandidate, reversedCandidate;

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

                // define the number of points for the crossover 
                int n = Seed.Count / 3;

                // initialize a list of offspring candidates that would be returned from the crossover
                List<MelodyCandidate> offSpringCandidatesList = new List<MelodyCandidate>();

                // crossover all the existing candidates with the seed candidate 
                foreach (var generatedCandidate in _candidates)
                {
                    var list = new List<MelodyCandidate> { seedCandidate, generatedCandidate };
                    ICollection<MelodyCandidate> offspringCandidates = NPointCrossover(list, n);
                    offSpringCandidatesList.AddRange(offspringCandidates);
                }

                // replace old candidates (parents) with their mixed offsprings 
                _candidates.Clear();
                _candidates.AddRange(offSpringCandidatesList);
            }
        }
        #endregion


        #region GenerateMelody()
        /// <inheritdoc/>
        private protected override IList<IBar> GenerateMelody()
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
                foreach (var candidate in _candidates)
                {
                    Mutate(candidate);
                }
                
                // rate each individual 
                EvaluateFitness();

                // natural selection 
                SelectNextGeneration();

                //MelodyGenome.CurrentGeneration++;
                if (++i == 13)
                    terminateCondition = true;
            }

            // TODO: convert internal genome representation of each candidate in to a MIDI track chunk representation
            int randomIndex = new Random().Next(_candidates.Count);
            return _candidates[randomIndex].Bars;
        }
        #endregion


        

       

        #region Mutate()
        /// <summary>
        /// Alter a candidate's solution state.
        /// </summary>
        protected internal void Mutate(MelodyCandidate candidate)
        {
            Random random = new Random();

            for (int i = 0; i < candidate.Bars.Count; i++)
            {
                switch(random.Next(17))
                {
                    case 0:
                        SyncopedNoteMutation(candidate);
                        break;
                    case 1:
                        PermutateNotes(candidate.Bars[i], permutation: Permutation.Shuffled);
                        break;
                    case 2:
                        ChordPitchMutation(candidate, i);
                        break;
                    case 3:
                        DurationEqualSplitMutation(candidate, i);
                        break;
                    case 4:
                        PermutateNotes(candidate.Bars[i], permutation: Permutation.Reversed);
                        break;
                    case 5:
                        ToggleFromHoldNoteMutation(candidate);
                        break;
                    case 6:
                        ScalePitchMutation(candidate, i);
                        break;
                    case 7:
                        PermutateNotes(candidate.Bars[i], permutation: Permutation.Shuffled);
                        break;
                    case 8:
                        DurationDelaySplitMutation(candidate, i);
                        break;
                    case 9:
                        PermutateNotes(candidate.Bars[i], permutation: Permutation.SortedDescending);
                        break;
                    case 10:
                        DurationAnticipationSplitMutation(candidate, i);
                        break;
                    case 11:
                        DurationSplitOfARandomNote(candidate.Bars[i], DurationSplitRatio.Delay);
                        break;
                    case 12:
                        ReverseBarNotesMutation(candidate, i);
                        break;
                    case 13:
                        ReverseChordNotesMutation(candidate, i);
                        break;
                    case 14:
                        ToggleToHoldNoteMutation(candidate, i);
                        break;
                }
            }

        }
        #endregion

        #region EvaluateFitness()
        /// <summary>
        /// Evaluates the candidate solution's fitness, i.e., grades (score).
        /// </summary>        
        protected internal void EvaluateFitness()
        {
            foreach (MelodyCandidate candidate in _candidates)
            {
                float adjacentPitchesGrade = EvaluateAdjacentIntervals(candidate);
                candidate.FitnessGrade = adjacentPitchesGrade * 100;
            }
        }
        #endregion


    }
}
