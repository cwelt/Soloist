using CW.Soloist.CompositionService.MusicTheory;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Standards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Note = CW.Soloist.CompositionService.MusicTheory.Note;

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

        private readonly TempoMap _tempoMap;

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

/*        public MelodyGenome(TrackChunk seed, MidiFile midiFile, List<IBar> chordProgression = null)
        {
            this.Bars = Utilities.EncodeMelody(seed, midiFile).ToArray();
            if (chordProgression != null)
                for (int i = 0; i < this.Bars.Count; i++)
                    Bars.ElementAt(i).Chords = chordProgression.ElementAt(i).Chords;
        }*/

        /// <summary>
        /// Converts a melody encoded in list of <see cref="IBar"/> to a 
        /// <see cref="TrackChunk"/> and adds the created track chunk to this midi's 
        /// files chunk list. 
        /// 
        /// TODO:
        ///     set instrument parameter, 
        ///     overload with track index
        ///     ...
        ///     
        /// </summary>
        /// 
        /// <param name="melody"></param>
        /// <param name="trackName"></param>
        /// <param name="instrument"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        internal TrackChunk ConvertMelodyToTrackChunk(IEnumerable<IBar> melody, string trackName = "Generated Melody", byte instrument = 24, byte channel = 15)
        {
            PatternBuilder melodyTrackBuilder = new PatternBuilder();
            melodyTrackBuilder.ProgramChange(GeneralMidiProgram.AcousticGuitar1); // set instrument  
            melodyTrackBuilder.SetVelocity((SevenBitNumber)100);
            foreach (IBar bar in melody)
            {
                foreach (INote note in bar.Notes)
                {
                    // set new note's length 
                    var duration = new MusicalTimeSpan(note.Duration.Numerator, note.Duration.Denominator);

                    // if this is a rest note, just step forward and continue to next note
                    if (note.Pitch == NotePitch.RestNote)
                        melodyTrackBuilder.StepForward(duration);
                    else
                    {
                        // create new note:
                        var newNote = Melanchall.DryWetMidi.MusicTheory.Note.Get((SevenBitNumber)(int)note.Pitch);

                        // add new note with pre-seted length to midi chunk track: 
                        melodyTrackBuilder.Note(newNote, duration);
                    }
                }
            }

            Pattern pattern = melodyTrackBuilder.Build();
            var melodyTrackChunk = pattern.ToTrackChunk(this._tempoMap, (FourBitNumber)channel);
            SequenceTrackNameEvent newTrackName = new SequenceTrackNameEvent(trackName);
            melodyTrackChunk.Events.Add(newTrackName);
            return melodyTrackChunk;
        }

        #region EncodeMelody
        // EncodeMelody
        internal IEnumerable<IBar> EncodeMelody(TrackChunk melodyTrack)
        {
            List<MusicTheory.INote> notes = new List<Note>().Cast<INote>().ToList(); ;
            List<IBar> bars = new List<IBar>();
            IBar currentBar = new Bar();
            int barCounter = 0;
            Melanchall.DryWetMidi.Interaction.Note note;
            short pitch;
            TempoMap tempoMap = _midiFile.GetTempoMap();
            long startBar, endBar;


            var notesAndRests = melodyTrack.GetNotesAndRests(RestSeparationPolicy.NoSeparation);

            foreach (var currentNote in notesAndRests)
            {
                MusicalTimeSpan length = currentNote.LengthAs<MusicalTimeSpan>(tempoMap);

                var musicEndTime = currentNote.EndTimeAs<MusicalTimeSpan>(tempoMap);
                var barBeatFractionEndTime = currentNote.EndTimeAs<BarBeatFractionTimeSpan>(tempoMap);
                var barBeatFractionStartTime = currentNote.TimeAs<BarBeatFractionTimeSpan>(tempoMap);
                startBar = barBeatFractionStartTime.Bars;
                endBar = barBeatFractionEndTime.Bars;

                note = currentNote as Melanchall.DryWetMidi.Interaction.Note;
                if (note != null)
                    pitch = note.NoteNumber;
                else pitch = -1;

                INote newNote = new Note((NotePitch)pitch, (byte)length.Numerator, (byte)length.Denominator);
                notes.Add(newNote);

                //add note to bar 
                if (barBeatFractionStartTime.Bars > barCounter)
                {
                    bars.Add(currentBar);
                    currentBar = new Bar();
                    currentBar.Notes.Add(newNote);
                    barCounter++;
                }

                currentBar.Notes.Add(newNote);

                if (barBeatFractionEndTime.Bars > barCounter)
                {
                    bars.Add(currentBar);
                    currentBar = new Bar();
                    barCounter++;
                }
            }
            return bars;
        }
        #endregion

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

