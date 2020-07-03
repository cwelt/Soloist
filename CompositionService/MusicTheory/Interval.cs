using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary> 
    /// Represets an interval in semitones (halftones) within an octave. 
    /// </summary>
    internal enum PitchInterval
    {
        /// <summary> Unison, 0 semitones. </summary>
        [Description("Unsion")]
        Unison,

        /// <summary> Minor Second, 1 semitone. </summary>
        [Description("Minor Second")]
        MinorSecond,

        /// <summary> Major Second, 2 semitones. </summary>
        [Description("Major Second")]
        MajorSecond,

        /// <summary> Minor Third, 3 semitones </summary>
        [Description("Minor Third")]
        MinorThird,

        /// <summary> Major Third, 4 semitones. </summary>
        [Description("Major Third")]
        MajorThird,

        /// <summary> Perfect Fourth, 5 semitones. </summary>
        [Description("Perfect Fourth")]
        PerfectFourth,

        /// <summary> Tritone, 6 semitones. </summary>
        [Description("Tritone - Augmented 4th / Diminshed 5th")]
        Tritone,

        /// <summary> Perfect Fifth, 7 semitones. </summary>
        [Description("Perfect Fifth")]
        PerfectFifth,

        /// <summary> Minor Sixth, 8 semitones. </summary>
        [Description("Minor Sixth")]
        MinorSixth,

        /// <summary> Major Sixth, 9 semitones. </summary>
        [Description("Major Sixth")]
        MajorSixth,

        /// <summary> Minor Seventh, 10 semitones. </summary>
        [Description("Minor Seventh")]
        MinorSeventh,

        /// <summary> Major Seventh, 11 semitones. </summary>
        [Description("Major Seventh")]
        MajorSeventh,

        /// <summary> Octave, 12 semitones. </summary>
        [Description("Octave")]
        Octave
    }
}
