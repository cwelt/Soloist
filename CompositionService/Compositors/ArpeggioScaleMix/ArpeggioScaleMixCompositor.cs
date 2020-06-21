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
            switch (overallNoteDurationFeel)
            {
                case OverallNoteDurationFeel.Slow:
                    DefaultDurationFraction = Duration.QuaterNoteFraction;
                    DefaultDurationDenomniator = Duration.QuaterNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Medium:
                default:
                    DefaultDurationFraction = Duration.EighthNoteFraction;
                    DefaultDurationDenomniator = Duration.EighthNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Intense:
                    DefaultDurationFraction = Duration.SixteenthNoteFraction;
                    DefaultDurationDenomniator = Duration.SixteenthNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Extreme:
                    DefaultDurationFraction = Duration.ThirtySecondNoteFraction;
                    DefaultDurationDenomniator = Duration.ThirtySecondNoteDenominator;
                    break;
            }
            DefaultDuration = new Duration(1, DefaultDurationDenomniator);

            ChordProgression = chordProgression;
            Seed = melodyInitializationSeed;
            MinPitch = minPitch;
            MaxPitch = maxPitch;

            ScaleArpeggioeMixInitializer(chordProgression);
            return chordProgression;
        }
    }
}
