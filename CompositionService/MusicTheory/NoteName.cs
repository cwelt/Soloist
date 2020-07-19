using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "A - La")]
        A,

        /// <summary> Ab ("A Flat - La Bemol")  </summary>
        [Display(Name = "Ab - La Bemol")]
        AFlat,

        /// <summary> A# ("A Sharp - La Diez")  </summary>
        [Display(Name = "A# - La Diez")]
        ASharp,

        /// <summary> B ("Si")  </summary>
        [Display(Name = "B - Si")]
        B,

        /// <summary> Bb ("B Flat - Si Bemol")  </summary>
        [Display(Name = "Bb - Si Bemol")]
        BFlat,

        /// <summary> B# ("B Sharp - Si Diez")  </summary>
        [Display(Name = "B# - Si Diez")]
        BSharp,

        /// <summary> C ("Do")  </summary>
        [Display(Name = "C - Do")]
        C,

        /// <summary> Cb ("C Flat - Do Bemol")  </summary>
        [Display(Name = "Cb - Do Bemol")]
        CFlat,

        /// <summary> C# ("C Sharp - Do Diez")  </summary>
        [Display(Name = "C# - Do Diez")]
        CSharp,

        /// <summary> D ("Re")  </summary>
        [Display(Name = "D - Re")]
        D,

        /// <summary> Db ("D Flat - Re Bemol")  </summary>
        [Display(Name = "Db - Re Bemol")]
        DFlat,

        /// <summary> D# ("D Sharp - Re Diez")  </summary>
        [Display(Name = "D# - Re Diez")]
        DSharp,

        /// <summary> E ("Mi")  </summary>
        [Display(Name = "E - Mi")]
        E,

        /// <summary> Eb ("E Flat - Mi Bemol")  </summary>
        [Display(Name = "Eb - Mi Bemol")]
        EFlat,

        /// <summary> E# ("E Sharp - Mi Diez")  </summary>
        [Display(Name = "E# - Mi Diez")]
        ESharp,

        /// <summary> F ("Fa")  </summary>
        [Display(Name = "F - Fa")]
        F,

        /// <summary> Fb ("F Flat - Fa Bemol")  </summary>
        [Display(Name = "Fb - Fa Bemol")]
        FFlat,

        /// <summary> F# ("F Sharp - Fa Diez")  </summary>
        [Display(Name = "F# - Fa Diez")]
        FSharp,

        /// <summary> G ("Sol")  </summary>
        [Display(Name = "G - Sol")]
        G,

        /// <summary> Gb ("G Flat - Sol Bemol")  </summary>
        [Display(Name = "Gb - Sol Bemol")]
        GFlat,

        /// <summary> G# ("G Sharp - Sol Diez")  </summary>
        [Display(Name = "G# - Sol Diez")]
        GSharp,
    }
}
