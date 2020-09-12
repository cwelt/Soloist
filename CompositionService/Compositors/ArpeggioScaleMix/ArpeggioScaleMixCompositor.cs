using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.ArpeggioScaleMix
{

    internal class ArpeggioScaleMixCompositor : Compositor
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ScaleArpeggioeMixInitializer(ChordProgression);
            return new[] { ChordProgression };
        }
    }
}
