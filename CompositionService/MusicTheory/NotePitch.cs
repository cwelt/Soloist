﻿using System;
using System.Collections.Generic;
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
    internal enum NotePitch
    {
        /// <summary> Represents a hold note, i.e., a note that holds the pitch of it's predecessor note. </summary>
        HoldNote = 128,

        /// <summary> Represents a hold note (i.e., silent note).  </summary>
        RestNote = -1,

        // all the midi notes from 0 to 127 
        CMinus1,
        CSharpMinus1,
        DMinus1,
        DSharpMinus1,
        EMinus1,
        FMinus1,
        FSharpMinus1,
        GMinus1,
        GSharpMinus1,
        AMinus1,
        ASharpMinus1,
        BMinus1,
        C0,
        CSharp0,
        D0,
        DSharp0,
        E0,
        F0,
        FSharp0,
        G0,
        GSharp0,
        A0,
        ASharp0,
        B0,
        C1,
        CSharp1,
        D1,
        DSharp1,
        E1,
        F1,
        FSharp1,
        G1,
        GSharp1,
        A1,
        ASharp1,
        B1,
        C2,
        CSharp2,
        D2,
        DSharp2,
        E2,
        F2,
        FSharp2,
        G2,
        GSharp2,
        A2,
        ASharp2,
        B2,
        C3,
        CSharp3,
        D3,
        DSharp3,
        E3,
        F3,
        FSharp3,
        G3,
        GSharp3,
        A3,
        ASharp3,
        B3,
        C4,
        CSharp4,
        D4,
        DSharp4,
        E4,
        F4,
        FSharp4,
        G4,
        GSharp4,
        A4,
        ASharp4,
        B4,
        C5,
        CSharp5,
        D5,
        DSharp5,
        E5,
        F5,
        FSharp5,
        G5,
        GSharp5,
        A5,
        ASharp5,
        B5,
        C6,
        CSharp6,
        D6,
        DSharp6,
        E6,
        F6,
        FSharp6,
        G6,
        GSharp6,
        A6,
        ASharp6,
        B6,
        C7,
        CSharp7,
        D7,
        DSharp7,
        E7,
        F7,
        FSharp7,
        G7,
        GSharp7,
        A7,
        ASharp7,
        B7,
        C8,
        CSharp8,
        D8,
        DSharp8,
        E8,
        F8,
        FSharp8,
        G8,
        GSharp8,
        A8,
        ASharp8,
        B8,
        C9,
        CSharp9,
        D9,
        DSharp9,
        E9,
        F9,
        FSharp9,
        G9
    }
}
