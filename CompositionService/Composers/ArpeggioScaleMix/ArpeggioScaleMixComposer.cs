using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.ArpeggioScaleMix
{
    /// <summary>
    /// Basic composer service which uses a mix of a scalerator 
    /// and an arpeggiator to generate a melody, i.e., a generated sequence of arpeggieos
    /// scale chord notes and scale notes from scales that are mapped to the backing chord progression.
    /// </summary>
    internal class ArpeggioScaleMixComposer : Composer
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ScaleArpeggioeMixInitializer(ChordProgression);
            return new[] { ChordProgression };
        }
    }
}
