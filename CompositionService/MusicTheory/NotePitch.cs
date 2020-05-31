using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents one of the 128 midi pitches, 
    /// a silent note <see cref="RestNote"/> 
    /// or a hold note <see cref="HoldNote"/>
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item> The enumeration names are sharp oriented, 
    /// for example 'ASharp' instead of 'BFlat',
    /// but the pitch representations are equivalent.</item>
    /// <item>
    /// The suffix of each enumeration pitch name represents the relative octave
    /// in the midi chart, for example: 'A4' represents the pitch of A in the fourth 
    /// octave with the frequency of 440 Hz.
    /// </item>
    /// </list>
    /// </remarks>
    public enum NotePitch
    {
        /// <summary> Represents a hold note, i.e., a note that holds the pitch of it's predecessor note. </summary>
        [Description("Hold Note")]
        HoldNote = 128,

        /// <summary> Represents a hold note (i.e., silent note).  </summary>
        [Description("Rest Note")]
        RestNote = -1,

        // all the midi notes from 0 to 127 
        [Description("C(-1)")]
        CMinus1,

        [Description("C#(-1)")]
        CSharpMinus1,

        [Description("D(-1)")]
        DMinus1,

        [Description("D#(-1)")]
        DSharpMinus1,

        [Description("E(-1)")]
        EMinus1,

        [Description("F(-1)")]
        FMinus1,

        [Description("F#(-1)")]
        FSharpMinus1,

        [Description("G(-1)")]
        GMinus1,

        [Description("G#(-1)")]
        GSharpMinus1,

        [Description("A(-1)")]
        AMinus1,

        [Description("A#(-1)")]
        ASharpMinus1,

        [Description("B(-1)")]
        BMinus1,

        [Description("C(0)")]
        C0,

        [Description("C#(0)")]
        CSharp0,

        [Description("D#(0)")]
        D0,

        [Description("D#(0)")]
        DSharp0,

        [Description("E(0)")]
        E0,

        [Description("F(0)")]
        F0,

        [Description("F#(0)")]
        FSharp0,

        [Description("G(0)")]
        G0,

        [Description("G#(0)")]
        GSharp0,

        [Description("A(0)")]
        A0,

        [Description("A#(0)")]
        ASharp0,

        [Description("B(0)")]
        B0,

        [Description("C(1)")]
        C1,

        [Description("C#(1)")]
        CSharp1,

        [Description("D(1)")]
        D1,

        [Description("D#(1)")]
        DSharp1,

        [Description("E(1)")]
        E1,

        [Description("F(1)")]
        F1,

        [Description("F#(1)")]
        FSharp1,

        [Description("G(1)")]
        G1,

        [Description("G#(1)")]
        GSharp1,

        [Description("A(1)")]
        A1,

        [Description("A#(1)")]
        ASharp1,

        [Description("B(1)")]
        B1,

        [Description("C(2)")]
        C2,

        [Description("C#(2)")]
        CSharp2,

        [Description("D(2)")]
        D2,

        [Description("D#(2)")]
        DSharp2,

        [Description("E(2)")]
        E2,

        [Description("F(2)")]
        F2,

        [Description("F#(2)")]
        FSharp2,

        [Description("G(2)")]
        G2,

        [Description("G#(2)")]
        GSharp2,

        [Description("A(2)")]
        A2,

        [Description("A#(2)")]
        ASharp2,

        [Description("B(2)")]
        B2,

        [Description("C(3)")]
        C3,

        [Description("C#(3)")]
        CSharp3,

        [Description("D(3)")]
        D3,

        [Description("D#(3)")]
        DSharp3,

        [Description("E(3)")]
        E3,

        [Description("F(3)")]
        F3,

        [Description("F#(3)")]
        FSharp3,

        [Description("G(3)")]
        G3,

        [Description("G#(3)")]
        GSharp3,

        [Description("A(3)")]
        A3,

        [Description("A#(3)")]
        ASharp3,

        [Description("B(3)")]
        B3,

        [Description("C(4)")]
        C4,

        [Description("C#(4)")]
        CSharp4,

        [Description("D(4)")]
        D4,

        [Description("D#(4)")]
        DSharp4,

        [Description("E(4)")]
        E4,

        [Description("F(4)")]
        F4,

        [Description("F#(4)")]
        FSharp4,

        [Description("G(4)")]
        G4,

        [Description("G#(4)")]
        GSharp4,

        [Description("A(4)")]
        A4,

        [Description("A#(4)")]
        ASharp4,

        [Description("B(4)")]
        B4,

        [Description("C(5)")]
        C5,

        [Description("C#(5)")]
        CSharp5,

        [Description("D(5)")]
        D5,

        [Description("D#(5)")]
        DSharp5,

        [Description("E(5)")]
        E5,

        [Description("F(5)")]
        F5,

        [Description("F#(5)")]
        FSharp5,

        [Description("G(5)")]
        G5,

        [Description("G#(5)")]
        GSharp5,

        [Description("A(5)")]
        A5,

        [Description("A#(5)")]
        ASharp5,

        [Description("B(5)")]
        B5,

        [Description("C(6)")]
        C6,

        [Description("C#(6)")]
        CSharp6,

        [Description("D(6)")]
        D6,

        [Description("D#(6)")]
        DSharp6,

        [Description("E(6)")]
        E6,

        [Description("F(6)")]
        F6,

        [Description("F#(6)")]
        FSharp6,

        [Description("G(6)")]
        G6,

        [Description("G#(6)")]
        GSharp6,

        [Description("A(6)")]
        A6,

        [Description("A#(6)")]
        ASharp6,

        [Description("B(6)")]
        B6,

        [Description("C(7)")]
        C7,

        [Description("C#(7)")]
        CSharp7,

        [Description("D(7)")]
        D7,

        [Description("D#(7)")]
        DSharp7,

        [Description("E(7)")]
        E7,

        [Description("F(7)")]
        F7,

        [Description("F#(7)")]
        FSharp7,

        [Description("G(7)")]
        G7,

        [Description("G#(7)")]
        GSharp7,

        [Description("A(7)")]
        A7,

        [Description("A#(7)")]
        ASharp7,

        [Description("B(7)")]
        B7,

        [Description("C(8)")]
        C8,

        [Description("C#(8)")]
        CSharp8,

        [Description("D(8)")]
        D8,

        [Description("D#(8)")]
        DSharp8,

        [Description("E(8)")]
        E8,

        [Description("F(8)")]
        F8,

        [Description("F#(8)")]
        FSharp8,

        [Description("G(8)")]
        G8,

        [Description("G#(8)")]
        GSharp8,

        [Description("A(8)")]
        A8,

        [Description("A#(0)")]
        ASharp8,

        [Description("B(8)")]
        B8,

        [Description("C(9)")]
        C9,

        [Description("C#(9)")]
        CSharp9,

        [Description("D(9)")]
        D9,

        [Description("D#(9)")]
        DSharp9,

        [Description("E(9)")]
        E9,

        [Description("F(9)")]
        F9,

        [Description("F#(9)")]
        FSharp9,

        [Description("G9")]
        G9
    }
}
