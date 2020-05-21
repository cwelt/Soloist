using CW.Soloist.CompositionService.CompositionStrategies;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService
{
    /// <summary>
    /// Context service class for composing a solo-melody over a given playback.
    /// <para>
    /// <remarks> This class serves as the context participant in the strategy design pattern. </remarks>
    /// </para>
    /// </summary>
    public class Composition
    {

        
        /// <summary>
        /// <value> 
        /// Gets or sets the compositor responsible for composing the solo melody with the desired composition strategy.
        /// See <see cref="Compositor"/>.
        /// </value> 
        /// </summary>
        public Compositor Compositor { get; set; }

        public IMidiFile Compose(string midiFilePath, string chordProgressionFilePath)
        {
            // TODO: extract chord progression data and build strongly typed collection
            // of IChord so we could delegate the compose request to the composition 
            // strategy

            var melody = Compositor.Compose(new Bar[] { });

            // TODO: Convert melody to IMidiTrack 

            IMidiFile midiFile = new DryWetMidiAdapter(midiFilePath);

            // TODO: assemble midi file with the composed midi track

            return midiFile;
        }
    }
}
