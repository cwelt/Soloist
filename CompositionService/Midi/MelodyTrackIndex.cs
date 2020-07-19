using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Represents the ordinal index number of the main melody track
    /// in a midi file, if such a track exists in the file (i.e., if 
    /// it is not a pure playback file).
    /// </summary>
    public enum MelodyTrackIndex
    {
        /// <summary> Pure playback with no melody tracks. </summary>
        [Display(Name = "This File Does Not Contain a Melody Track")]
        NoMelodyTrackInFile,

        /// <summary> Melody track is the 1st track on midi file. </summary>
        [Display(Name = "Track Number 1")]
        TrackNumber1,

        /// <summary> Melody track is the 2nd track on midi file. </summary>
        [Display(Name = "Track Number 2")]
        TrackNumber2,

        /// <summary> Melody track is the 3rd track on midi file. </summary>
        [Display(Name = "Track Number 3")]
        TrackNumber3,

        /// <summary> Melody track is the 4th track on midi file. </summary>
        [Display(Name = "Track Number 4")]
        TrackNumber4,

        /// <summary> Melody track is the 5th track on midi file. </summary>
        [Display(Name = "Track Number 5")]
        TrackNumber5,

        /// <summary> Melody track is the 6th track on midi file. </summary>
        [Display(Name = "Track Number 6")]
        TrackNumber6,

        /// <summary> Melody track is the 7th track on midi file. </summary>
        [Display(Name = "Track Number 7")]
        TrackNumber7,

        /// <summary> Melody track is the 8th track on midi file. </summary>
        [Display(Name = "Track Number 8")]
        TrackNumber8,

        /// <summary> Melody track is the 9th track on midi file. </summary>
        [Display(Name = "Track Number 9")]
        TrackNumber9,

        /// <summary> Melody track is the 10th track on midi file. </summary>
        [Display(Name = "Track Number 10")]
        TrackNumber10,

        /// <summary> Melody track is the 11th track on midi file. </summary>
        [Display(Name = "Track Number 11")]
        TrackNumber11,

        /// <summary> Melody track is the 12th track on midi file. </summary>
        [Display(Name = "Track Number 12")]
        TrackNumber12,

        /// <summary> Melody track is the 13th track on midi file. </summary>
        [Display(Name = "Track Number 13")]
        TrackNumber13,

        /// <summary> Melody track is the 14th track on midi file. </summary>
        [Display(Name = "Track Number 14")]
        TrackNumber14,

        /// <summary> Melody track is the 15th track on midi file. </summary>
        [Display(Name = "Track Number 15")]
        TrackNumber15,

        /// <summary> Melody track is the 16th track on midi file. </summary>
        [Display(Name = "Track Number 16")]
        TrackNumber16
    }
}