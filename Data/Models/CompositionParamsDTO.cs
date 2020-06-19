using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.Data.Models
{

    public class CompositionParamsDTO : ICompositionParamsDTO
    {
        /// <inheritdoc cref="ICompositionParamsDTO.MusicalInstrument"/>
        public MusicalInstrument MusicalInstrument { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.PitchRangeSource"/>
        public PitchRangeSource PitchRangeSource { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.MinPitch"/>
        public byte MinPitch { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.MaxPitch"/>
        public byte MaxPitch { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.ExistingMelodyTrackIndex"/>
        public int? ExistingMelodyTrackIndex { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.UseExistingMelodyAsSeed"/>
        public bool UseExistingMelodyAsSeed { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.OverallFeel"/>
        public OverallNoteDurationFeel OverallFeel { get; set; }
    }
}
