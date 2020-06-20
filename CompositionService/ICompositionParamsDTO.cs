using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
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
        /// <summary> Algorithm for carrying out the actual composition work. </summary>
        CompositionStrategy CompositionStrategy { get; set; }


        /// <summary> Musical instrument for the melody track. </summary>
        MusicalInstrument MusicalInstrument { get; set; }

        /// <summary> Source of pitch range determination. </summary>
        PitchRangeSource PitchRangeSource { get; set; }

        /// <summary> Pitch low bound for the composition. </summary>
        NotePitch MinPitch { get; set; }

        /// <summary> Pitch upper bound for the composition. </summary>
        NotePitch MaxPitch { get; set; }

        /// <summary> 
        /// Indicator for determining wheter the existing melody should serve as
        /// a seed for the composition or should it be disregarded.  
        /// <para>
        /// if the compositio's midi file is a pure playback and contains no melody track
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
