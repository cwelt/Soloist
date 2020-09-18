using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.Arpeggiator
{
    internal class ArpeggiatorComposer : Composer
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ArpeggiatorInitializer(ChordProgression);
            return new [] { ChordProgression };
        }
    }
}
