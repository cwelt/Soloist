using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.Scalerator
{
    /// <summary>
    /// Basic composer service which uses a scalerator to 
    /// generate a melody, i.e., a generated sequence of scale notes 
    /// of scales that are mapped to the backing chord progression.
    /// </summary>
    internal class ScaleratorComposer : Composer
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ScaleratorInitializer(ChordProgression);
            return new[] { ChordProgression };
        }
    }
}
