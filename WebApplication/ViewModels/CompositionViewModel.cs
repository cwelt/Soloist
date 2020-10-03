using System.Web.Mvc;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.Enums;
using CW.Soloist.WebApplication.Validations;
using CW.Soloist.CompositionService.Composers;
using CW.Soloist.CompositionService.MusicTheory;

namespace CW.Soloist.WebApplication.ViewModels
{
    public class CompositionViewModel
    {
        [Required]
        [DisplayName("Overall Note Duration Feel")]
        public OverallNoteDurationFeel OverallNoteDurationFeel { get; set; }

        [Required]
        [DisplayName("Musical Instrument")]
        public MusicalInstrument MusicalInstrument { get; set; }

        [DisplayName("Pitch Lowest Bound")]
        public NotePitch MinPitch { get; set; }

        [PitchRangeValidation]
        [DisplayName("Pitch Upper Bound")]
        public NotePitch MaxPitch { get; set; }

        public IEnumerable<SelectListItem> PitchSelectList { get; set; }


        [DisplayName("Composition Algorithm Strategy")]
        public CompositionStrategy CompositionStrategy { get; set; }

        [DisplayName("Use Existing Melody as Seed")]
        public bool useExistingMelodyAsSeed { get; set; }

        [DisplayName("Song")]
        public int SongId { get; set; }

        public SelectList SongSelectList { get; set; }

        [Range(0, 100)]
        [DisplayName("Smooth Note Connectivity:")]
        public double SmoothMovement { get; set; } = 15;

        [Range(0, 100)]
        [DisplayName("Mitigate Extreme Intervals:")]
        public double ExtremeIntervals { get; set; } = 10;


        [Range(0, 100)]
        [DisplayName("Rich Pitch Variety:")]
        public double PitchVariety { get; set; } = 15;

        [Range(0, 100)]
        [DisplayName("Wide Pitch Range:")]
        public double PitchRange { get; set; } = 5;

        [Range(0, 100)]
        [DisplayName("Melody Contour Direction:")]
        public double ContourDirection { get; set; } = 5;

        [Range(0, 100)]
        [DisplayName("Melody Contour Stability:")]
        public double ContourStability { get; set; } = 10;

        [Range(0, 100)]
        [DisplayName("Syncopation Usage:")]
        public double Syncopation { get; set; } = 10; // 40

        [Range(0, 100)]
        [DisplayName("Note Density Balance:")]
        public double DensityBalance { get; set; } = 15;

        [Range(0, 100)]
        [DisplayName("Chord Notes On Accented Beats:")]
        public double AccentedBeats { get; set; } = 15; // 50 

        /// <summary> Sums up all the proportional evaluators weights. </summary>
        public double WeightSum => 
                AccentedBeats + ContourDirection + ContourStability +
                DensityBalance + ExtremeIntervals + PitchRange +
                PitchVariety + SmoothMovement + Syncopation;
    }
}