namespace CW.Soloist.CompositionService.Midi
{
    // TODO: add setters to handle midi events in the individual chunks


    /// <summary>
    /// Represents a single track in a midi file. 
    /// </summary>
    public interface IMidiTrack
    {
        /// <summary> Ordinal nubmer of this track in the midi file. </summary>
        int TrackNumber { get; }

        /// <summary> Midi sequence track name. </summary>
        string TrackName { get;  }

        /// <summary> 
        /// The general <a href="https://en.wikipedia.org/wiki/General_MIDI">MIDI program number</a> 
        ///  which identifies this track's MIDI muscial instrument        
        /// </summary>
        byte InstrumentMidiCode { get; }

        /// <summary> The <see cref="InstrumentMidiCode"/> instrument description </summary>
        string InstrumentName { get; }
    }
}
