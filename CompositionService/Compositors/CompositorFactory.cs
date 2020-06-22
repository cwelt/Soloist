using CW.Soloist.CompositionService.Compositors.Arpeggiator;
using CW.Soloist.CompositionService.Compositors.ArpeggioScaleMix;
using CW.Soloist.CompositionService.Compositors.GeneticAlgorithm;
using CW.Soloist.CompositionService.Compositors.Scalerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors
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
        internal static Compositor CreateCompositor(CompositionStrategy strategy = CompositionStrategy.GeneticAlgorithmStrategy)
        {
            switch (strategy)
            {
                case CompositionStrategy.GeneticAlgorithmStrategy:
                    return new GeneticAlgorithmCompositor();
                case CompositionStrategy.ArpeggiatorStrategy:
                    return new ArpeggiatorCompositor();
                case CompositionStrategy.ScaleratorStrategy:
                    return new ScaleratorCompositor();
                case CompositionStrategy.ArpeggioScaleMixStrategy:
                    return new ArpeggioScaleMixCompositor();
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }
}
