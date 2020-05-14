using CW.Soloist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.GeneticAlgorithmCompositor
{
    public abstract class GeneticAlgorithmCompositor : CompositorStrategy
    {
        public override object Compose(string playback)
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


        protected internal abstract void InitializeFirstGeneration(); 
        protected internal abstract void Crossover(); 
        protected internal abstract void Mutate(); 
        protected internal abstract void EvaluateFitness(); 
        protected internal abstract void SelectNextGeneration(); 
    }
}
