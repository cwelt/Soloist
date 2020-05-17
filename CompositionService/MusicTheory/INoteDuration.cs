using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a note or chord duration.
    /// </summary>
    internal interface INoteDuration
    {
        byte Nominator { get; set; }
        byte Denominator { get; set; }
    }
}
