using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary> Represets one of the 12-tonal note names. </summary>
    /// <remarks> The enumeration values support both sharp notes and flat notes. </remarks>
    public enum NoteName
    {
        /// <summary> A ("La")  </summary>
        [Description("A - La")]
        A,

        /// <summary> Ab ("A Flat - La Bemol")  </summary>
        [Description("Ab - La Bemol")]
        AFlat,

        /// <summary> A# ("A Sharp - La Diez")  </summary>
        [Description("A# - La Diez")]
        ASharp,

        /// <summary> B ("Si")  </summary>
        [Description("B - Si")]
        B,

        /// <summary> Bb ("B Flat - Si Bemol")  </summary>
        [Description("Bb - Si Bemol")]
        BFlat,

        /// <summary> B# ("B Sharp - Si Diez")  </summary>
        [Description("B# - Si Diez")]
        BSharp,

        /// <summary> C ("Do")  </summary>
        [Description("C - Do")]
        C,

        /// <summary> Cb ("C Flat - Do Bemol")  </summary>
        [Description("Cb - Do Bemol")]
        CFlat,

        /// <summary> C# ("C Sharp - Do Diez")  </summary>
        [Description("C# - Do Diez")]
        CSharp,

        /// <summary> D ("Re")  </summary>
        [Description("D - Re")]
        D,

        /// <summary> Db ("D Flat - Re Bemol")  </summary>
        [Description("Db - Re Bemol")]
        DFlat,

        /// <summary> D# ("D Sharp - Re Diez")  </summary>
        [Description("D# - Re Diez")]
        DSharp,

        /// <summary> E ("Mi")  </summary>
        [Description("E - Mi")]
        E,

        /// <summary> Eb ("E Flat - Mi Bemol")  </summary>
        [Description("Eb - Mi Bemol")]
        EFlat,

        /// <summary> E# ("E Sharp - Mi Diez")  </summary>
        [Description("E# - Mi Diez")]
        ESharp,

        /// <summary> F ("Fa")  </summary>
        [Description("F - Fa")]
        F,

        /// <summary> Fb ("F Flat - Fa Bemol")  </summary>
        [Description("Fb - Fa Bemol")]
        FFlat,

        /// <summary> F# ("F Sharp - Fa Diez")  </summary>
        [Description("F# - Fa Diez")]
        FSharp,

        /// <summary> G ("Sol")  </summary>
        [Description("G - Sol")]
        G,

        /// <summary> Gb ("G Flat - Sol Bemol")  </summary>
        [Description("Gb - Sol Bemol")]
        GFlat,

        /// <summary> G# ("G Sharp - Sol Diez")  </summary>
        [Description("G# - Sol Diez")]
        GSharp,
    }
}
