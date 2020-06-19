using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoloistWebClient.ViewModels
{
    public class ComposeFormViewModel
    {
        public String MidiFile { get; set; }
        public String ChordProgressionFile { get; set; }
        public MusicalInstrument MusicalInstrument { get; set; }

        public PitchRangeSource PitchRangeSource { get; set; }

        public byte MinPitch { get; set; }

        public byte MaxPitch { get; set; }

        public int? ExistingMelodyTrackIndex { get; set; }

        public bool UseExistingMelodyAsSeed { get; set; }

        public OverallNoteDurationFeel OverallFeel { get; set; }
    }
}