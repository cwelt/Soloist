﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using DWMidiI = Melanchall.DryWetMidi.Interaction;
using DWMidiMT = Melanchall.DryWetMidi.MusicTheory;
using CW.Soloist.CompositionService.MusicTheory;
using Note = CW.Soloist.CompositionService.MusicTheory.Note;
using System.Threading.Tasks;
using System.Threading;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Provides a high level interface for a <a href="https://bit.ly/3bRVzQT"> SMF (Standard MIDI File) </a> 
    /// </summary>
    internal partial class DryWetMidiAdapter : IMidiFile
    {
        #region Adapter Specific Private Data Members 

        private readonly MidiFile _midiContent;

        private readonly TempoMap _tempoMap;

        private readonly IList<TrackChunk> _trackChunks;

        private readonly TrackChunk _metadataTrack;

        private readonly List<MidiEvent> _metaEvents;

        private static readonly string DefaulTrackName = "Soloist Generated Melody";

        /// <summary> Medium for playing the MIDI files events on an output device. </summary>
        private Playback _midiPlayer;

        #endregion


        #region IMidiFile Interface Properties 

        public string FilePath { get; }

        public object Content => _midiContent;
        public string Title { get; } 
        public IDuration KeySignature { get; }
        public byte BeatsPerMinute { get; }
        public int NumberOfBars { get; }

        public NotePitch? LowestPitch { get; }
        public NotePitch? HighestPitch { get; }
        public ICollection<IMidiTrack> Tracks { get; private set; }

        #endregion

        #region Consturctors 

        /// <summary> Constructor </summary>
        /// <param name="midiFilePath"></param>
        internal DryWetMidiAdapter(string midiFilePath)
            : this(File.OpenRead(midiFilePath)) 
        {
            FilePath = midiFilePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        internal DryWetMidiAdapter(Stream stream, string midiFileName = null)
        {
            _midiContent = MidiFile.Read(stream);
            _tempoMap = _midiContent.GetTempoMap();
            _trackChunks = _midiContent.GetTrackChunks().ToList();
            _metadataTrack = _trackChunks.First();
            _metaEvents = _metadataTrack.Events.ToList();

            // set midi title property 
            Title = (((from e in _metaEvents
                       where e.EventType == MidiEventType.SequenceTrackName
                       select e).First()) as BaseTextEvent)?.Text ?? "Undefined";

            // set key signature property 
            TimeSignatureEvent timeSignatureEvent = (from e in _metaEvents
                                      where e.EventType == MidiEventType.TimeSignature
                                      select e)?.First() as TimeSignatureEvent;
            KeySignature = new Duration(timeSignatureEvent.Numerator, timeSignatureEvent.Denominator, false);


            // set number of bars property
            BarBeatFractionTimeSpan duration = _midiContent.GetDuration<BarBeatFractionTimeSpan>();
            NumberOfBars = (int)duration.Bars + (int)Math.Ceiling(duration.Beats / timeSignatureEvent.Numerator);

            // set BPM property 
            BeatsPerMinute = (byte)(_midiContent.GetTempoMap().Tempo.AtTime(0).BeatsPerMinute);

            // set MIDI pitch range
            var notes = _midiContent.GetNotes();
            LowestPitch = (NotePitch)(byte)(notes.Min(note => note.NoteNumber));
            HighestPitch = (NotePitch)(byte)(notes.Max(note => note.NoteNumber));


            // TODO - refactor track representation and dry wet track adapter
            // add midi tracks 
            Tracks = new List<IMidiTrack>();
            foreach (var track in _trackChunks)
            {
                Tracks.Add(new DryWetMidiTrackAdapter(track));
            }
        }


        #endregion

        #region Play()
        public void Play()
        {
            // Play the Output
            using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
            using (_midiPlayer = _midiContent.GetPlayback(outputDevice))
            {
                _midiPlayer.Speed = 1.0;
                _midiPlayer.Play();
            }
        }
        #endregion

        #region PlayAsync()
        public async Task PlayAsync() => await Task.Run(() => Play());
        #endregion

        #region Stop()
        public void Stop() => _midiPlayer?.Stop();
        #endregion 

        #region EmbedMelody
        /// <inheritdoc/>
        public void EmbedMelody(IList<IBar> melody, MusicalInstrument instrument = MusicalInstrument.Sitar, byte? trackNumber = null)
        {
            // use utility private helper method to convert the melody into a midi track 
            TrackChunk melodyTrack = ConvertMelodyToTrackChunk(melody, trackName: DefaulTrackName, instrument);

            // assemble the midi file with new track which contains the melody's midi events 
            int indexOfNewTrack = trackNumber ?? _midiContent.Chunks.Count;
            _midiContent.Chunks.Insert(indexOfNewTrack, melodyTrack);
        }
        #endregion

        #region ConvertMelodyToTrackChunk()
        /// <summary>
        /// Converts a melody encoded in list of <see cref="IBar"/> to a <see cref="TrackChunk"/>
        /// </summary>
        /// <param name="melodyBars"> The bars and notes which are to be converted. </param>
        /// <param name="trackName"> The name that would be given to the newly created midi track. </param>
        /// <param name="instrument"> Midi <a href="https://bit.ly/30pmSzP">program number</a> which represents a musical instrument. </param>
        /// <param name="channel"> The channel number in which the new track would be assigned to. </param>
        /// <returns> A Midi track which contains the melody encapsulated as midi events data. </returns>
        private TrackChunk ConvertMelodyToTrackChunk(IList<IBar> melodyBars, string trackName, MusicalInstrument instrument, byte channel = 15)
        {
            // initialization 
            IBar bar = null;
            INote note = null, nextNote = null;

            PatternBuilder melodyTrackBuilder = new PatternBuilder();

            // set instrument  
            melodyTrackBuilder.ProgramChange((SevenBitNumber)(byte)instrument);

            // set default velocity (volume)
            melodyTrackBuilder.SetVelocity((SevenBitNumber)100);

            // enumerate over all notes         
            for (int i = 0; i < melodyBars.Count; i++)
            {
                bar = melodyBars[i];
                for (int j = 0; j < bar.Notes.Count; j++)
                {
                    note = bar.Notes[j];

                    // set new note's length 
                    var duration = new MusicalTimeSpan(note.Duration.Numerator, note.Duration.Denominator);

                    // if this is a rest or hold note, just step forward and continue to next note
                    if (note.Pitch == NotePitch.RestNote || note.Pitch == NotePitch.HoldNote)
                        melodyTrackBuilder.StepForward(duration);
                    else
                    {
                        // create new note:
                        var newNote = DWMidiMT::Note.Get((SevenBitNumber)(int)note.Pitch);

                        // if successor note(s) are hold notes, add their lengths to the current note  
                        nextNote = (j < bar.Notes.Count - 1) ? (bar.Notes[j + 1]) : ((i < melodyBars.Count - 1 && melodyBars[i + 1].Notes.Count > 0) ? (melodyBars[i + 1].Notes?[0]) : null);
                        while (nextNote?.Pitch == NotePitch.HoldNote)
                        {
                            duration = duration.Add(new MusicalTimeSpan(nextNote.Duration.Numerator, nextNote.Duration.Denominator), TimeSpanMode.LengthLength) as MusicalTimeSpan;
                            (i, j) = (j == bar.Notes.Count - 1) ? (i + 1, 0) : (i, j + 1);
                            bar = melodyBars[i];
                            nextNote = (j < bar.Notes.Count - 1) ? (bar.Notes[j + 1]) : ((i < melodyBars.Count - 1) ? (melodyBars[i + 1].Notes[0]) : null);
                        }

                        // add new note with pre-seted length to midi chunk track: 
                        melodyTrackBuilder.Note(newNote, duration);
                    }
                }
            }

            // use the builder to build a midi track on the requested channel
            Pattern pattern = melodyTrackBuilder.Build();
            TrackChunk melodyTrackChunk = pattern.ToTrackChunk(_tempoMap, (FourBitNumber)channel);

            // add the track's name from input parameter 
            SequenceTrackNameEvent trackNameEvent = new SequenceTrackNameEvent(trackName);
            melodyTrackChunk.Events.Add(trackNameEvent);

            // return midi track to caller 
            return melodyTrackChunk;
        }
        #endregion

        #region ExtractMelody()
        /// <inheritdoc/>
        public void ExtractMelodyTrack(byte trackNumber, in IList<IBar> melodyBars = null)
        {
            // get the requested track 
            TrackChunk melodyTrack = _midiContent.Chunks[trackNumber] as TrackChunk;

            // remove the track from the midi file 
            _midiContent.Chunks.RemoveAt(trackNumber);
            this._trackChunks.RemoveAt(trackNumber);

            /* if requested, decode the melody from the removed track as a list of bars 
             * with music notes and embed this sequence the melody bars reference parameter. */
            if (melodyBars != null)
            {
                ConvertTrackChunkToMelody(melodyTrack, melodyBars);
            }
        }
        #endregion

        #region ConvertTrackChunkToMelody()
        /// <summary>
        /// Converts a midi track from a midi file into a collection of musical notes.
        /// </summary>
        /// <param name="melodyTrack"> The midi track to be converted. </param>
        /// <param name="bars"> The melody contained in the specified track is returned 
        /// in this parameter decoded as music notes in the bar's note collections. 
        /// The bars supplied in this parameter should be fully initialized with the amount of bars 
        /// and duration compatible to the midi's file structure. </param>
        private void ConvertTrackChunkToMelody(TrackChunk melodyTrack, in IList<IBar> bars)
        {
            //Data Initialization
            int currentBarIndex = 0;

            /* note length, start time & end time in two different metric representations:
            / (1) BarBeatFractionTimeSpan (BB):  total number of bars & beats.
            / (2) MusicalTimeSpan (F): total number of beats as a fraction of numerator/denominator. */
            BarBeatFractionTimeSpan lengthBB, startTimeBB, endTimeBB;
            MusicalTimeSpan lengthF, startTimeF, endTimeF, barEndTime, barLengthRemainder, noteLengthRemainder;

            INote note; // current note in iteration 
            short pitch; // curent note's pitch 
            long startBar, endBar; // start & end bar indices of an current note
            Duration barDuration, noteDuration; // current note's & current bar's duration 

            // fetch all notes & rests in midi file 
            IEnumerable<ILengthedObject> notesAndRests = melodyTrack.GetNotesAndRests(RestSeparationPolicy.NoSeparation);

            // enumerate over all notes in file 
            foreach (var midiNote in notesAndRests)
            {
                // current note length 
                lengthF = midiNote.LengthAs<MusicalTimeSpan>(_tempoMap);
                lengthBB = midiNote.LengthAs<BarBeatFractionTimeSpan>(_tempoMap);

                // current note start time 
                startTimeF = midiNote.TimeAs<MusicalTimeSpan>(_tempoMap);
                startTimeBB = midiNote.TimeAs<BarBeatFractionTimeSpan>(_tempoMap);

                // current note end time 
                endTimeF = midiNote.EndTimeAs<MusicalTimeSpan>(_tempoMap);
                endTimeBB = midiNote.EndTimeAs<BarBeatFractionTimeSpan>(_tempoMap);

                // bars on which the note starts at \ end on 
                startBar = startTimeBB.Bars;
                endBar = endTimeBB.Bars;

                // fetch pitch of current note 
                pitch = (midiNote as DWMidiI::Note)?.NoteNumber ?? (short)NotePitch.RestNote;

                // case 1: current note fits entirely inside current bar 
                if (endBar <= currentBarIndex)
                {
                    note = new Note((NotePitch)pitch, (byte)lengthF.Numerator, (byte)lengthF.Denominator);
                    bars[currentBarIndex].Notes.Add(note);

                    // in case current note ends on edge of bar, advance to next one 
                    if (endTimeBB.Beats == bars[currentBarIndex].TimeSignature.Numerator)
                        currentBarIndex++;
                    continue;
                }

                // case 2: note's length spans over multipile bars 
                else
                {
                    // fill remainder of bar with current note 
                    barEndTime = new MusicalTimeSpan(currentBarIndex + 1, 1);
                    barLengthRemainder = barEndTime.Subtract(startTimeF, TimeSpanMode.TimeTime) as MusicalTimeSpan;
                    
                    note = new Note((NotePitch)pitch, (byte)barLengthRemainder.Numerator, (byte)barLengthRemainder.Denominator);
                    bars[currentBarIndex++].Notes.Add(note);

                    // if bar spans over 3 bars or more, fill inbetween bars with hold notes. 
                    while (currentBarIndex < endBar)
                    {
                        barDuration = new Duration(bars[currentBarIndex].TimeSignature.Numerator, bars[currentBarIndex].TimeSignature.Denominator);
                        note = new Note(NotePitch.HoldNote, barDuration);
                        bars[currentBarIndex++].Notes.Add(note);
                    }

                    // fill the note's end bar with the a hold note with remainder of the note's length 
                    noteLengthRemainder = endTimeF.Subtract(new MusicalTimeSpan(currentBarIndex, 1), TimeSpanMode.TimeTime) as MusicalTimeSpan;
                    if (noteLengthRemainder.Numerator > 0)
                    {
                        noteDuration = new Duration((byte)noteLengthRemainder.Numerator, (byte)noteLengthRemainder.Denominator);
                        note = new Note(NotePitch.HoldNote, noteDuration);
                        bars[currentBarIndex].Notes.Add(note);
                    }

                    // in case current note ends on edge of bar, advance to next one 
                    if (currentBarIndex < bars.Count && endTimeBB.Beats == bars[currentBarIndex].TimeSignature.Numerator)
                        currentBarIndex++;
                }
            }
        }
        #endregion

        #region GetBarDuration()
        /// <inheritdoc/>
        public IDuration GetBarDuration(int barIndex)
        {
            // try to fetch key signature defined for the specified bar 
            ValueChange<TimeSignature> barKeySignature = _tempoMap.TimeSignature.AtTime(new MusicalTimeSpan(barIndex, 1), _tempoMap).FirstOrDefault();

            // if an explicit key signature for this bar was found, assemble an duration from it
            if (barKeySignature != null)
                return new Duration((byte)barKeySignature.Value.Numerator, (byte)barKeySignature.Value.Denominator, false);

            // if no explicit key signature exists for this specific bar, return the default key signature from meta events
            else return KeySignature;
        }
        #endregion

        #region SaveFile()
        /// <inheritdoc/>
        public string SaveFile(string path = null, string fileNamePrefix = "", bool pathIncludesFileName = false)
        {
            // get time stamp 
            string timeStamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");

            // set file name 
            string fileName = string.Empty;
            if (!pathIncludesFileName)
            {
                fileName = String.IsNullOrWhiteSpace(fileNamePrefix)
                    ? fileNamePrefix + $"output_{timeStamp}.mid"
                    : fileNamePrefix + $"_output_{timeStamp}.mid";
            }

            // set full path: if no path is specified then set desktop as the default path
            path = path ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\midioutput";

            string fullPath;

            if (!pathIncludesFileName)
                fullPath = path + Path.DirectorySeparatorChar + fileName;
            else fullPath = path;

            // save file 
            try
            {
                this._midiContent.Write(filePath: fullPath, overwriteFile: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.InnerException?.Message);
                throw;
            }
            return fullPath;
        }
        #endregion

        #region GetPitchRangeForTrack()
        public void GetPitchRangeForTrack(int trackIndex, out NotePitch? lowestPitch, out NotePitch? highestPitch)
        {
            var notes = (_midiContent.Chunks[trackIndex] as TrackChunk).GetNotes();
            lowestPitch = (NotePitch)(byte)(notes.Min(note => note.NoteNumber));
            highestPitch = (NotePitch)(byte)(notes.Max(note => note.NoteNumber));
        }
        #endregion

    }
}

