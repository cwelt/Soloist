namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Represents a single track in a midi file. 
    /// </summary>
    public interface IMidiTrack
    {
        /// <summary> Ordinal number of this track in the midi file. </summary>
        MelodyTrackIndex TrackNumber { get; }

        /// <summary> Midi sequence track name. </summary>
        string TrackName { get;  }

        /// <summary> 
        /// The general <a href="https://en.wikipedia.org/wiki/General_MIDI">MIDI program number</a> 
        ///  which identifies this track's MIDI musical instrument.        
        /// </summary>
        MusicalInstrument? InstrumentMidiCode { get; }

        /// <summary> The <see cref="InstrumentMidiCode"/> instrument description. </summary>
        string InstrumentName { get; }
    }
}
