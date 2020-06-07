using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a source for mapping between a given chord and note pitches. 
    /// </summary>
    internal enum ChordNoteMappingSource
    {
        /// <summary>
        /// Map the notes from the chord's structure arpeggio notes, 
        /// for example, map C major chord to notes C (do), E (mi) and G (sol).
        /// </summary>
        Chord,

        /// <summary>
        /// Map the notes from a pre-determined scale that contain notes that
        /// are compatible to the chord, a major chord might be mapped to the dorian
        /// major scale, and dominant 7 chord might be mapped to the blues scale. 
        /// The actual notes in the mapping scale are implementation dependent.
        /// </summary>
        Scale
    }
}
