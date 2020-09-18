using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.ArpeggioScaleMix
{
    internal class ArpeggioScaleMixComposer : Composer
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ScaleArpeggioeMixInitializer(ChordProgression);
            return new[] { ChordProgression };
        }
    }
}
