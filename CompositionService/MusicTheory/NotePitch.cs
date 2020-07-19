using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Hold Note")]
        HoldNote = 128,

        /// <summary> Represents a hold note (i.e., silent note).  </summary>
        [Display(Name = "Rest Note")]
        RestNote = -1,

        /// <summary> Note: C; Octave: -1; Midi Number: 0.</summary>
        [Display(Name = "C-1: 0")]
        CMinus1,

        /// <summary> Note: C#; Octave: -1; Midi Number: 1.</summary>
        [Display(Name = "C#-1: 1")]
        CSharpMinus1,

        /// <summary> Note: D; Octave: -1; Midi Number: 2.</summary>
        [Display(Name = "D-1: 2")]
        DMinus1,


        /// <summary> Note: D#; Octave: -1; Midi Number: 3.</summary>
        [Display(Name = "D#-1: 3")]
        DSharpMinus1,

        /// <summary> Note: E; Octave: -1; Midi Number: 4.</summary>
        [Display(Name = "E-1: 4")]
        EMinus1,

        /// <summary> Note: F; Octave: -1; Midi Number: 5.</summary>
        [Display(Name = "F-1: 5")]
        FMinus1,

        /// <summary> Note: F#; Octave: -1; Midi Number: 6.</summary>
        [Display(Name = "F#-1: 6")]
        FSharpMinus1,

        /// <summary> Note: G; Octave: -1; Midi Number: 7.</summary>
        [Display(Name = "G-1: 7")]
        GMinus1,

        /// <summary> Note: G#; Octave: -1; Midi Number: 8.</summary>
        [Display(Name = "G#-1: 8")]
        GSharpMinus1,

        /// <summary> Note: A; Octave: -1; Midi Number: 9.</summary>
        [Display(Name = "A-1: 9")]
        AMinus1,


        /// <summary> Note: A#; Octave: -1; Midi Number: 10.</summary>
        [Display(Name = "A#-1: 10")]
        ASharpMinus1,

        /// <summary> Note: B; Octave: -1; Midi Number: 11.</summary>
        [Display(Name = "B-1: 11")]
        BMinus1,

        /// <summary> Note: C; Octave: 0; Midi Number: 12.</summary>
        [Display(Name = "C0: 12")]
        C0,

        /// <summary> Note: C#; Octave: 0; Midi Number: 13.</summary>
        [Display(Name = "C#0: 13")]
        CSharp0,

        /// <summary> Note: D; Octave: 0; Midi Number: 14.</summary>
        [Display(Name = "D#0: 14")]
        D0,

        /// <summary> Note: D#; Octave: 0; Midi Number: 15.</summary>
        [Display(Name = "D#0: 15")]
        DSharp0,

        /// <summary> Note: E; Octave: 0; Midi Number: 16.</summary>
        [Display(Name = "E0: 16")]
        E0,

        /// <summary> Note: F; Octave: 0; Midi Number: 17.</summary>
        [Display(Name = "F0: 17")]
        F0,

        /// <summary> Note: F#; Octave: 0; Midi Number: 18.</summary>
        [Display(Name = "F#0: 18")]
        FSharp0,

        /// <summary> Note: G; Octave: 0; Midi Number: 19.</summary>
        [Display(Name = "G0 : 19")]
        G0,

        /// <summary> Note: G#; Octave: 0; Midi Number: 20.</summary>
        [Display(Name = "G#(0): 20")]
        GSharp0,

        /// <summary> Note: A; Octave: 0; Midi Number: 21.</summary>
        [Display(Name = "A0: 21")]
        A0,

        /// <summary> Note: A#; Octave: 0; Midi Number: 22.</summary>
        [Display(Name = "A#0: 22")]
        ASharp0,

        /// <summary> Note: B; Octave: 0; Midi Number: 23.</summary>
        [Display(Name = "B0: 23")]
        B0,

        /// <summary> Note: C; Octave: 1; Midi Number: 24.</summary>
        [Display(Name = "C1: 24")]
        C1,

        /// <summary> Note: C#; Octave: 1; Midi Number: 25.</summary>
        [Display(Name = "C#1: 25")]
        CSharp1,

        /// <summary> Note: D; Octave: 1; Midi Number: 26.</summary>
        [Display(Name = "D(1): 26")]
        D1,

        /// <summary> Note: D#; Octave: 1; Midi Number: 27.</summary>
        [Display(Name = "D#1: 27")]
        DSharp1,

        /// <summary> Note: E; Octave: 1; Midi Number: 28.</summary>
        [Display(Name = "E1: 28")]
        E1,

        /// <summary> Note: F; Octave: 1; Midi Number: 29.</summary>
        [Display(Name = "F1: 29")]
        F1,

        /// <summary> Note: F#; Octave: 1; Midi Number: 30.</summary>
        [Display(Name = "F#1: 30")]
        FSharp1,

        /// <summary> Note: G; Octave: 1; Midi Number: 31.</summary>
        [Display(Name = "G1: 31")]
        G1,

        /// <summary> Note: G#; Octave: 1; Midi Number: 32.</summary>
        [Display(Name = "G#1: 32")]
        GSharp1,

        /// <summary> Note: A; Octave: 1; Midi Number: 33.</summary>
        [Display(Name = "A1: 33")]
        A1,

        /// <summary> Note: A; Octave: 1; Midi Number: 34.</summary>
        [Display(Name = "A#1: 34")]
        ASharp1,

        /// <summary> Note: B; Octave: 1; Midi Number: 35.</summary>
        [Display(Name = "B1: 35")]
        B1,

        /// <summary> Note: C; Octave: 2; Midi Number: 36.</summary>
        [Display(Name = "C2: 36")]
        C2,

        /// <summary> Note: C#; Octave: 2; Midi Number: 37.</summary>
        [Display(Name = "C#2: 37")]
        CSharp2,

        /// <summary> Note: D; Octave: 2; Midi Number: 38.</summary>
        [Display(Name = "D2: 38")]
        D2,

        /// <summary> Note: D#; Octave: 2; Midi Number: 39.</summary>
        [Display(Name = "D#2: 39")]
        DSharp2,

        /// <summary> Note: E; Octave: 2; Midi Number: 40.</summary>
        [Display(Name = "E2: 40")]
        E2,

        /// <summary> Note: F; Octave: 2; Midi Number: 41.</summary>
        [Display(Name = "F2: 41")]
        F2,

        /// <summary> Note: F#; Octave: 2; Midi Number: 42.</summary>
        [Display(Name = "F#2: 42")]
        FSharp2,

        /// <summary> Note: G; Octave: 2; Midi Number: 43.</summary>
        [Display(Name = "G2: 43")]
        G2,

        /// <summary> Note: G#; Octave: 2; Midi Number: 44.</summary>
        [Display(Name = "G#2: 44")]
        GSharp2,

        /// <summary> Note: A; Octave: 2; Midi Number: 45.</summary>
        [Display(Name = "A2: 45")]
        A2,

        /// <summary> Note: A#; Octave: 2; Midi Number: 46.</summary>
        [Display(Name = "A#2: 46")]
        ASharp2,

        /// <summary> Note: B; Octave: 2; Midi Number: 47.</summary>
        [Display(Name = "B2: 47")]
        B2,

        /// <summary> Note: C; Octave: 3; Midi Number: 48.</summary>
        [Display(Name = "C3: 48")]
        C3,

        /// <summary> Note: C#; Octave: 3; Midi Number: 49.</summary>
        [Display(Name = "C#3: 49")]
        CSharp3,

        /// <summary> Note: D; Octave: 3; Midi Number: 50.</summary>
        [Display(Name = "D3: 50")]
        D3,

        /// <summary> Note: D#; Octave: 3; Midi Number: 51.</summary>
        [Display(Name = "D#3: 51")]
        DSharp3,

        /// <summary> Note: E; Octave: 3; Midi Number: 52.</summary>
        [Display(Name = "E3: 52")]
        E3,

        /// <summary> Note: F; Octave: 3; Midi Number: 53.</summary>
        [Display(Name = "F3: 53")]
        F3,

        /// <summary> Note: F#; Octave: 3; Midi Number: 54.</summary>
        [Display(Name = "F#3: 54")]
        FSharp3,

        /// <summary> Note: G; Octave: 3; Midi Number: 55.</summary>
        [Display(Name = "G3: 55")]
        G3,

        /// <summary> Note: G#; Octave: 3; Midi Number: 56.</summary>
        [Display(Name = "G#3: 56")]
        GSharp3,

        /// <summary> Note: A; Octave: 3; Midi Number: 57.</summary>
        [Display(Name = "A3: 57")]
        A3,

        /// <summary> Note: A#; Octave: 3; Midi Number: 58.</summary>
        [Display(Name = "A#3: 58")]
        ASharp3,

        /// <summary> Note: B; Octave: 3; Midi Number: 59.</summary>
        [Display(Name = "B3: 59")]
        B3,

        /// <summary> Note: C; Octave: 4; Midi Number: 60.</summary>
        [Display(Name = "C4: 60")]
        C4,

        /// <summary> Note: C#; Octave: 4; Midi Number: 61.</summary>
        [Display(Name = "C#4: 61")]
        CSharp4,

        /// <summary> Note: D; Octave: 4; Midi Number: 62.</summary>
        [Display(Name = "D4: 62")]
        D4,

        /// <summary> Note: D#; Octave: 4; Midi Number: 63.</summary>
        [Display(Name = "D#4: 63")]
        DSharp4,

        /// <summary> Note: E; Octave: 4; Midi Number: 64.</summary>
        [Display(Name = "E4: 64")]
        E4,

        /// <summary> Note: F; Octave: 4; Midi Number: 65.</summary>
        [Display(Name = "F4: 65")]
        F4,

        /// <summary> Note: G; Octave: 4; Midi Number: 66.</summary>
        [Display(Name = "F#4: 66")]
        FSharp4,

        /// <summary> Note: G; Octave: 4; Midi Number: 67.</summary>
        [Display(Name = "G4 : 67")]
        G4,

        /// <summary> Note: G#; Octave: 4; Midi Number: 68.</summary>
        [Display(Name = "G#4: 68")]
        GSharp4,

        /// <summary> Note: A; Octave: 4; Midi Number: 69.</summary>
        [Display(Name = "A4: 69")]
        A4,

        /// <summary> Note: A#; Octave: 4; Midi Number: 70.</summary>
        [Display(Name = "A#4: 70")]
        ASharp4,

        /// <summary> Note: B; Octave: 4; Midi Number: 71.</summary>
        [Display(Name = "B4: 71")]
        B4,

        /// <summary> Note: C; Octave: 5; Midi Number: 72.</summary>
        [Display(Name = "C5: 72")]
        C5,

        /// <summary> Note: C#; Octave: 5; Midi Number: 73.</summary>
        [Display(Name = "C#: 73")]
        CSharp5,

        /// <summary> Note: D; Octave: 5; Midi Number: 74.</summary>
        [Display(Name = "D5: 74")]
        D5,

        /// <summary> Note: D#; Octave: 5; Midi Number: 75.</summary>
        [Display(Name = "D#5: 75")]
        DSharp5,

        /// <summary> Note: E; Octave: 5; Midi Number: 76.</summary>
        [Display(Name = "E5: 76")]
        E5,

        /// <summary> Note: F; Octave: 5; Midi Number: 77.</summary>
        [Display(Name = "F5: 77")]
        F5,

        /// <summary> Note: F#; Octave: 5; Midi Number: 78.</summary>
        [Display(Name = "F#5: 78")]
        FSharp5,

        /// <summary> Note: G; Octave: 5; Midi Number: 79.</summary>
        [Display(Name = "G5: 79")]
        G5,

        /// <summary> Note: G#; Octave: 5; Midi Number: 80.</summary>
        [Display(Name = "G#5: 80")]
        GSharp5,

        /// <summary> Note: A; Octave: 5; Midi Number: 81.</summary>
        [Display(Name = "A5: 81")]
        A5,

        /// <summary> Note: A#; Octave: 5; Midi Number: 82.</summary>
        [Display(Name = "A#5: 82")]
        ASharp5,

        /// <summary> Note: B; Octave: 5; Midi Number: 83.</summary>
        [Display(Name = "B5: 83")]
        B5,

        /// <summary> Note: C; Octave: 6; Midi Number: 84.</summary>
        [Display(Name = "C6: 84")]
        C6,

        /// <summary> Note: C#; Octave: 6; Midi Number: 85.</summary>
        [Display(Name = "C#6: 85")]
        CSharp6,

        /// <summary> Note: D; Octave: 6; Midi Number: 86.</summary>
        [Display(Name = "D6: 86")]
        D6,

        /// <summary> Note: D#; Octave: 6; Midi Number: 87.</summary>
        [Display(Name = "D#6: 87")]
        DSharp6,

        /// <summary> Note: E; Octave: 6; Midi Number: 88.</summary>
        [Display(Name = "E6: 88")]
        E6,

        /// <summary> Note: F; Octave: 6; Midi Number: 89.</summary>
        [Display(Name = "F6: 89")]
        F6,

        /// <summary> Note: F#; Octave: 6; Midi Number: 90.</summary>
        [Display(Name = "F#6: 90")]
        FSharp6,

        /// <summary> Note: G; Octave: 6; Midi Number: 91.</summary>
        [Display(Name = "G6: 91")]
        G6,

        /// <summary> Note: G#; Octave: 6; Midi Number: 92.</summary>
        [Display(Name = "G#6: 92")]
        GSharp6,

        /// <summary> Note: A; Octave: 6; Midi Number: 93.</summary>
        [Display(Name = "A6: 93")]
        A6,

        /// <summary> Note: A#; Octave: 6; Midi Number: 94.</summary>
        [Display(Name = "A#6: 94")]
        ASharp6,

        /// <summary> Note: B; Octave: 6; Midi Number: 95.</summary>
        [Display(Name = "B6: 95")]
        B6,

        /// <summary> Note: C; Octave: 7; Midi Number: 96.</summary>
        [Display(Name = "C7: 96")]
        C7,

        /// <summary> Note: C#; Octave: 7; Midi Number: 97.</summary>
        [Display(Name = "C#7: 97")]
        CSharp7,

        /// <summary> Note: D; Octave: 7; Midi Number: 98.</summary>
        [Display(Name = "D7: 98")]
        D7,

        /// <summary> Note: D#; Octave: 7; Midi Number: 99.</summary>
        [Display(Name = "D#7: 99")]
        DSharp7,

        /// <summary> Note: E; Octave: 7; Midi Number: 100.</summary>
        [Display(Name = "E7: 100")]
        E7,

        /// <summary> Note: F; Octave: 7; Midi Number: 101.</summary>
        [Display(Name = "F7: 101")]
        F7,

        /// <summary> Note: F#; Octave: 7; Midi Number: 102.</summary>
        [Display(Name = "F#7: 102")]
        FSharp7,

        /// <summary> Note: G; Octave: 7; Midi Number: 103.</summary>
        [Display(Name = "G7: 103")]
        G7,

        /// <summary> Note: G#; Octave: 7; Midi Number: 104.</summary>
        [Display(Name = "G#7: 104")]
        GSharp7,

        /// <summary> Note: A; Octave: 7; Midi Number: 105.</summary>
        [Display(Name = "A7: 105")]
        A7,

        /// <summary> Note: A#; Octave: 7; Midi Number: 106.</summary>
        [Display(Name = "A#7: 106")]
        ASharp7,

        /// <summary> Note: B; Octave: 7; Midi Number: 107.</summary>
        [Display(Name = "B7: 107")]
        B7,

        /// <summary> Note: C; Octave: 8; Midi Number: 108.</summary>
        [Display(Name = "C8: 108")]
        C8,

        /// <summary> Note: C#; Octave: 8; Midi Number: 109.</summary>
        [Display(Name = "C#8: 109")]
        CSharp8,

        /// <summary> Note: D; Octave: 8; Midi Number: 110.</summary>
        [Display(Name = "D8: 110")]
        D8,

        /// <summary> Note: D#; Octave: 8; Midi Number: 111.</summary>
        [Display(Name = "D#8: 111")]
        DSharp8,

        /// <summary> Note: E; Octave: 8; Midi Number: 112.</summary>
        [Display(Name = "E8: 112")]
        E8,

        /// <summary> Note: F; Octave: 8; Midi Number: 113.</summary>
        [Display(Name = "F8: 113")]
        F8,

        /// <summary> Note: F#; Octave: 8; Midi Number: 114.</summary>
        [Display(Name = "F#8: 114")]
        FSharp8,

        /// <summary> Note: G; Octave: 8; Midi Number: 115.</summary>
        [Display(Name = "G8: 115")]
        G8,

        /// <summary> Note: G#; Octave: 8; Midi Number: 116.</summary>
        [Display(Name = "G#8: 116")]
        GSharp8,

        /// <summary> Note: A; Octave: 8; Midi Number: 117.</summary>
        [Display(Name = "A8: 117")]
        A8,

        /// <summary> Note: A#; Octave: 8; Midi Number: 118.</summary>
        [Display(Name = "A#8: 118")]
        ASharp8,

        /// <summary> Note: B; Octave: 8; Midi Number: 119.</summary>
        [Display(Name = "B8: 119")]
        B8,

        /// <summary> Note: C; Octave: 9; Midi Number: 120.</summary>
        [Display(Name = "C9: 120")]
        C9,

        /// <summary> Note: C#; Octave: 9; Midi Number: 121.</summary>
        [Display(Name = "C#9: 121")]
        CSharp9,

        /// <summary> Note: D; Octave: 9; Midi Number: 122.</summary>
        [Display(Name = "D9: 122")]
        D9,

        /// <summary> Note: D#; Octave: 9; Midi Number: 123.</summary>
        [Display(Name = "D#9: 123")]
        DSharp9,

        /// <summary> Note: E; Octave: 9; Midi Number: 124.</summary>
        [Display(Name = "E9: 124")]
        E9,

        /// <summary> Note: F; Octave: 9; Midi Number: 125.</summary>
        [Display(Name = "F9: 125")]
        F9,

        /// <summary> Note: F#; Octave: 9; Midi Number: 126.</summary>
        [Display(Name = "F#9: 126")]
        FSharp9,

        /// <summary> Note: G; Octave: 9; Midi Number: 127.</summary>
        [Display(Name = "G9: 127")]
        G9
    }
}
