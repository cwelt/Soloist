﻿using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.Arpeggiator
{
    internal class ArpeggiatorCompositor : Compositor
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ArpeggiatorInitializer(ChordProgression);
            return new [] { ChordProgression };
        }
    }
}
