using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// A chord's type (structure) that is determined by the intervals 
    /// between the individual notes in the chord. 
    /// </summary>
    /// <remarks>
    /// The documentation for the individual chord types include the degrees
    /// relative to the chord's root. The 'b' sign represents a flatted degree, 
    /// and a '#' represents an augmented degree.  
    /// For example 1-3-5 represents a triangle major chord, 
    /// 1-3b-5 a triangle minor chord, 
    /// and 1-3-5# an triangle augmented 5 major chord. 
    /// </remarks>
    public enum ChordType
    {
        /// <summary> Diminished chord (1-3b-5b-7b). </summary>
        Diminished,

        /// <summary> Dominant 7 chord (1-3-5-7b). </summary>
        Dominant7,

        /// <summary> Dominant 7 chord With a suspended 4th (1-4-5-7b). </summary>
        Dominant7Suspended4,

        /// <summary> Dominant 7 chord with an augemented 5th (1-3-5#-7b). </summary>
        Dominant7Augmented5,

        /// <summary> Dominant 7 chord with a flattend 9th (1-3-5-7b-9b). </summary>
        Dominant7b9,

        /// <summary> Dominant 7 chord with an augmented 9th (1-3-5-7b-9#). </summary>
        Dominant7Sharped9,

        /// <summary> Half-Diminished chord (1-3b-5b-7). </summary>
        HalfDiminished,

        /// <summary> Major triangle chord (1-3-5). </summary>
        Major,

        /// <summary> Major 13th chord (1-3-5-6). </summary>
        Major13,

        /// <summary> Major triangle suspended 4th chord (1-4-5). </summary>
        MajorSuspended4,

        /// <summary> Major triangle augmented 5th chord (1-3-5#). </summary>
        MajorAugmented5,

        /// <summary> Major 7 chord (1-3-5-7). </summary>
        Major7,

        /// <summary> Minor triangle chord (1-3b-5). </summary>
        Minor,

        /// <summary> Minor 13th chord (1-3b-5-6). </summary>
        Minor6,

        /// <summary> Minor 7 chord (1-3b-5-7b). </summary>
        Minor7,

        /// <summary> Minor-Major chord (1-3b-5-7). </summary>
        MinorMajor7,
    }
}
