using System;
using System.Collections.Generic;
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
        A,
        /// <summary> Ab ("A Flat - La Bemol")  </summary>
        AFlat,
        /// <summary> A# ("A Sharp - La Diez")  </summary>
        ASharp,
        /// <summary> B ("Si")  </summary>
        B,
        /// <summary> Bb ("B Flat - Si Bemol")  </summary>
        BFlat,
        /// <summary> B# ("B Sharp - Si Diez")  </summary>
        BSharp,
        /// <summary> C ("Do")  </summary>
        C,
        /// <summary> Cb ("C Flat - Do Bemol")  </summary>
        CFlat,
        /// <summary> C# ("C Sharp - Do Diez")  </summary>
        CSharp,
        /// <summary> D ("Re")  </summary>
        D,
        /// <summary> Db ("D Flat - Re Bemol")  </summary>
        DFlat,
        /// <summary> D# ("D Sharp - Re Diez")  </summary>
        DSharp,
        /// <summary> E ("Mi")  </summary>
        E,
        /// <summary> Eb ("E Flat - Mi Bemol")  </summary>
        EFlat,
        /// <summary> E# ("E Sharp - Mi Diez")  </summary>
        ESharp,
        /// <summary> F ("Fa")  </summary>
        F,
        /// <summary> Fb ("F Flat - Fa Bemol")  </summary>
        FFlat,
        /// <summary> F# ("F Sharp - Fa Diez")  </summary>
        FSharp,
        /// <summary> G ("Sol")  </summary>
        G,
        /// <summary> Gb ("G Flat - Sol Bemol")  </summary>
        GFlat,
        /// <summary> G# ("G Sharp - Sol Diez")  </summary>
        GSharp,
    }
}
