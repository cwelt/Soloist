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
    internal class Duration : IDuration
    {
        internal const float WholeNote = 1F;
        internal const float HalfNote = 0.5F;
        internal const float QuaterNote = 0.25F;
        internal const float EighthNote = 0.125F;
        internal const float SixteenthNote = 0.0625F;
        internal const float ThirtySecondNote = 0.03125F;
        internal const float TripletEighthNote = 1 / 12;
        internal const float TripletSixteenthNote = 1 / 24;

        /// <summary> Number of beats </summary>
        public byte Numerator { get; set; }

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
            Numerator = nominator;
            Denominator = denominator;
        }

        /// <summary>
        /// Copy constructor: constructs a <see cref="IDuration"/> instance 
        /// based on the <paramref name="duration"/> parameter values.
        /// </summary>
        /// <param name="duration"></param>
        public Duration(IDuration duration)
            : this(duration.Numerator, duration.Denominator) { }
        #endregion


        public IDuration Subtract(IDuration duration)
        {
            return MusicTheoryServices.DurationAritmetic(MusicTheoryServices.AritmeticOperation.Subtract, this, duration);
        }


        public IDuration Add(IDuration duration)
        {
            return MusicTheoryServices.DurationAritmetic(MusicTheoryServices.AritmeticOperation.Add, this, duration);
        }

        public override string ToString() => $"{Numerator}/{Denominator}";

        public bool IsDenominatorPowerOfTwo()
        {
            byte num = this.Denominator;
            int remainder = 0;
            while (remainder == 0)
            {
               remainder = num % 2;
               num /= 2;
            }
                
            if (num == 0)
                return true;
            else return false; 
        }
    }
}
