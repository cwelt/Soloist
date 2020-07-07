using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
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
        #region GetAllPitches(this IEnumerable<IBar> bars, bool includeRestHoldNotes = false)
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
        #endregion

        #region GetAllPitches(this IBar bar, bool includeRestHoldNotes = false)
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
        #endregion

        #region IsOffBeatNote(this INote note, IBar contextBar)
        /// <summary>
        /// Determines whether the given note starts on a off-beat in the context of 
        /// the given bar, or whether it starts on an on-beat.
        /// <para>
        /// The note is considered to be an off-beat if it does not start on a strong beat,
        /// i.e., it does not start in the beginning of the bar nor in the middle of the bar.
        /// </para>
        /// <para>
        /// Incase this method is intended to be called repeatedly through a bar, 
        /// it is recommended  to use the more effecient analogous version of INote extension 
        /// method, which carries out the same task, but saves calculations of data that is
        /// passed from the caller through additional parameters. Therefore the caller could 
        /// calculate them once per bar and pass them on continuous calls on the individual
        /// notes.
        /// </para>
        /// For more information, check it out: <see cref="IsOffBeatNote(IBar, float, float?)"/>
        /// </summary>
        /// <param name="note"> The subject note in question.</param>
        /// <param name="contextBar"> The bar which contains the note in question.</param>
        /// <returns></returns>
        public static bool IsOffBeatNote(this INote note, IBar contextBar)
        {
            // init 
            IDuration noteDuration;
            float beatAccumulator = 0; // accumulates preceding notes length
            
            // find starting point in bar for the given note 
            int noteIndex = contextBar.Notes.IndexOf(note);

            // assure the given note exists in the given context bar
            if (noteIndex < 0)
                return false;

            // accumulate length of beats that precede the given note  
            for (int i = 0; i < noteIndex; i++)
            {
                noteDuration = contextBar.Notes[i].Duration;
                beatAccumulator += (float)noteDuration.Numerator / noteDuration.Denominator;
            }

            // delegate task to optimized method version 
            return contextBar.IsOffBeatNote(startingBeat: beatAccumulator);
        }
        #endregion

        #region IsOffBeatNote(this IBar bar, float startingBeat, float? barLength = null)
        /// <summary>
        /// Determines whether the given starting beat starts on an off-beat in the subject  
        /// bar, or whether it starts on an on-beat.
        /// The note is considered to be an off-beat if it does not start on a strong beat,
        /// i.e., it does not start in the beginning of the bar nor in the middle of the bar.
        /// </summary>
        /// <param name="bar"> The subject bar in question.</param>
        /// <param name="startingBeat">The starting time of the beat in question, 
        /// in relation to it's containning bar. </param>
        /// <param name="barLength"> Length of the bar. On continuous calls on the same bar,
        /// pass this value explictly to acheive better performance. </param>
        /// <returns> True if the starting beat is an off-beat of the subject bar, false otherwise. </returns>
        public static bool IsOffBeatNote(this IBar bar, float startingBeat, float? barLength = null)
        {
            float barDuration = barLength ??
                    (float)bar.TimeSignature.Numerator / bar.TimeSignature.Denominator;

            // assure this beat is not the first or halfway beat of the bar 
            if ((startingBeat != 0) && ((barDuration % 2 != 0) || (startingBeat / barLength !=Duration.HalfNoteFraction)))
                return true;
            return false;
        }
        #endregion

        #region IsOffBeatNote(this IBar bar, float startingBeat, float? barLength = null)
        /// <summary>
        /// Determines whether the given starting beat starts on an off-beat in the subject  
        /// bar, or whether it starts on an on-beat.
        /// The note is considered to be an off-beat if it does not start on a strong beat,
        /// i.e., it does not start in the beginning of the bar nor in the middle of the bar.
        /// </summary>
        /// <param name="bar"> The subject bar in question.</param>
        /// <param name="startingBeat">The starting time of the beat in question, 
        /// in relation to it's containning bar. </param>
        /// <param name="barLength"> Length of the bar. On continuous calls on the same bar,
        /// pass this value explictly to acheive better performance. </param>
        /// <returns> True if the starting beat is an off-beat of the subject bar, false otherwise. </returns>
        public static float GetDurationInContext(this INote note, IBar barContext, IList<IBar> barSequenceContext, int? noteIndex = null, int? barIndex = null)
        {
            // initialization 
            INote nextNote;
            int nextNoteIndex, nextBarIndex;

            // initialize base duration of the subject note
            float noteDuration = (float)note.Duration.Numerator / note.Duration.Denominator;

            // set indices manually if necessary 
            noteIndex = noteIndex ?? barContext.Notes.IndexOf(note);
            barIndex = barIndex ?? barSequenceContext.IndexOf(barContext);

            // assure the given note, bar & bar sequence share the same context 
            if (noteIndex == -1 || barIndex == -1)
                return noteDuration;


            while((nextNote = barSequenceContext.GetSuccessorNote(excludeRestHoldNotes: false,
                (int)barIndex, (int)noteIndex, out nextBarIndex, out nextNoteIndex))
                ?.Pitch == NotePitch.HoldNote)
            {
                noteDuration += (float)nextNote.Duration.Numerator / nextNote.Duration.Denominator;
                noteIndex = nextNoteIndex;
                barIndex = nextBarIndex;
            }

            return noteDuration;
        }
        #endregion

        #region GetPredecessorNote()
        /// <summary> 
        /// <para>Gets the note in the melody which preceds the note at the given indices.</para>
        /// If <paramref name="excludeRestHoldNotes"/> is set, then hold and rest notes  
        /// would be bypassed, and the first preceding note which is not a rest or hold note 
        /// would be returned. If no preceding note is found then null is returned. 
        /// </summary>
        /// <param name="melodyBarsContext"> The bar sequence which contains the melody notes. </param>
        /// <param name="excludeRestHoldNotes">If set, rest notes and hold notes would be discarded during search for a preceding note.</param>
        /// <param name="barIndex"> Index of the bar containing the given note. </param>
        /// <param name="noteIndex"> Index of the note of whom it's predecessor is wanted. </param>
        /// <param name="precedingNoteBarIndex"> Index of the bar which contains the preceding note.</param>
        /// <param name="precedingNoteIndex">Index of the preceding note inside his containing note sequence.</param>
        /// <returns> Preceding note in the melody, or null if no predecessor note is found. </returns>
        public static INote GetPredecessorNote(this IList<IBar> melodyBarsContext, bool excludeRestHoldNotes, int barIndex, int noteIndex, out int precedingNoteBarIndex, out int precedingNoteIndex)
        {
            // initialization 
            INote note = null;
            int startingNoteIndex = 0;

            /* start scanning backwards from current bar & current note:
             * outer loop is for bars, inner loop for notes in the individual bars */
            for (int i = barIndex; i >= 0; i--)
            {
                /* in current bar start searching right before the given note.
                 * in the rest of the bars start from the right edge end of the bar. */
                startingNoteIndex = ((i == barIndex) ? (noteIndex - 1) : (melodyBarsContext[i].Notes.Count - 1));
                for (int j = startingNoteIndex; j >= 0; j--)
                {
                    note = melodyBarsContext[i].Notes[j];
                    if (!excludeRestHoldNotes || (note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote))
                    {
                        // set out params with the indices values and return the preceding note 
                        precedingNoteBarIndex = i;
                        precedingNoteIndex = j;
                        return note;
                    }
                }
            }

            // incase no preceding note is found, set the output accordingly 
            precedingNoteBarIndex = -1;
            precedingNoteIndex = -1;
            return null;
        }
        #endregion

        #region GetSuccessorNote()
        /// <summary> 
        /// <para>Gets the note in the melody which succeeds the note at the given indices.</para>
        /// If <paramref name="excludeRestHoldNotes"/> is set, then hold and rest notes  
        /// would be bypassed, and the first succeeding note which is not a rest or hold note 
        /// would be returned. If no succeeding note is found then null is returned. 
        /// </summary>
        /// <param name="melodyBarsContext"> The bar sequence which contains the melody notes. </param>
        /// <param name="excludeRestHoldNotes">If set, rest notes and hold notes would be discarded during search for a preceding note. </param>
        /// <param name="barIndex"> Index of the bar containing the given note. </param>
        /// <param name="noteIndex"> Index of the note of which it's predecessor is wanted. </param>
        /// <param name="succeedingNoteBarIndex">Index of the bar which contains the successor note. </param>
        /// <param name="succeedingNoteIndex">Index of the successor note inside his containing note sequence. </param>
        /// <returns> Succeeding note in the melody, or null if no successor note is found. </returns>
        public static INote GetSuccessorNote(this IList<IBar> melodyBarsContext, bool excludeRestHoldNotes, int barIndex, int noteIndex, out int succeedingNoteBarIndex, out int succeedingNoteIndex)
        {
            // initialization 
            INote note = null;
            int startingNoteIndex = 0;

            // start scanning forwards from current bar & current note 
            for (int i = barIndex; i < melodyBarsContext.Count; i++)
            {
                /* in current bar start searching right after the given note.
                 * in the rest of the bars start from the right beginning of the bar. */
                startingNoteIndex = ((i == barIndex) ? (noteIndex + 1) : 0);
                for (int j = startingNoteIndex; j < melodyBarsContext[i].Notes.Count; j++)
                {
                    note = melodyBarsContext[i].Notes[j];
                    if (!excludeRestHoldNotes || (note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote))
                    {
                        // set out params with the indices values and return the succeeding note 
                        succeedingNoteBarIndex = i;
                        succeedingNoteIndex = j;
                        return note;
                    }
                }
            }
            // incase no succeeding note is found, set the output accordingly 
            succeedingNoteBarIndex = -1;
            succeedingNoteIndex = -1;
            return null;
        }
        #endregion
    }
}
