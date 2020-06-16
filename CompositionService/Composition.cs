using System;
using System.IO;
using System.Collections.Generic;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Compositors.GeneticAlgorithm;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using System.Linq;

namespace CW.Soloist.CompositionService
{
    /// <summary>
    /// Context service class for composing a solo-melody over a given playback.
    /// <para> This class serves as the context participant in the strategy design pattern. </para>
    /// </summary>
    public class Composition
    {
        private readonly string _midiInputFilePath;
        private readonly string _midiInputFileName;
        private IMidiFileService _midiInputFile;
        private MusicalInstrument _musicalInstrument;

        
        
        // TODO: use input file to construct the output without duplicating unnecessary data (prototype\flyweight)
        private IMidiFileService _midiOutputFile;

        private readonly IList<IBar> _chordProgression;

        /// <summary>
        /// Gets or sets the compositor responsible for composing the solo melody with the desired composition strategy.
        /// </summary>
        public Compositor Compositor { get; set; }

        #region Constructor 
        /// <summary> Construct a new composition. </summary>
        /// <param name="midiFilePath"> Path of the midi playback file.</param>
        /// <param name="chordProgressionFilePath"> Path of the chord progression file.</param>
        public Composition(string midiFilePath, string chordProgressionFilePath, MusicalInstrument instrument = MusicalInstrument.OverdrivenGuitar)
        {
            // open & read the midi file using an adapter 
            _midiInputFilePath = midiFilePath;
            _midiInputFileName = Path.GetFileNameWithoutExtension(midiFilePath);
            _midiInputFile = new DryWetMidiAdapter(midiFilePath);
            _musicalInstrument = instrument;

            // get chords from file 
            try
            {
                _chordProgression = ReadChordsFromFile(chordProgressionFilePath);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        #endregion


        /// <summary>
        /// overloaded version that uses the default composition strategy. <see cref="Compose(Compositor)"/>
        /// </summary>
        /// <returns> A new midi file containing the composed solo-melody. </returns>
        public IMidiFileService Compose(MusicalInstrument instrument = MusicalInstrument.OverdrivenGuitar)
        {
            return Compose(CompositionStrategy.GeneticAlgorithmStrategy, instrument);
        }

        /// <summary>
        /// Composes a solo-melody over this composition's midi playback file and chord progression. 
        /// </summary>
        /// <param name="CompositionStrategy"> The strategy to use to compose the melody. </param>
        /// <returns> A new midi file containing the composed solo-melody. </returns>
        public IMidiFileService Compose(CompositionStrategy strategy, MusicalInstrument instrument = MusicalInstrument.OverdrivenGuitar)
        {
            // set composition strategy
            Compositor = Compositor.CreateCompositor(strategy);

            // create adapter handle for the newly created midi file 
            _midiOutputFile = new DryWetMidiAdapter(_midiInputFilePath);

            // remove currently existing melody from midi file 
            _midiOutputFile.ExtractMelody(trackNumber: 1, _chordProgression);

            // compose a new melody 
            IEnumerable<IBar> melody = Compositor.Compose(_chordProgression);

            // Embed the new generated melody into the midi file
            _midiOutputFile.EmbedMelody(melody: melody.ToList(), melodyTrackName: "new melody", instrument);

            // save output
            _midiOutputFile.SaveFile(fileNamePrefix: _midiInputFileName);

            return _midiOutputFile;
        }

        #region ReadChordsFromFile()
        /// <summary>
        /// <para>Reads and parses a chord progression from an input chord file.</para>
        /// Input file format:
        /// <list type="bullet">
        /// <item> Each line should contain data for a single bar.</item>
        /// <item> The bar data should contain a time signature token (further explained ahead), 
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
        private IList<IBar> ReadChordsFromFile(string chordProgressionFilePath)
        {
            using (StreamReader streamReader = File.OpenText(chordProgressionFilePath))
            {
                // initialization 
                uint lineNumber = 1;
                string currentLine = null;
                List<IBar> chordProgression = new List<IBar>();
                IBar bar = null;
                IDuration midiDuration = null;
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
                string genericErrorMessage = $"Error parsing chord progression file '{chordProgressionFilePath}'.\n";

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
                        customErrorMessage = $"Line {lineNumber} must include a time signature and at least one chord.";
                        throw new FormatException(genericErrorMessage + customErrorMessage);
                    }

                    // set bar's time signature (first token of each line)
                    barTimeSignature = lineTokens[0].Split('/');
                    if ((barTimeSignature?.Length != 2) ||
                        (!Byte.TryParse(barTimeSignature?[0], out barNumerator)) ||
                        (!Byte.TryParse(barTimeSignature?[1], out barDenominator)))
                    {
                        customErrorMessage = $"Invalid time signature format in line {lineNumber}: '{lineTokens[0]}'. The required format is 'numerator/denominator', for example 4/4.";
                        throw new FormatException(genericErrorMessage + customErrorMessage);
                    }
                    bar = new Bar(new Duration(barNumerator, barDenominator));

                    // validate that bar duration from CHORD file matches the duration from MIDI file  
                    midiDuration = _midiInputFile.GetBarDuration(chordProgression.Count);
                    if (barNumerator != midiDuration.Numerator || barDenominator != midiDuration.Denominator)
                    {
                        customErrorMessage = $"Time signature '{barNumerator}/{barDenominator}' of bar number {chordProgression.Count + 1} in line {lineNumber} in chord file [{chordProgressionFilePath}] does not match time signature '{midiDuration.Numerator}/{midiDuration.Denominator}' in midi file [{_midiInputFilePath}].";
                        throw new FormatException(genericErrorMessage + customErrorMessage);
                    }

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
                            customErrorMessage = $"Invalid chord format in line {lineNumber}: '{chordProperties}'. The required format is '{typeof(NoteName)}-{typeof(ChordType)}-DurationInBeats', for example F-Major7-2.";
                            throw new FormatException(genericErrorMessage + customErrorMessage);
                        }
                        totalBeatsInBar += numberOfBeats;
                        bar.Chords.Add(new Chord(chordRoot, chordType, new Duration(numberOfBeats, bar.TimeSignature.Denominator)));
                    }
                    
                    // validate bar's chords total duration == bar's duration 
                    if (bar.TimeSignature.Numerator != totalBeatsInBar)
                    {
                        customErrorMessage = $"Line {lineNumber}: Total number of beats of chords in bar {chordProgression.Count + 1} which is {totalBeatsInBar} must be equal to the bar's key signature numerator, which is {bar.TimeSignature.Numerator}.";
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

                // validate that total nubmer of bars in CHORD file match MIDI file total
                if (_midiInputFile.NumberOfBars != chordProgression.Count)
                {
                    customErrorMessage = $"Number of bars mismatch: chord file [{chordProgressionFilePath}] has {chordProgression.Count} bars, while midi file [{_midiInputFilePath}] has {_midiInputFile.NumberOfBars}!";
                    throw new FormatException(customErrorMessage);
                }
                return chordProgression;
            }
        }
        #endregion


        public static void TestChordNoteMapping()
        {
            // test chord-note mapping
            foreach (var chordType in Enum.GetValues(typeof(ChordType)))
            {
                IChord chord = new Chord(NoteName.C, (ChordType)chordType, new Duration());
                Console.Write($"{chordType}: ");
                foreach (var note in chord.GetScaleNotes(4, 5))
                {
                    Console.Write(note + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
