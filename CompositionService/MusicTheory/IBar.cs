using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a single musical bar in a musical piece. 
    /// <para>
    /// Each bar contains a time signature (<see cref="IDuration"/>) which 
    /// represents the number of beats in the bar and the duration of each beat,
    /// a list of chords (<see cref="IChord"/>) which represent the harmony in
    /// the current bar, and a list of notes (<see cref="INote"/>).
    /// </para>
    /// </summary>
    public interface IBar
    {
        /// <summary> 
        /// Time signature of current bar which contains the bar's duration: 
        /// number of beats in a bar, and the duration of a single beat, 
        /// for example 4/4 for standard western music and 3/4 for valse. 
        /// </summary>
        IDuration TimeSignature { get; }

        /// <summary> The chords which represent the harmony in the bar. </summary>
        IList<IChord> Chords { get; }

        /// <summary> The notes which represent the melody in the bar. </summary>
        IList<INote> Notes { get; set; }
    }
}
