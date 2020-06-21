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
        internal override IList<IBar> Compose(
            IList<IBar> chordProgression,
            IList<IBar> melodyInitializationSeed = null,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Intense,
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E8)
        {
            ScaleArpeggioeMixInitializer(chordProgression);
            return chordProgression;
        }
    }
}
