using CW.Soloist.CompositionService.MusicTheory;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Standards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Note = CW.Soloist.CompositionService.MusicTheory.Note;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Provides a high level interface for a <a href="https://bit.ly/3bRVzQT"> SMF (Standard MIDI File) </a> 
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


        //TODO: 
        // 1. UPDATE THE SETTER TO UPDATE THE ACTUAL SequenceTrackName EVENT 
        // 2. Update Getter for a Lazy Load
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


        public byte BeatsPerMinute => (byte)(_midiFile.GetTempoMap().Tempo.AtTime(0).BeatsPerMinute);

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
            get;
            set;
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
            this._tempoMap = _midiFile.GetTempoMap();
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


        /// <inheritdoc/>
        public void EmbedMelody(IEnumerable<IBar> melody, string melodyTrackName = "Melody", byte instrumentId = 64)
        {
            TrackChunk melodyTrack = ConvertMelodyToTrackChunk(melody, melodyTrackName, instrumentId);
            this._midiFile.Chunks.Add(melodyTrack);
        }

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
        private TrackChunk ConvertMelodyToTrackChunk(IEnumerable<IBar> melody, string trackName, byte instrumentId = 64, byte channel = 15)
        {
            PatternBuilder melodyTrackBuilder = new PatternBuilder();

            // set instrument  
            melodyTrackBuilder.ProgramChange((SevenBitNumber)instrumentId);

            // set default velocity (volume)
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
            SequenceTrackNameEvent trackNameEvent = new SequenceTrackNameEvent(trackName);
            melodyTrackChunk.Events.Add(trackNameEvent);
            return melodyTrackChunk;
        }


        /// <inheritdoc/>
        public IEnumerable<IBar> ExtractMelody(byte trackNumber)
        {
            // get the requested track 
            TrackChunk melodyTrack = _midiFile.Chunks[trackNumber] as TrackChunk;

            // remove the track from the midi file 
            _midiFile.Chunks.RemoveAt(trackNumber);

            // convert the track to a music-theory melody representation
            IEnumerable<IBar> extractedMelody = ConvertTrackChunkToMelody(melodyTrack);
            return extractedMelody;
        }

        #region ConvertTrackChunkToMelody
        // EncodeMelody
        private IEnumerable<IBar> ConvertTrackChunkToMelody(TrackChunk melodyTrack)
        {
            List<MusicTheory.INote> notes = new List<Note>().Cast<INote>().ToList(); ;
            List<IBar> bars = new List<IBar>();
            IBar currentBar = new Bar();
            int barCounter = 0;
            Melanchall.DryWetMidi.Interaction.Note note;
            short pitch;
            TempoMap tempoMap = _tempoMap;
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
    }

}

