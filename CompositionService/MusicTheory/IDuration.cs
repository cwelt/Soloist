using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a duration of a musical component such as a note or chord. 
    /// </summary>
    /// <remarks>
    /// Each duration is assembled of two properties: <see cref="Numerator"/>, 
    /// which represents the number of beats in the duration, 
    /// and <see cref="Denominator"/>, which represents the duration of a single beat,
    /// in relation to whole note duration. For example, a chord which is played 
    /// over half a bar, would have a duration of 1/2. Duration of notes and chords 
    /// are calculated as the quotient of the nominator divided by the denominator, 
    /// so for example, a duration instance of 2/4 is equivalent to 1/2. 
    /// </remarks>

    public interface IDuration
    {
        byte Numerator { get; set; }
        byte Denominator { get; set; }
    }
}
