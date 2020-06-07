using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Provides services which support operations and manipulations low level musical 
    /// components such as note, chord scale etc. 
    /// </summary>
    internal interface IMusicTheoryService
    {
        /// <summary>
        /// Returns note pitches in a specified range that map to the given chord. 
        /// </summary>
        /// <param name="chord"></param>
        /// <param name="mappingSource"></param>
        /// <param name="minOctave"></param>
        /// <param name="maxOctave"></param>
        /// <returns></returns>
         IEnumerable<NotePitch> GetChordNotes(IChord chord, ChordNoteMappingSource mappingSource, int minOctave = 0, int maxOctave = 9);
    }
}
