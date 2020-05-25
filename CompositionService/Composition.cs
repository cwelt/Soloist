using CW.Soloist.CompositionService.CompositionStrategies;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            // strategy;
            var chords = ReadChordsFromFile(chordProgressionFilePath);

            var melody = Compositor.Compose(new Bar[] { });

            // TODO: Convert melody to IMidiTrack 

            IMidiFile midiFile = new DryWetMidiAdapter(midiFilePath);

            // TODO: assemble midi file with the composed midi track

            return midiFile;
        }

        #region ReadChordsFromFile
        /// <summary>
        /// <para>Reads and parses a chord progression from an input chord file.</para>
        /// Input file format:
        /// <list type="bullet">
        /// <item> Each line should contain data for a single bar.</item>
        /// <item> The bar data should contain a time signature token (further explained), 
        /// followed by the bar's chords tokens (further explained ahead). 
        /// All tokens must be separated with white space between each other. </item>
        /// <item><term>Key signature token format</term> 'numerator/dominator' for example '3/4' for a bar with three quarters. </item>
        /// <item><term>Chord token format</term> 'chordRoot-chordType-ChordNumberOfBeats', 
        /// where chordRoot is of type <see cref="NoteName"/>, chordType is of type <see cref="ChordType"/>,
        /// and numberOfBeats is relative to the beat units from the bar's key signature denominator.
        /// for example 'FSharp-Dominant7-2,' represents a F#7 chord with duration of two beats. </item>
        /// </list>
        /// </summary>
        /// <param name="chordProgressionFilePath">Path of input chord progression file.</param>
        /// <returns>
        /// A strongly typed enumerable collection of <see cref="IBar"/> 
        /// containning all chords in the file.
        /// </returns>
        /// <exception cref="FormatException">
        /// The lines are not compatible to the defined format. see the format details above.
        /// </exception>
        private IEnumerable<IBar> ReadChordsFromFile(string chordProgressionFilePath)
        {
            using (StreamReader streamReader = File.OpenText(chordProgressionFilePath))
            {
                // initialization 
                uint lineNumber = 1;
                string currentLine = null;
                List<IBar> chordProgression = new List<IBar>();
                IBar bar = null;
                NoteName chordRoot;
                ChordType chordType;
                byte barNumerator = 0;
                byte barDenominator = 0;
                byte numberOfBeats = 0; 
                byte totalBeatsInBar = 0;
                string[] lineTokens = null;
                string[] barTimeSignature = null;
                string[] chordProperties = null;
                string customErrorMessage = string.Empty;
                string genericErrorMessage = $"Error parsing line {lineNumber} in file '{chordProgressionFilePath}'.\n";

                // parse file line after line 
                while ((currentLine = streamReader.ReadLine()) != null)
                {
                    // skip empty lines
                    if (string.IsNullOrWhiteSpace(currentLine))
                        continue;

                    // validate minimum tokens in line (at least a time signature and 1 chord)
                    lineTokens = currentLine.Split('\b','\t');
                    if(lineTokens?.Length < 2)
                    {
                        customErrorMessage = "Each line must include a time signature and at least one chord.";
                        throw new FormatException(genericErrorMessage + customErrorMessage);
                    }

                    // set bar's time signature (first token of each line)
                    barTimeSignature = lineTokens[0].Split('/');
                    if ((barTimeSignature?.Length != 2) ||
                        (!Byte.TryParse(barTimeSignature?[0], out barNumerator)) ||
                        (!Byte.TryParse(barTimeSignature?[1], out barDenominator)))
                    {
                        customErrorMessage = $"Invalid time signature format: {lineTokens[0]}. The required format is 'numerator/denominator', for example 4/4.";
                        throw new FormatException(genericErrorMessage + customErrorMessage);
                    }
                    bar = new Bar(new Duration(barNumerator, barDenominator));

                    // set bar's chords (rest of tokens in line)
                    for (int i = 1; i < lineTokens.Length; i++)
                    {
                        // skip white spaces between the tokens
                        if (string.IsNullOrWhiteSpace(lineTokens[i]))
                            continue;
                        
                        // parse chord properties: root, type & duration
                        chordProperties = lineTokens[i].Split('-');
                        if ((!Enum.TryParse(chordProperties?[0], out chordRoot)) ||
                            (!Enum.TryParse(chordProperties?[1], out chordType)) ||
                            (!Byte.TryParse(chordProperties?[2], out numberOfBeats)))
                        {
                            customErrorMessage = $"Invalid chord format: {chordProperties}. The required format is '{typeof(NoteName)}-{typeof(ChordType)}-DurationInBeats', for example F-Major7-2.";
                            throw new FormatException(genericErrorMessage + customErrorMessage);
                        }
                        totalBeatsInBar += numberOfBeats;
                        bar.Chords.Add(new Chord(chordRoot, chordType, new Duration(numberOfBeats, bar.TimeSignature.Denominator)));
                    }
                    
                    // validate bar's chords total duration == bar's duration 
                    if (bar.TimeSignature.Numerator != totalBeatsInBar)
                    {
                        customErrorMessage = $"Total number of beats of chords in the bar which is {totalBeatsInBar} must be equal to the bar's key signature numerator, which is {bar.TimeSignature.Numerator}.";
                        throw new FormatException(genericErrorMessage + customErrorMessage);
                    }

                    // add curent line's bar 
                    chordProgression.Add(bar);
                    lineNumber++;

                    // clean variables old values for next iteration
                    bar = null;
                    currentLine = customErrorMessage = string.Empty;
                    lineTokens = chordProperties = barTimeSignature = null;
                    barNumerator = barDenominator = numberOfBeats = totalBeatsInBar = 0;
                }
                return chordProgression;
            }
        }
        #endregion
    }
}
