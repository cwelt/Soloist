using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService
{
    /// <summary>
    /// Data transfer object interface for gathering all required 
    /// parameters for delivering a request for a musical composition. 
    /// </summary>
    public interface ICompositionParamsDTO
    {
        /// <summary> Musical instrument for the melody track. </summary>
        MusicalInstrument MusicalInstrument { get; set; }

        /// <summary> Source of pitch range determination. </summary>
        PitchRangeSource PitchRangeSource { get; set; }

        /// <summary> Pitch low bound for the composition. </summary>
        byte MinPitch { get; set; }

        /// <summary> Pitch upper bound for the composition. </summary>
        byte MaxPitch { get; set; }

        /// <summary> 
        /// Index of the existing melody track in the midi file, if one exists. 
        /// <para>
        /// This track would be replaced by the new composed 
        /// melody track. If the current midi file is a pure playback 
        /// that contains no existing melody track, then this property
        /// should be set to null. 
        /// </para>
        /// </summary>
        int? ExistingMelodyTrackIndex { get; set; }

        /// <summary> 
        /// Indicator for determining wheter the existing melody should serve as
        /// a seed for the composition or should it be disregarded.  
        /// <para>
        /// If <see cref="ExistingMelodyTrackIndex"/> is set to null, i.e., 
        /// if the midi file is a pure playback and contains no melody track
        /// whatsoever, then this property would be disregarded.
        /// </para>
        /// </summary>
        bool UseExistingMelodyAsSeed { get; set; }

        /// <summary>
        /// Determines the overall feel and density regarding the amount of notes
        /// in the composed melody. 
        /// </summary>
        OverallNoteDurationFeel OverallFeel { get; set; }
    }
}
