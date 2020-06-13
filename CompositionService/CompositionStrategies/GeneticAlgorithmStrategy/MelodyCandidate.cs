using CW.Soloist.CompositionService.MusicTheory;
using System.Collections.Generic;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    // TODO: Use abstraction -> Implement interface of  melody genome 
    internal class MelodyCandidate
    {
        internal int Generation { get; } = CurrentGeneration + 1;
        internal double FitnessGrade { get; set; } = 0;
        
        /// <summary> List of bars which contain the melody of this candidate. </summary>
        internal IList<IBar> Bars { get; set; }
        private protected bool isDirty { get; } = false;

        public static int CurrentGeneration { get; set; } = 0;
    }
}