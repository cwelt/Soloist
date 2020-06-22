using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.Scalerator
{
    internal class ScaleratorCompositor : Compositor
    {
        private protected override IList<IBar> GenerateMelody()
        {
            ScaleratorInitializer(ChordProgression);
            return ChordProgression;
        }
    }
}
