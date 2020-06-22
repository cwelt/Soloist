using System;
using System.IO;
using System.Collections.Generic;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
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
        private readonly byte? _melodyTrackIndex;
        private IList<IBar> _melodySeed;

        /// <summary> Default lowest pitch that would be used for the composition. </summary>
        public const NotePitch DefaultMinPitch = NotePitch.E2;

        /// <summary> Default highest pitch that would be used for the composition. </summary>
        public const NotePitch DefaultMaxPitch = NotePitch.E6;

        /// <summary> Handle for the midi input file content and metadata. </summary>
        public IMidiFile MidiInputFile { get; }

        /// <summary> Handle for the midi file which contains the composed melody. </summary>
        public IMidiFile MidiOutputFile { get; private set; }
        
        /// <summary> The chord progression harmony of the composition. </summary>
        public IList<IBar> ChordProgression { get; }

        /// <summary>
        /// compositor responsible for composing the solo melody with the desired composition strategy.
        /// </summary>
        private Compositor _compositor;

        /// <summary> The musical instrument what would be used for playing the composed melody. </summary>
        public MusicalInstrument MusicalInstrument;

        /// <summary> The composition strategy for carrying out the actual melody compoition. </summary>
        public CompositionStrategy CompositionStrategy { get; set; }


        #region Constructors 

        /// <summary>
        /// Construct a composition from a chord progression, midi file handler, 
        /// and an indicator determinning if this midi file is a pure playback or whether
        /// it contains a melody track that should be replaced when composing a new melody.
        /// </summary>
        /// <remarks> Number of bars in midi file and chord progression must match exactly, and so do their durations. </remarks>
        /// <param name="chordProgression"> The bar sequence which contains the chord progression.</param>
        /// <param name="midiFile"> The MIDI file handler. </param>
        /// <param name="melodyTrackIndex"> Index of the existing melody track in the midi file, if one exists. 
        /// <para>
        /// When composing, the track with this index would be replaced by the new composed 
        /// melody track. If the current midi file is a pure playback that contains no existing
        /// melody track, then this property should be set to null. </para></param>
        /// <exception cref="InvalidDataException"> Thrown when there is a mismatch 
        /// between the chord progression and the midi file, either in the total number 
        /// of bars, or the duration of individual bars. </exception>
        /// <exception cref="IndexOutOfRangeException"> Throw when <paramref name="melodyTrackIndex"/> 
        /// has a value which equal or greater than the number of track in the given midi file. </exception>
        public Composition(IList<IBar> chordProgression, IMidiFile midiFile, byte? melodyTrackIndex = null)
        {
            // set chord progression and midi properties 
            ChordProgression = chordProgression;
            MidiInputFile = midiFile;
            _midiInputFilePath = MidiInputFile.FilePath;
            _midiInputFileName = Path.GetFileNameWithoutExtension(MidiInputFile.FilePath);
            _melodyTrackIndex = melodyTrackIndex;

            // validate melody track index is not out of bounds 
            if (melodyTrackIndex.HasValue && melodyTrackIndex >= MidiInputFile.Tracks.Count)
            {
                throw new IndexOutOfRangeException(
                    $"Error: Invalid index for {nameof(melodyTrackIndex)}." +
                    $"The given MIDI file has only {MidiInputFile.Tracks.Count} tracks.");
            }
                
            // validate that total nubmer of bars in CHORD progression match MIDI file total
            if (MidiInputFile.NumberOfBars != chordProgression.Count)
            {
                throw new InvalidDataException(
                    $"Error: Number of bars mismatch: \n" +
                    $"chord file has {chordProgression.Count} bars," +
                    $" while midi file has {MidiInputFile.NumberOfBars}!" +
                    $"\nBars must match inorder to build a composition.");
            }

            // validate that each bar duration from CHORD progression matches the duration from MIDI file  
            IDuration midiDuration = null;
            byte barNumerator = 0;
            byte barDenominator = 0;

            for (int barIndex = 0; barIndex < ChordProgression.Count; barIndex++)
            {
                // get bar's duration from chord progression 
                barNumerator = ChordProgression[barIndex].TimeSignature.Numerator;
                barDenominator = ChordProgression[barIndex].TimeSignature.Denominator;

                // get bar's duration from midi file 
                midiDuration = MidiInputFile.GetBarDuration(barIndex);

                // validate equality
                if (barNumerator != midiDuration.Numerator || barDenominator != midiDuration.Denominator)
                {
                    throw new InvalidDataException(
                        $"Error: Time signature '{barNumerator}/{barDenominator}' " +
                        $"of bar number {barIndex + 1} in the chord progression " +
                        $"does not match the corresponding time signature " +
                        $"'{midiDuration.Numerator}/{midiDuration.Denominator}' " +
                        $"in the midi file.");
                }
            }
        }

        /// <summary>
        /// Construct a composition from a chord progression, midi file handler, 
        /// and an indicator determinning if this midi file is a pure playback or whether
        /// it contains a melody track that should be replaced when composing a new melody.
        /// </summary>
        /// <remarks> Number of bars in midi file and chord progression must match exactly, and so do their durations. </remarks>
        /// <param name="chordProgressionFilePath"></param>
        /// <param name="midiFilePath"></param>
        /// <param name="melodyTrackIndex"> One-based Index of the existing melody track in the midi file, if one exists. 
        /// <para>
        /// When composing, the track with this index would be replaced by the new composed 
        /// melody track. If the current midi file is a pure playback that contains no existing
        /// melody track, then this property should be set to null. </para></param>
        /// <exception cref="InvalidDataException"> Thrown when there is a mismatch 
        /// between the chord progression and the midi file, either in the total number 
        /// of bars, or the duration of individual bars. </exception>
        /// <exception cref="FormatException"> Thrown when the chord progression file isn't 
        /// constructed in the expected format syntax <see cref="ReadChordsFromFile(string)"/>. </exception>
        /// /// <exception cref="IndexOutOfRangeException"> Throw when <paramref name="melodyTrackIndex"/> 
        /// has a value which equal or greater than the number of track in the given midi file. </exception>
        public Composition(string chordProgressionFilePath, string midiFilePath, byte? melodyTrackIndex = null)
            : this( chordProgression: ReadChordsFromFile(chordProgressionFilePath), 
                    midiFile: new DryWetMidiAdapter(midiFilePath), melodyTrackIndex) {}
        #endregion

        /// <summary>
        /// <inheritdoc cref="Compose(CompositionStrategy, OverallNoteDurationFeel, MusicalInstrument, PitchRangeSource, NotePitch, NotePitch, bool)"/>
        /// <para> This is an overloaded version which gathers all the input parameters in a single structure.
        /// See <seealso cref="Compose(CompositionStrategy, OverallNoteDurationFeel, MusicalInstrument, PitchRangeSource, NotePitch, NotePitch, bool)"/> </para>
        /// </summary>
        /// <param name="compositionParams"> Data transfer object containing all the various 
        /// additional parameters for composition preferences and constriants.</param>
        /// <returns></returns>
        public IMidiFile Compose(ICompositionParamsDTO compositionParams)
        {
            return Compose(
                strategy: compositionParams.CompositionStrategy,
                overallNoteDurationFeel: compositionParams.OverallFeel,
                musicalInstrument: compositionParams.MusicalInstrument, 
                pitchRangeSource: compositionParams.PitchRangeSource,
                minPitch: compositionParams.MinPitch,
                maxPitch: compositionParams.MaxPitch,
                useExistingMelodyAsSeed: compositionParams.UseExistingMelodyAsSeed);
        }

        /// <summary>
        /// Composes a solo-melody over this composition's midi playback file and chord progression,
        /// using the additional preferences and constraints parameters. 
        /// </summary>
        /// <returns> A new midi file containing the composed solo-melody. </returns>
        /// <param name="strategy"></param>
        /// <param name="overallNoteDurationFeel"></param>
        /// <param name="musicalInstrument"></param>
        /// <param name="pitchRangeSource"></param>
        /// <param name="minPitch"></param>
        /// <param name="maxPitch"></param>
        /// <param name="useExistingMelodyAsSeed"></param>
        /// <returns></returns>
        public IMidiFile Compose(
            CompositionStrategy strategy = CompositionStrategy.GeneticAlgorithmStrategy,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            MusicalInstrument musicalInstrument = MusicalInstrument.AcousticGrandPiano,
            PitchRangeSource pitchRangeSource = PitchRangeSource.Custom,
            NotePitch minPitch = DefaultMinPitch,
            NotePitch maxPitch = DefaultMaxPitch,
            bool useExistingMelodyAsSeed = true)
        {
            // set compositor according to composition strategy
            _compositor = CompositorFactory.CreateCompositor(strategy);

            // make a copy of the input midi file for the output file   
            MidiOutputFile = new DryWetMidiAdapter(_midiInputFilePath);

            /* if the midi file already contains a melody track, 
             * extract it out of the intended midi output file 
             * and if requested, save it in a separate bar sequence for further usage 
             * as a melody initialization seed for the composition if needed. */
            if (_melodyTrackIndex.HasValue)
            {
                /* if the existing melody should serve as a seed, 
                 * initialize a bar-sequence place-holder for it, 
                 * based on the chord progression structure */
                _melodySeed = useExistingMelodyAsSeed ?
                    Composition.CloneChordProgressionBars(ChordProgression) : null;

                /* extract/remove the existing melody from midi file and 
                 * save it in the place holder if it was initialized */
                MidiOutputFile.ExtractMelodyTrack((byte)_melodyTrackIndex, _melodySeed);
            }

            // initialize pitch range from midi file if requested 
            if (pitchRangeSource == PitchRangeSource.MidiFile && _melodyTrackIndex.HasValue)
            {
                NotePitch? lowestPitch, highestPitch; 
                MidiInputFile.GetPitchRangeForTrack((int)_melodyTrackIndex, out lowestPitch, out highestPitch);
                if (lowestPitch.HasValue && highestPitch.HasValue)
                {
                    minPitch = (NotePitch)lowestPitch;
                    maxPitch = (NotePitch)highestPitch;
                }
            }

            // validate pitch range is at least one octave long (12 semi-tones) 
            if ((byte)maxPitch - (byte)minPitch < MusicTheoryServices.SemitonesInOctave)
                throw new ArgumentException($"Error: Pitch range must be at least one octave long!\n" +
                    $"current range: minPitch={(byte)minPitch}, maxPitch={(byte)maxPitch}");

            // compose a new melody 
            IList<IBar> composedMelody = _compositor.Compose(
                chordProgression: ChordProgression,
                melodyInitializationSeed: _melodySeed,
                overallNoteDurationFeel: overallNoteDurationFeel,
                minPitch: minPitch,
                maxPitch: maxPitch);

            // Embed the new generated melody into the midi file
            MidiOutputFile.EmbedMelody(composedMelody, musicalInstrument);

            // save output
            //MidiOutputFile.SaveFile(fileNamePrefix: _midiInputFileName);

            return MidiOutputFile;
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
        public static IList<IBar> ReadChordsFromFile(string chordProgressionFilePath)
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
                return chordProgression;
            }
        }
        #endregion

        #region ReadMidiFile()
        /// <summary>
        /// Reads contents of a standard midi file and returns a high-level  
        /// interface handler for the midi's content and metadata. 
        /// </summary>
        /// <param name="filePath"> Path of the midi file.</param>
        /// <returns></returns>
        public static IMidiFile ReadMidiFile(string filePath)
        {
            return new DryWetMidiAdapter(filePath);
        }
        #endregion

        #region CloneChordProgressionBars()
        /// <summary>
        /// Clones the structure and chords from the given bar chord progression 
        /// bar sequence, and returns a new bar sequence which contain the same 
        /// time signatures and chords as the chord progression, but with an empty
        /// list of notes in each bar (empty melody).
        /// </summary>
        /// <param name="chordProgression"> Chord progression bar sequence to clone. </param>
        /// <returns> A new bar sequence which contains the same time signature durations
        /// and the same chord progression as the given bar sequence, but with an empty melody.</returns>
        internal static IList<IBar> CloneChordProgressionBars(IEnumerable<IBar> chordProgression)
        {
            // create a new empty bar sequence for the result 
            IList<IBar> clonedBars = new List<IBar>(chordProgression.Count());

            /* fill up the new bar sequence with the original bars' time signature & 
             * chords but with an empty note sequence in each bar (empty melody).  */
            foreach (IBar bar in chordProgression)
                clonedBars.Add(new Bar(bar.TimeSignature, bar.Chords));

            // return the cloned bars
            return clonedBars;
        }
        #endregion


        #region TEST ChordNoteMapping
        // TODO: DELETE ME!!!
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
        #endregion
    }
}
