using CW.Soloist.CompositionService.Composers.Arpeggiator;
using CW.Soloist.CompositionService.Composers.ArpeggioScaleMix;
using CW.Soloist.CompositionService.Composers.GeneticAlgorithm;
using CW.Soloist.CompositionService.Composers.Scalerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Composers
{
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
