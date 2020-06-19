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
        /// <summary> Number of beats in the duration. </summary>
        byte Numerator { get; set; }

        /// <summary> Length of the beat in relation to a whole note duration. </summary>
        byte Denominator { get; set; }

        /// <summary>
        /// Subtracts the given duration from this duration instance and returns 
        /// the difference length between them as a new duration. 
        /// </summary>
        /// <remarks> The argument's length must be equal or shorter than this duration's length. </remarks>
        /// <param name="duration"> The duration to subtract from this duration. </param>
        /// <returns> The new duration which is the result of the subtraction. </returns>
        IDuration Subtract(IDuration duration);

        /// <summary>
        /// Adds the given duration to this duration instance and returns 
        /// the sum length of them as a new duration. 
        /// </summary>
        /// <param name="duration"> The duration to add to this one. </param>
        /// <returns> The new duration with a length which is the sum of the addition. </returns>
        IDuration Add(IDuration duration);

        /// <summary>
        /// Utility method which determins whether this duration's 
        /// denominator is a power of two or not.
        /// </summary>
        /// <returns> True if this duration's denominator is a power of two, and false elsewise.</returns>
        bool IsDenominatorPowerOfTwo();
    }
}
