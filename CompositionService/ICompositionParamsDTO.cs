using CW.Soloist.CompositionService.Midi;
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

        /// <summary> Pitch low bound for the composition. </summary>
        byte MinPitch { get; set; }

        /// <summary> Pitch upper bound for the composition. </summary>
        byte MaxPitch { get; set; }

    }
}
