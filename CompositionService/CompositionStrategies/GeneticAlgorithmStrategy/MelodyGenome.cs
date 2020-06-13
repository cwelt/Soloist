using CW.Soloist.CompositionService.MusicTheory;
using System.Collections.Generic;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    // TODO: Use abstraction -> Implement interface of  melody genome 
    internal class MelodyGenome
    {
        internal int Generation { get; } = CurrentGeneration + 1;
        internal double FitnessGrade { get; set; } = 0;
        internal IEnumerable<IBar> Melody { get; set; }
        private protected bool isDirty { get; } = false;

        public static int CurrentGeneration { get; set; } = 0;
    }
}