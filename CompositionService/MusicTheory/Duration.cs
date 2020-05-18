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
    /// Each duration is assembled of two properties: <see cref="Nominator"/>, 
    /// which represents the number of beats in the duration, 
    /// and <see cref="Denominator"/>, which represents the duration of a single beat,
    /// in relation to whole note duration. For example, a chord which is played 
    /// over half a bar, would have a duration of 1/2. Duration of notes and chords 
    /// are calculated as the quotient of the nominator divided by the denominator, 
    /// so for example, a duration instance of 2/4 is equivalent to 1/2. 
    /// </remarks>
    internal class Duration : IDuration
    {
        /// <summary> Number of beats </summary>
        public byte Nominator { get; set; }

        /// <summary> Duration of a single beat </summary>
        public byte Denominator { get; set; }

        #region Constructors
        /// <summary>
        /// Constructs an instance of <see cref="IDuration"/> 
        /// with a duration of <paramref name="nominator"/> / <paramref name="denominator"/>.
        /// </summary>
        /// <param name="nominator"></param>
        /// <param name="denominator"></param>
        public Duration(byte nominator = 1, byte denominator = 4)
        {
            Nominator = nominator;
            Denominator = denominator;
        }

        /// <summary>
        /// Copy constructor: constructs a <see cref="IDuration"/> instance 
        /// based on the <paramref name="duration"/> parameter values.
        /// </summary>
        /// <param name="duration"></param>
        public Duration(IDuration duration)
            : this(duration.Nominator, duration.Denominator) { }

        #endregion

        public override string ToString() => $"{Nominator}/{Denominator}";
    }
}
