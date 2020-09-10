using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// Factory for creating midi files and midi tracks instances,
    /// and providing them as abstract interfaces, 
    /// abstracting from clients the specific implementations they use,
    /// inversing the controll of dependencies ("Don't call us, We'll call you..."),
    /// such that high-level entites of the clients depened only on the abstract interfaces 
    /// and not on the concrete implementation that uses one third library or another. 
    /// </summary>
    internal static class MidiFactory
    {
        #region CreateMidiFile()
        /// <summary>
        /// Creates a midi file instance that represents the midi file that us located in the given path. 
        /// </summary>
        /// <param name="midiFilePath"> The path in which the midi file is located.  </param>
        /// <returns> A IMidiFile instance that represents the midi file that us located in the given path </returns>
        internal static IMidiFile CreateMidiFile(string midiFilePath) 
        {
            return new DryWetMidiAdapter(midiFilePath);
        }

        /// <summary>
        /// Creates a midi file instance based on a given byte stream which contains the midi's file content. 
        /// </summary>
        /// <param name="stream"> Byte stream with the midi's file content.  </param>
        /// <param name="disposeStream"> If set, stream would be closed once it's read completely. </param>
        /// <returns> A IMidiFile instance that represents the midi file that us located in the given path </returns>
        internal static IMidiFile CreateMidiFile(Stream stream, bool disposeStream = false)
        {
            return new DryWetMidiAdapter(stream, disposeStream);
        }
        #endregion
    }


}
