using CW.Soloist.CompositionService.CompositionStrategies.UtilEnums;
using CW.Soloist.CompositionService.MusicTheory;
using Melanchall.DryWetMidi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    /// <summary> <para> Compose a solo-melody over a given playback by implementing a genetic algorithm. </para>
    /// This class implements a concrete composition strategy (<see cref="Compositor"/>)
    /// for use by a <see cref="Composition"/> context instance. </summary>
    public partial class GeneticAlgorithmCompositor : Compositor
    {
        internal IList<MelodyGenome> Candidates { get; private set; }
        private protected Action<IBar>[] _mutations;


        public GeneticAlgorithmCompositor()
        {
            _mutations = new Action<IBar>[]
            {
                ChordPitchMutation,
                ScalePitchMutation,
                
            };
        }


        /// <inheritdoc/>
        internal override IEnumerable<IBar> Compose(IEnumerable<IBar> chordProgression, IEnumerable<IBar> seed = null)
        {
            // get first generatiion 
            InitializeFirstGeneration();

            int i = 0;
            bool terminateCondition = false;

            while (!terminateCondition)
            {
                // mix and combine pieces of different individuals 
                Crossover();

                // modify parts of individuals 
                Mutate(chordProgression);

                // rate each individual 
                EvaluateFitness();

                // natural selection 
                SelectNextGeneration();

                //MelodyGenome.CurrentGeneration++;
                if (i++ == 10)
                    terminateCondition = true;
            }

            // TODO: convert internal genome representation of each candidate in to a MIDI track chunk representation
            return chordProgression;
        }


        /// <summary>
        /// Initialize first generation of solution candidates. 
        /// </summary>
        protected internal void InitializeFirstGeneration()
        {
            Console.WriteLine($"In {this.GetType().FullName}, \ninitializing first generation");
        }

        /// <summary>
        /// Crossover two solution candidates and generate new solutions. 
        /// </summary>
        protected internal void Crossover()
        {

        }

        /// <summary>
        /// Alter a candidate's solution state.
        /// </summary>
        protected internal void Mutate(IEnumerable<IBar> melody)
        {
            Random randomizer = new Random();
            int NumberOfBars = melody.Count();
            int randomBarIndex = randomizer.Next(NumberOfBars);

            foreach (var bar in melody)
            {
                ChangePitchForARandomNote(bar, ChordNoteMappingSource.Chord, octaveRadius: 1);
                EqualDurationSplitMutation(bar);
                ChangePitchForARandomNote(bar, ChordNoteMappingSource.Scale, octaveRadius: 1);
                AnticipationDurationSplitMutation(bar);
                ChangePitchForARandomNote(bar, ChordNoteMappingSource.Chord, octaveRadius: 1);
                DelayDurationSplitMutation(bar);
                ChangePitchForARandomNote(bar, ChordNoteMappingSource.Chord, octaveRadius: 1);
            }
        }

        /// <summary>
        /// Evaluate the candidate solution's fitness.
        /// </summary>        
        protected internal void EvaluateFitness()
        {

        }


        /// <summary>
        /// Select the best evaluated solutions. 
        /// </summary>
        protected internal void SelectNextGeneration()
        {

        }
    }
}
