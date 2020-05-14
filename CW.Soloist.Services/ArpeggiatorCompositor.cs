using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.Services
{
    /// <summary>
    /// Compose a solo-melody over a given playback by implementing an arpeggiators.
    /// <para>
    /// This class implements a concrete composition strategy. 
    /// </para>
    /// </summary>
    public class ArpeggiatorCompositor : CompositorStrategy
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="playback"></param>
        /// <returns></returns>
        public override object Compose(string playback)
        {
            throw new NotImplementedException();
        }
    }
}
