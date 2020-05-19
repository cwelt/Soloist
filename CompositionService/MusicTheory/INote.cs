using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a single musical note instance. 
    /// <para>
    /// Each note is represented by a note duration (<see cref="Duration"/>)
    /// and an absolute pitch (<see cref="Pitch"/>), 
    /// which also determines it's name (<see cref="Name"/>).
    /// </para>
    /// </summary>
    internal interface INote
    {
        /// <summary> The note's name (<see cref="NoteName"/>) .</summary>
        NoteName? Name { get; }

        /// <summary> The note's absolute pitch (<see cref="NotePitch"/>) .</summary>
        NotePitch Pitch { get; set; }

        /// <summary> The note's duration (<see cref="Duration"/>) .</summary>
        IDuration Duration { get; set; }
    }
}
