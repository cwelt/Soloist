using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Provides a high level interface for a <a href="https://bit.ly/3bRVzQT"> SMF (Standard MIDI File) </a>.
    /// </summary>
    public interface IMidiFile
    {
        /// <summary> Absolute physical path of the MIDI file. </summary>
        string FilePath { get; set; }

        /// <summary> MIDI Sequence Name from the header track meta events. </summary>
        string Title { get; set; }

        /// <summary> BPM <a href="https://bit.ly/2LIuuVE">(Beats Per Minute).</a> </summary>
        byte BeatsPerMinute { get;  }

        /// <summary> Total number of bars in the MIDI file. </summary>
        int NumberOfBars {get; }

        /// <summary> The key signature from the MIDI meta events. </summary>
        string KeySignature { get; }

        /// <summary> The tracks contained in the MIDI file. </summary>
        ICollection<IMidiTrack> Tracks { get; set; }

        /// <summary> Starts playing the MIDI events containd in the file. </summary>
        void Play();

        /// <summary> Stops playing the MIDI data. </summary>
        void Stop();

    }
}
