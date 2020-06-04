using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Provides services which support operations and manipulations low level musical components 
    /// such as note, chord scale etc. 
    /// </summary>
    internal interface IMusicTheoryService
    {
         IEnumerable<NotePitch> GetChordNotes(NoteName chordRootNote, ChordType chordType, int minOctave = 0, int maxOctave = 9);
    }
}
