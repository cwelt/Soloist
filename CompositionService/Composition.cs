using CW.Soloist.CompositionService.CompositionStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService
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
        /// Gets or sets the compositor responsible for composing the solo melody with the desired composition strategy.
        /// See <see cref="Compositor"/>.
        /// </value> 
        /// </summary>
        public Compositor Compositor { get; set; }
    }
}
