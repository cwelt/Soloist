using System.Collections.Generic;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.CompositionService.Composers.Arpeggiator
{
    /// <summary>
    /// Basic composer service which uses an arpeggiator to 
    /// generate a melody, i.e., a generated sequence of arpeggio notes that
    /// belong to the chords from the backing chord progression.
    /// </summary>
    internal class ArpeggiatorComposer : Composer
    {
        private protected override IEnumerable<IList<IBar>> GenerateMelody()
        {
            ArpeggiatorInitializer(ChordProgression);
            return new [] { ChordProgression };
        }
    }
}
