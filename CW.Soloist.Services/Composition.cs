using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.Services
{
    /// <summary>
    /// Context service class for composing a solo-melody over a given playback.
    /// <para>
    /// <remarks> This class serves as the context participant in the strategy design pattern. </remarks>
    /// </para>
    /// </summary>
    public class Composition
    {

        /// <summary>
        /// <value> 
        /// Gets or sets the compositor strategy responsible for composing the solo melody.
        /// See <see cref="CompositorStrategy"/>.
        /// </value> 
        /// </summary>
        public CompositorStrategy Compositor { get; set; }
    }
}
