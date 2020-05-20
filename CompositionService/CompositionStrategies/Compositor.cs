using CW.Soloist.CompositionService.MusicTheory;
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
        /// <summary> Melody seed to base on the composition of the new melody. </summary>
        internal IEnumerable<INote> Seed { get; }

        /// <summary> The outcome of the <see cref="Compose"/> method. </summary>
        internal IEnumerable<INote> ComposedMelody { get; private set; }

        /// <summary> The playback's harmony. </summary>
        internal IEnumerable<IBar> ChordProgression { get; }

        /// <summary> Default duration denominator for a single note. </summary>
        internal int DefaultDuration { get; set; } = 8;

        /// <summary> Minimum octave of note pitch range for the composition. </summary>
        internal int MinOctave { get; set; } = 3;

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        internal int MaxOctave { get; set; } = 9;

        /// <summary>
        /// Compose a solo-melody over a given <paramref name="playback"/>.
        /// </summary>
        /// <param name="playback"> The MIDI playback file name.</param>
        /// <param name="chordProgression"> The chords of the song in the playback. </param>
        /// <returns> The composition of solo-melody</returns>

        internal abstract IEnumerable<IBar> Compose(string playback, IEnumerable<object> chordProgression);
    }
}
