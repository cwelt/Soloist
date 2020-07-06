using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Extension methods for music theory entites. 
    /// This class contains for instance utillity methods for extracting nested collections 
    /// of note pitches from a collection of indivdual bars.
    /// </summary>
    internal static class MusicTheoryExtensions
    {

        /// <summary>
        /// Retreives all note pitches that are contained in a bar sequence,
        /// and returns them in a flattened sequence, either including rest and hold note,
        /// or excluding them, according to <paramref name="includeRestHoldNotes"/> flag.
        /// By default rest and hold notes are filtered, i.e., excluded. 
        /// </summary>
        /// <param name="bars"> The bar sequence to retrieve the pitches from.</param>
        /// <param name="includeRestHoldNotes"> Flag indicator for either including or excluding rest and hold notes.</param>
        /// <returns> Flattened sequence of all note pitches from the given bar sequence. </returns>
        public static IEnumerable<NotePitch> GetAllPitches(this IEnumerable<IBar> bars, bool includeRestHoldNotes = false)
        {
            // set predicate filter acoording to incoming parameter flag 
            Func<NotePitch, bool> predicate;
            if (includeRestHoldNotes)
                predicate = pitch => pitch != NotePitch.HoldNote && pitch != NotePitch.RestNote;
            else predicate = pitch => true;

            // filter and return notes which satisfy the predicate 
            return bars.SelectMany(bar => bar.Notes
                .Select<INote, NotePitch>(note => note.Pitch)
                .Where(pitch => pitch != NotePitch.HoldNote && pitch != NotePitch.RestNote));
        }

        /// <summary>
        ///  Retreives all note pitches that are contained in the given bar,
        ///  either including rest and hold note, or excluding them, 
        ///  according to <paramref name="includeRestHoldNotes"/> flag.
        ///  By default rest and hold notes are filtered, i.e., excluded. 
        /// </summary>
        /// <remarks> This method is an overload of 
        /// <see cref="GetAllPitches(IEnumerable{IBar}, bool)"/>
        /// which carries the same request for a bar sequence. 
        /// </remarks>
        /// <param name="bar"> The bar for whom to retrieve it's pitches. </param>
        /// <param name="includeRestHoldNotes"> Flag indicator for either including or excluding rest and hold notes.</param>
        /// <returns> Sequence of all note pitches from the given bar sequence. </returns>
        public static IEnumerable<NotePitch> GetAllPitches(this IBar bar, bool includeRestHoldNotes = false)
        {
            // delegate request to overloaded version which handles collections of bars
            return GetAllPitches(new[] { bar }, includeRestHoldNotes);
        }
    }
}
