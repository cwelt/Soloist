using CW.Soloist.CompositionService.MusicTheory;
using System.Collections.Generic;

namespace CW.Soloist.CompositionService.Composers.Scalerator
{
    internal class ScaleratorComposer : Composer
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ScaleratorInitializer(ChordProgression);
            return new[] { ChordProgression };
        }
    }
}
