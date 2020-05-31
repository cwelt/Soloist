using CW.Soloist.CompositionService.CompositionStrategies;
using CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy;
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
        private readonly string _midiInputFile;
        private IMidiFile _midiOutputFile;
        private readonly IList<IBar> _chordProgression;

        /// <summary>
        /// Gets or sets the compositor responsible for composing the solo melody with the desired composition strategy.
        /// </summary>
        public Compositor Compositor { get; set; }


        /// <summary>
        /// Construct a new composition.
        /// </summary>
        /// <param name="midiFilePath"> Path of the midi playback file.</param>
        /// <param name="chordProgressionFilePath"> Path of the chord progression file.</param>
        /// <param name="CompositionStrategy"> Composition strategy compositor.</param>
        public Composition(string midiFilePath, string chordProgressionFilePath, Compositor CompositionStrategy)
        {
            _midiInputFile = midiFilePath;
            Compositor = CompositionStrategy;

            // get chords from file 
            try
            {
                _chordProgression = ReadChordsFromFile(chordProgressionFilePath).ToList();
            }
            catch (FormatException)
            {
                throw;
            }
        }
        


        public IMidiFile Compose()
        {
            IMidiFile midiFile = new DryWetMidiAdapter(_midiInputFile);
            IEnumerable<IBar> melody = Compositor.Compose(_chordProgression);


            // test mock melody 
            IBar bar = new Bar(new Duration(4, 4));
            bar.Notes.Add(new Note(NotePitch.A5, new Duration(1, 2)));
            bar.Notes.Add(new Note(NotePitch.E5, new Duration(1, 2)));
            bar.Notes.Add(new Note(NotePitch.F5, new Duration(1, 2)));
            bar.Notes.Add(new Note(NotePitch.E5, new Duration(1, 2)));


            melody = new List<IBar>() { bar };

            // Embed the generated melody in the midi file
            midiFile.EmbedMelody(melody);


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
