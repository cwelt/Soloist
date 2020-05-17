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
    internal class NoteDuration : INoteDuration
    {
        public byte Nominator { get; set; }
        public byte Denominator { get; set; }

        #region Constructors
        /// <summary>
        /// Constructs an instance of <see cref="INoteDuration"/> 
        /// with a duration of <paramref name="nominator"/> / <paramref name="denominator"/>.
        /// </summary>
        /// <param name="nominator"></param>
        /// <param name="denominator"></param>
        public NoteDuration(byte nominator = 1, byte denominator = 4)
        {
            Nominator = nominator;
            Denominator = denominator;
        }
        #endregion

        public override string ToString() => $"{Nominator}/{Denominator}";
    }
}
