using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Standards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Provides a high level interface for a <a href="https://bit.ly/3bRVzQT"> SMF (Standard MIDI File) </a> 
    /// 
    /// </summary>
    internal class DryWetMidiAdapter : IMidiFile
    {
        #region Adapter Specific Private Data Members 

        private readonly MidiFile _midiFile;

        private readonly IList<TrackChunk> _trackChunks;

        private readonly TrackChunk _metadataTrack;

        private readonly List<MidiEvent> _metaEvents;

        /// <summary> Medium for playing the MIDI files events on an output device. </summary>
        private Playback _midiPlayBack;

        #endregion

        #region IMidiFile Interface Properties 

        public string FilePath { get; set; }

        public string Title
        {
            get
            {
                return (((from e in _metaEvents
                          where e.EventType == MidiEventType.SequenceTrackName
                          select e).First()) as BaseTextEvent)?.Text;
                //return ((BaseTextEvent)titleEvent)?.Text

            }
            set { Title = value; }
        }

        public string KeySignature
        {
            get
            {
                // rhythm time key signature & BPM 
                var timeSignatureEvent = (from e in _metaEvents
                                          where e.EventType == MidiEventType.TimeSignature
                                          select e)?.First() as TimeSignatureEvent;
                return $"{timeSignatureEvent.Numerator}/{timeSignatureEvent.Denominator}";
            }
        }


        public int BeatsPerMinute
        {
            get => (int)(_midiFile.GetTempoMap().Tempo.AtTime(0).BeatsPerMinute);
            set => BeatsPerMinute = value;
        }

        public int NumberOfBars
        {
            get
            {
                var duration = _midiFile.GetDuration<BarBeatFractionTimeSpan>();
                var timeSignatureEvent = (from e in _metaEvents
                                          where e.EventType == MidiEventType.TimeSignature
                                          select e)?.First() as TimeSignatureEvent;
                return (int)duration.Bars + (int)Math.Ceiling(duration.Beats / timeSignatureEvent.Numerator);
            }
        }
        public ICollection<IMidiTrack> Tracks
        {
            get ;
            set ;
        }


        #endregion

        #region Consturctor 
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="midiFilePath"></param>
        public DryWetMidiAdapter(string midiFilePath)
        {
            this.FilePath = midiFilePath;
            this._midiFile = MidiFile.Read(midiFilePath);
            this._trackChunks = _midiFile.GetTrackChunks().ToList();

            // add midi tracks
            this.Tracks = new List<IMidiTrack>();
            foreach (var track in _trackChunks)
            {
                Tracks.Add(new DryWetMidiTrackAdapter(track));
            }
            this._metadataTrack = _trackChunks.First();
            this._metaEvents = _metadataTrack.Events.ToList();
        }

        #endregion

        #region Public Methods 
        public void Play()
        {
            // Play the Output
            using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
            using (var MidiPlayBack = _midiFile.GetPlayback(outputDevice))
            {
                MidiPlayBack.Speed = 1.0;
                MidiPlayBack.Play();
            }
        }

        public void Stop()
        {
            _midiPlayBack?.Stop();
        }

        #endregion


        #region DryWetMidi Midi Track Adapter - Internal Class
        // TODO: add setters to handle midi events in the individual chunks
        private class DryWetMidiTrackAdapter : IMidiTrack
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
        #endregion
    }
}

