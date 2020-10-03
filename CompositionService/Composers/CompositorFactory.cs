using System;
using CW.Soloist.CompositionService.Composers.Scalerator;
using CW.Soloist.CompositionService.Composers.Arpeggiator;
using CW.Soloist.CompositionService.Composers.ArpeggioScaleMix;
using CW.Soloist.CompositionService.Composers.GeneticAlgorithm;

namespace CW.Soloist.CompositionService.Composers
{
    /// <summary>
    /// Factory for creating composers (see <see cref="Composer"/>) 
    /// according to the requested composing algorithm strategy. 
    /// </summary>
    internal static class CompositorFactory
    {
        #region CreateCompositor()
        /// <summary>
        /// Factory for creating a compositor instance based on the given 
        /// composition strategy enumeration value.  
        /// </summary>
        /// <param name="strategy"> The requested composition algorithm strategy. </param>
        /// <returns></returns>
        internal static Composer CreateCompositor(CompositionStrategy strategy = CompositionStrategy.GeneticAlgorithmStrategy)
        {
            switch (strategy)
            {
                case CompositionStrategy.GeneticAlgorithmStrategy:
                    return new GeneticAlgorithmComposer();
                case CompositionStrategy.ArpeggiatorStrategy:
                    return new ArpeggiatorComposer();
                case CompositionStrategy.ScaleratorStrategy:
                    return new ScaleratorComposer();
                case CompositionStrategy.ArpeggioScaleMixStrategy:
                    return new ArpeggioScaleMixComposer();
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }
}
