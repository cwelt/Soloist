using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.Arpeggiator
{
    internal class ArpeggiatorCompositor : Compositor
    {
        internal override IList<IBar> Compose(
            IList<IBar> chordProgression, 
            IList<IBar> melodyInitializationSeed = null, 
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium, 
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E6)
        {
            ArpeggiatorInitializer(chordProgression);
            return chordProgression;
        }
    }
}
