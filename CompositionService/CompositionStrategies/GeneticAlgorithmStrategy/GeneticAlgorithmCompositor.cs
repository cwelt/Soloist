﻿using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    /// <summary>
    /// Compose a solo-melody over a given playback by implementing a genetic algorithm,
    /// <para>
    /// This class implements a concrete composition strategy (<see cref="Compositor"/>)
    /// for use by a <see cref="Composition"/> context instance.
    /// </para>
    /// </summary>
    public class GeneticAlgorithmCompositor : Compositor
    {
        /// <inheritdoc/>
        internal override IEnumerable<IBar> Compose(string playback, IEnumerable<object> chordProgression)
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
