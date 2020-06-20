using CW.Soloist.CompositionService;
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

    /// <inheritdoc cref="ICompositionParamsDTO"/>
    public class CompositionParamsDTO : ICompositionParamsDTO
    {
        /// <inheritdoc cref="ICompositionParamsDTO.CompositionStrategy"/>
        public CompositionStrategy CompositionStrategy { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.OverallFeel"/>
        public OverallNoteDurationFeel OverallFeel { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.MusicalInstrument"/>
        public MusicalInstrument MusicalInstrument { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.PitchRangeSource"/>
        public PitchRangeSource PitchRangeSource { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.MinPitch"/>
        public NotePitch MinPitch { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.MaxPitch"/>
        public NotePitch MaxPitch { get; set; }

        /// <inheritdoc cref="ICompositionParamsDTO.UseExistingMelodyAsSeed"/>
        public bool UseExistingMelodyAsSeed { get; set; }


    }
}
