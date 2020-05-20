using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies.ArpeggiatorStrategy
{
    /// <summary>
    /// Compose a solo-melody over a given playback by implementing an arpeggiator,
    /// which generates arpeggioes of the chord note under the given 
    /// <para>
    /// This class implements a concrete composition strategy (<see cref="Compositor"/>)
    /// for use by a <see cref="Composition"/> context instance.
    /// </para>
    /// </summary>
    public class ArpeggiatorCompositor : Compositor
    {
        /// <inheritdoc/>
        internal override IEnumerable<IBar> Compose(string playback, IEnumerable<object> chordProgression)
        {
            throw new NotImplementedException();
        }
    }
}
