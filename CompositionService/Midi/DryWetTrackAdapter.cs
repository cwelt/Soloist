using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    // TODO: add setters to handle midi events in the individual chunks
    internal class DryWetMidiTrackAdapter : IMidiTrack
    {
        public string TrackName { get; }
        public byte InstrumentMidiCode { get; }
        public string InstrumentName { get; }


        internal DryWetMidiTrackAdapter(TrackChunk track)
        {
            // track name 
            TrackName = (from e in track.Events
                         where e.EventType == MidiEventType.SequenceTrackName
                         select ((SequenceTrackNameEvent)e)).FirstOrDefault()?.Text;

            // instrument 
            var programChangeEvent = from e in track.Events
                                     where e.EventType == MidiEventType.ProgramChange
                                     select ((ProgramChangeEvent)e);

            if (programChangeEvent != null && programChangeEvent.Count() > 0)
            {
                InstrumentMidiCode = (byte)(programChangeEvent.First().ProgramNumber);
                if (InstrumentMidiCode > 0 && InstrumentMidiCode <= 127)
                    InstrumentName = Enum.GetName(typeof(GeneralMidiProgram), InstrumentMidiCode);
                else InstrumentName = "Drums";
            }
            else
            {
                InstrumentName = "Drums & Percussion";
            }
        }
    }
}
