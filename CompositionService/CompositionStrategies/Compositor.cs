using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies
{

    /// <summary>
    /// Abstract compositor strategy class. 
    /// Subclasses of this class implement concrete composition strategies. 
    /// <para><remarks>
    /// Composition strategies are used by the <see cref="Composition"/> context class.
    /// This class is the abstract strategy class in the strategy design pattern.
    /// </remarks></para>
    /// </summary>
    public abstract class Compositor
    {
        /// <summary>
        /// Compose a solo-melody over a given <paramref name="playback"/>.
        /// <param name="playback"> The MIDI playback file name.</param>
        /// <param name="chordProgression"> The chords of the song in the playback. </param>
        /// <returns> The composition of solo-melody</returns>
        /// </summary>

        public abstract object Compose(string playback, IEnumerable<object> chordProgression);
    }
}
