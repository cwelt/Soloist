using System.ComponentModel;

namespace CW.Soloist.CompositionService.Composers
{
    /// <summary>
    /// Strategy algorithm for composing a melody.
    /// </summary>
    public enum CompositionStrategy
    {
        /// <summary> Genetic algorithm strategy. </summary>
        [Description("Genetic Algorithm Strategy")]
        GeneticAlgorithmStrategy,

        /// <summary> Arpeggio based strategy. </summary>
        [Description("Arpeggio Based Strategy")]
        ArpeggiatorStrategy,

        /// <summary> Scale based strategy. </summary>
        [Description("Scale Based Strategy")]
        ScaleratorStrategy,

        /// <summary> Mix of arpeggio-scale based strategy. </summary>
        [Description("Arpeggio-Scale Based Mix Strategy")]
        ArpeggioScaleMixStrategy,
    }
}
