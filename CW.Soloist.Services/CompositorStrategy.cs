using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.Services
{

    /// <summary>
    /// Abstract compositor strategy class. 
    /// Subclasses of this class implement concrete composition strategies. 
    /// <para><remarks>
    /// Composition strategies are used by the <see cref="Composition"/> context class.
    /// </remarks></para>
    /// </summary>
    public abstract class CompositorStrategy
    {
        /// <summary>
        /// Compose a solo-melody over a given <paramref name="playback"/> 
        /// </summary>
        /// <remarks> This class is the abstract strategy class
        ///  in the strategy design pattern 
        ///  </remarks>
        /// <param name="playback"></param>
        /// <returns> The composition of solo-melody</returns>
        public abstract object Compose(string playback);
    }
}
