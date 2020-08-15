namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary> <para>
    /// Represents a duration of a musical component such as a note or chord. </para>
    /// Each duration is assembled of two properties: <see cref="Numerator"/>, 
    /// which represents the number of beats in the duration, 
    /// and <see cref="Denominator"/>, which represents the duration of a single beat,
    /// in relation to whole note duration. For example, a chord which is played 
    /// over half a bar, would have a duration of 1/2. Duration of notes and chords 
    /// are calculated as the quotient of the nominator divided by the denominator, 
    /// so for example, a duration instance of 2/4 is equivalent to 1/2. 
    /// </summary>
    public interface IDuration
    {
        /// <summary> Number of beats in the duration. </summary>
        byte Numerator { get; set; }

        /// <summary> Length of the beat in relation to a whole note duration. </summary>
        byte Denominator { get; set; }

        /// <summary>
        /// Length of the duration as a fraction that is the quotient of the division
        /// of the duration's numerator by the duration's denominator. 
        /// For exmaple, a duration of 1/4 has a fraction of 0.25.
        /// </summary>
        float Fraction { get; }

        /// <summary>
        /// Adds the given duration to this duration instance and returns 
        /// the sum length of them as a new duration. 
        /// </summary>
        /// <param name="duration"> The duration to add to this one. </param>
        /// <returns> The new duration with a length which is the sum of the addition. </returns>
        IDuration Add(IDuration duration);

        /// <summary>
        /// Subtracts the given duration from this duration instance and returns 
        /// the difference length between them as a new duration. 
        /// </summary>
        /// <remarks> The argument's length must be equal or shorter than this duration's length. </remarks>
        /// <param name="duration"> The duration to subtract from this duration. </param>
        /// <returns> The new duration which is the result of the subtraction. </returns>
        IDuration Subtract(IDuration duration);

        /// <summary> 
        /// Utility method which determines whether this duration's 
        /// denominator is a power of two or not.
        /// This could be helpful for musical quantization process.
        /// </summary>
        /// <returns> True if this duration's denominator is a power of two, and false elsewise.</returns>
        bool IsDenominatorPowerOfTwo();
    }
}