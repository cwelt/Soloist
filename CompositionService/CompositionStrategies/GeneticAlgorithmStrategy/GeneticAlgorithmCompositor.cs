using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class GeneticAlgorithmCompositor : Compositor
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// 
        /// <returns></returns>
        public override object Compose(string playback, IEnumerable<object> chordProgression)
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
                Mutate();

                // rate each individual 
                EvaluateFitness();

                // natural selection 
                SelectNextGeneration();
                //MelodyGenome.CurrentGeneration++;

                if (i++ == 100)
                    terminateCondition = true;
            }

            // TODO: convert internal genome representation of each candidate in to a MIDI track chunk representation
            return null;

        }


        /// <summary>
        /// Initialize first generation of solution candidates. 
        /// </summary>
        protected internal void InitializeFirstGeneration()
        {
            Console.WriteLine($"In {this.GetType().FullName}, initializing first generation");
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
        protected internal void Mutate()
        {

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
