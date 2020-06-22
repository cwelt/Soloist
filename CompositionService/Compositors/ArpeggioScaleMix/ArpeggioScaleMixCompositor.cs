using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.ArpeggioScaleMix
{

    internal class ArpeggioScaleMixCompositor : Compositor
    {
        private protected override IList<IBar> GenerateMelody()
        {
            ScaleArpeggioeMixInitializer(ChordProgression);
            return ChordProgression;
        }
    }
}
