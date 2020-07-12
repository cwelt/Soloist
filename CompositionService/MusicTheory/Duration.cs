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
        internal const float WholeNoteFraction = 1F;
        internal const float HalfNoteFraction = 0.5F;
        internal const float QuaterNoteFraction = 0.25F;
        internal const float EighthNoteFraction = 0.125F;
        internal const float SixteenthNoteFraction = 0.0625F;
        internal const float ThirtySecondNoteFraction = 0.03125F;
        internal const float TripletEighthNoteFraction = 1F / 12;
        internal const float TripletSixteenthNoteFraction = 1F / 24;
        internal const byte WholeNoteDenominator = 1;
        internal const byte HalfNoteDenominator = 2;
        internal const byte QuaterNoteDenominator = 4;
        internal const byte EighthNoteDenominator = 8;
        internal const byte SixteenthNoteDenominator = 16;
        internal const byte ThirtySecondNoteDenominator = 32;
        internal const byte TripletEighthNoteDenominator = 12;
        internal const byte TripletSixteenthNoteDenominator = 24;

        /// <summary> Number of beats </summary>
        public byte Numerator { get; set; }

        /// <summary> Duration of a single beat </summary>
        public byte Denominator { get; set; }

        /// <inheritdoc cref="IDuration.Fraction"/>
        public float Fraction => (float)Numerator / Denominator;

        #region Constructors
        /// <summary>
        /// Constructs an instance of <see cref="IDuration"/> 
        /// with a duration of <paramref name="numerator"/> / <paramref name="denominator"/>.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Duration(byte numerator = 1, byte denominator = 4, bool reduceToLowestTerms = true)
        {
            if (reduceToLowestTerms)
            {
                byte gcd = (byte)GreatestCommonDivisor(numerator, denominator);
                Numerator = (byte)(numerator / gcd);
                Denominator = (byte)(denominator / gcd);
            }
            else
            {
                Numerator = numerator;
                Denominator = denominator;
            }
        }

        /// <summary>
        /// Copy constructor: constructs a <see cref="IDuration"/> instance 
        /// based on the <paramref name="duration"/> parameter values.
        /// </summary>
        /// <param name="duration"></param>
        public Duration(IDuration duration, bool reduceToLowestTerms = true)
            : this(duration.Numerator, duration.Denominator, reduceToLowestTerms) { }
        #endregion


        public IDuration Subtract(IDuration duration)
        {
            return MusicTheoryServices.DurationAritmetic(MusicTheoryServices.AritmeticOperation.Subtract, this, duration)
                .ReduceFractionToLowestTerms();
        }

        public IDuration Add(IDuration duration)
        {
            return MusicTheoryServices.DurationAritmetic(MusicTheoryServices.AritmeticOperation.Add, this, duration)
                .ReduceFractionToLowestTerms();
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

            /* // alternative implemntation which is more compact but less clear 
            return (int)Math.Ceiling(Math.Log(Denominator) / Math.Log(2))
                == (int)Math.Floor(Math.Log(Denominator) / Math.Log(2)); */
        }

        public static bool operator <= (Duration duration1, IDuration duration2)
        {
            return duration1.Fraction <= duration2.Fraction;
        }

        public static bool operator >= (Duration duration1, IDuration duration2)
        {
            return duration1.Fraction >= duration2.Fraction;
        }

        public static bool operator < (Duration duration1, IDuration duration2)
        {
            return duration1.Fraction < duration2.Fraction;
        }

        public static bool operator > (Duration duration1, IDuration duration2)
        {
            return duration1.Fraction > duration2.Fraction;
        }

        //  function for calculating gcd with euclids'es algorithm
        internal static int GreatestCommonDivisor(int a, int b)
        {
            if (b == 0)
                return a;
            return GreatestCommonDivisor(b, a % b);
        }
    }
}
