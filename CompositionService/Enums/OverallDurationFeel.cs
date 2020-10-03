    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    namespace CW.Soloist.CompositionService.Enums
    {
        /// <summary>
        /// Determines the overall feel and density regarding the amount of notes
        /// in the composed melody. 
        /// <para>
        /// For exmaple, the <see cref="Slow"></see> feel would generally make the 
        /// notes duration longer, and therefore reduce the amount of notes in each bar,
        /// causing an overall effect of a slow-moderate feeling melody.
        /// On contrast, the <see cref="Intense"/> for example would generally make
        /// the notes duration much shorter, and therefore increase the amount of notes
        /// in each bar, creating a fast-phased intense solo melody.
        /// </para>
        /// </summary>
        public enum OverallNoteDurationFeel
        {
            /// <summary> Slow moderate quarter-note based feeling. </summary>
            [Display(Name = "Slow Moderate Feeling")]
            Slow = 4,

            /// <summary> Medium flowing eighth-note based feeling. </summary>
            [Display(Name = "Medium Flowing Feeling")]
            Medium = 8,

            /// <summary> Fast intense sixteenth-note based. </summary>
            [Display(Name = "Fast-Phased Intense Feeling")]
            Intense = 16,

            /// <summary> Extreme super-fast thirty-second-note based feeling. </summary>
            [Display(Name = "Extreme Speed-of-Light Feeling")]
            Extreme = 32
        }
    }
