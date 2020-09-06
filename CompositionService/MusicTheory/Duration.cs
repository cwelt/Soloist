using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <inheritdoc cref="IDuration"/>
    internal class Duration : IDuration
    {
        #region Duration Constants
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
        #endregion


        #region Properties
        /// <summary> Number of beats </summary>
        public byte Numerator { get; set; }


        /// <summary> Duration of a single beat </summary>
        public byte Denominator { get; set; }


        /// <inheritdoc cref="IDuration.Fraction"/>
        public float Fraction => (float)Numerator / Denominator;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructs an instance of <see cref="IDuration"/> 
        /// with a duration of <paramref name="numerator"/> / <paramref name="denominator"/>.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <param name="reduceToLowestTerms"></param>
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
        /// <param name="duration"> The duration to base the values on. </param>
        /// <param name="reduceToLowestTerms"> Reduces the numerator and denominator do their lowest terms. </param>
        public Duration(IDuration duration, bool reduceToLowestTerms = true)
            : this(duration.Numerator, duration.Denominator, reduceToLowestTerms) { }
        #endregion


        #region IDuration Methods
        public IDuration Add(IDuration duration)
        {
            return MusicTheoryServices.DurationArithmetic(MusicTheoryServices.ArithmeticOperation.Add, this, duration)
                .ReduceFractionToLowestTerms();
        }

        public IDuration Subtract(IDuration duration)
        {
            return MusicTheoryServices.DurationArithmetic(MusicTheoryServices.ArithmeticOperation.Subtract, this, duration)
                .ReduceFractionToLowestTerms();
        }

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
        #endregion


        #region Additional Methods
        /// <summary> Returns string representation of this duration. </summary>
        /// <returns>A string representation of this duration. </returns>
        public override string ToString() => $"{Numerator}/{Denominator}";


        /// <summary> Utility function for calculating gcd with euclids'es algorithm. </summary>
        /// <returns> The greatest common divisor of <paramref name="a"/> and <paramref name="b"/>.</returns>
        internal static int GreatestCommonDivisor(int a, int b)
        {
            if (b == 0)
                return a;
            return GreatestCommonDivisor(b, a % b);
        }
        #endregion

        #region Operators
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
        #endregion
    }
}
