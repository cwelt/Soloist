using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.UtilEnums
{
    /// <summary>
    /// Direction of chord arpeggio notes.  
    /// </summary>
    internal enum NoteSequenceMode
    {
        Ascending,
        Descending,
        ChordZigzag,
        BarZigzag,
    }
}
