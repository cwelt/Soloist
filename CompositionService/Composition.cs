using System;
using System.IO;
using System.Collections.Generic;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System.Linq;
using CW.Soloist.CompositionService.Compositors.GeneticAlgorithm;

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
        private readonly MelodyTrackIndex? _melodyTrackIndex;
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

        public static bool IsMelodyTrackIndexValid(int? melodyTrackIndex, IMidiFile midiFile, out string errorMessage)
        {
            if (melodyTrackIndex.HasValue && melodyTrackIndex >= midiFile?.Tracks.Count)
            {
                errorMessage = $"Error: Invalid index for {nameof(melodyTrackIndex)}." +
                               $"The given MIDI file has only {midiFile.Tracks.Count - 1} tracks.";
                return false;
            }
            errorMessage = null;
            return true;
        }

        public static bool AreBarsCompatible(IList<IBar> chordProgression, IMidiFile midiFile, out string errorMessage)
        {
            // validate that total num of bars in MIDI matches num of bars in chord progression
            if (chordProgression?.Count != midiFile?.NumberOfBars)
            {
                errorMessage = $"Error: Number of bars mismatch: \n" +
                    $"chord file has {chordProgression?.Count} bars," +
                    $" while midi file has {midiFile?.NumberOfBars}!";
                return false;
            }

            // validate that each bar duration from CHORD progression matches the duration in MIDI
            IDuration midiDuration = null;
            byte barNumerator = 0;
            byte barDenominator = 0;

            for (int barIndex = 0; barIndex < chordProgression.Count; barIndex++)
            {
                // get bar's duration from chord progression 
                barNumerator = chordProgression[barIndex].TimeSignature.Numerator;
                barDenominator = chordProgression[barIndex].TimeSignature.Denominator;

                // get bar's duration from midi file 
                midiDuration = midiFile.GetBarDuration(barIndex);

                // validate equality
                if (barNumerator != midiDuration.Numerator || barDenominator != midiDuration.Denominator)
                {
                    errorMessage = $"Error: Time signature '{barNumerator}/{barDenominator}' " +
                        $"of bar number {barIndex + 1} in the chord progression " +
                        $"does not match the corresponding time signature " +
                        $"'{midiDuration.Numerator}/{midiDuration.Denominator}' " +
                        $"in the midi file.";
                }
            }

            errorMessage = null;
            return true;
        }



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
        public Composition(IList<IBar> chordProgression, IMidiFile midiFile, MelodyTrackIndex? melodyTrackIndex = null)
        {
            // set chord progression and midi properties 
            ChordProgression = chordProgression;
            MidiInputFile = midiFile;
            _midiInputFilePath = MidiInputFile.FilePath;
            _midiInputFileName = Path.GetFileNameWithoutExtension(MidiInputFile.FilePath);
            _melodyTrackIndex = melodyTrackIndex;

            // place holder for an error message if validtions fail
            string errorMessage;

            // validate melody track index is not out of bounds 
            if (!Composition.IsMelodyTrackIndexValid((int?)melodyTrackIndex, MidiInputFile, out errorMessage))
                throw new IndexOutOfRangeException(errorMessage);

            // validate that bars in CHORD progression are compatible with MIDI file 
            if (!AreBarsCompatible(chordProgression, MidiInputFile, out errorMessage))
                throw new InvalidDataException(errorMessage);
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
        public Composition(string chordProgressionFilePath, string midiFilePath, MelodyTrackIndex? melodyTrackIndex = null)
            : this(chordProgression: ReadChordsFromFile(chordProgressionFilePath),
                    midiFile: new DryWetMidiAdapter(midiFilePath), melodyTrackIndex)
        { }
        #endregion


        #region Compose
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
        /// <param name="customParams"></param>
        /// <returns></returns>
        public IMidiFile[] Compose(
            CompositionStrategy strategy = CompositionStrategy.GeneticAlgorithmStrategy,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            MusicalInstrument musicalInstrument = MusicalInstrument.AcousticGrandPiano,
            PitchRangeSource pitchRangeSource = PitchRangeSource.Custom,
            NotePitch minPitch = DefaultMinPitch,
            NotePitch maxPitch = DefaultMaxPitch,
            bool useExistingMelodyAsSeed = true, 
            params object[] customParams)
        {
            // set compositor according to composition strategy
            _compositor = CompositorFactory.CreateCompositor(strategy);

            // make a copy of the input midi file for the output file   
            MidiOutputFile = new DryWetMidiAdapter(_midiInputFilePath);

            /* if the midi file already contains a melody track, 
             * extract it out of the intended midi output file 
             * and if requested, save it in a separate bar sequence for further usage 
             * as a melody initialization seed for the composition if needed. */
            if (_melodyTrackIndex.HasValue && _melodyTrackIndex.Value != MelodyTrackIndex.NoMelodyTrackInFile)
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
            if (!IsPitchRangeValid((int)minPitch, (int)maxPitch, out string errorMessage))
                throw new ArgumentException(errorMessage);

            // compose a new melody 
            IList<IBar>[] composedMelodies = _compositor.Compose(
                chordProgression: ChordProgression,
                melodyInitializationSeed: _melodySeed,
                overallNoteDurationFeel: overallNoteDurationFeel,
                minPitch: minPitch,
                maxPitch: maxPitch,
                customParams: customParams)
                .ToArray();

            // Embed each new generated melody into a new separate midi file
            IMidiFile[] midiOutputs = new IMidiFile[composedMelodies.Length];
            for (int i = 0; i < composedMelodies.Length; i++)
            {
                // TODO LATER: Clone the playback explictly 
                midiOutputs[i] = new DryWetMidiAdapter(_midiInputFilePath);
                midiOutputs[i].ExtractMelodyTrack((byte)_melodyTrackIndex);
                midiOutputs[i].EmbedMelody(composedMelodies[i], musicalInstrument);
            }

            // save first output in dedicated placeholder 
            MidiOutputFile = midiOutputs[0];

            return midiOutputs;
        }
        #endregion




        #region ReadChordsFromFile()
        /// <summary>
        /// <para>Reads and parses a chord progression from an input stream or chord file.</para>
        /// Input file/stream format:
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
            // delegate the reading to overloaded version which accepts a stream 
            return ReadChordsFromFile(File.OpenText(chordProgressionFilePath));
        }

        /// <inheritdoc cref="ReadChordsFromFile(string)"/>
        /// <param name="streamReader"> Input stream which contains the chords data. </param>
        public static IList<IBar> ReadChordsFromFile(StreamReader streamReader)
        {
            streamReader.BaseStream.Position = 0;

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
            string genericErrorMessage = $"Error parsing chord progression file.\n";

            // parse file line after line 
            while ((currentLine = streamReader.ReadLine()) != null)
            {
                // skip empty lines
                if (string.IsNullOrWhiteSpace(currentLine))
                    continue;

                // validate minimum tokens in line (at least a time signature and 1 chord)
                lineTokens = currentLine.Split('\b', '\t', ' ');
                if (lineTokens?.Length < 2)
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
                bar = new Bar(new Duration(barNumerator, barDenominator, false));

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
                        customErrorMessage = $"Invalid chord format in line {lineNumber}: '{lineTokens[i]}'. The required format is '{typeof(NoteName)}-{typeof(ChordType)}-DurationInBeats', for example F-Major7-2.";
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

        public static IMidiFile ReadMidiFile(Stream stream)
        {
            return new DryWetMidiAdapter(stream);
        }
        #endregion

        #region CreateMidiPlayback()
        /// <summary>
        /// Creates a midi playback file by copying an existing midi file or stream 
        /// and removing it's melody track, is such exists (according to the specified 
        /// track number in the <paramref name="MelodyTrackNumber"/> parameter. 
        /// <para> If the given midi is already a pure playback with no melody tracks, 
        /// then a new copy of this midi would be returned. 
        /// </para>
        /// </summary>
        /// <param name="midiStream"> The midi input that the playback should be created from. </param>
        /// <param name="MelodyTrackNumber">Number of the melody track in the given midi, if such a track exists.</param>
        /// <returns></returns>
        public static IMidiFile CreateMidiPlayback(Stream midiStream, MelodyTrackIndex? melodyTrackNumber)
        {
            // create a new midi file instance 
            IMidiFile midifile = new DryWetMidiAdapter(midiStream);

            // remove melody track is such exists on this midi file 
            if (melodyTrackNumber.HasValue && melodyTrackNumber != MelodyTrackIndex.NoMelodyTrackInFile)
            {
                midifile.ExtractMelodyTrack((byte)melodyTrackNumber);
            }

            // return the resulted playback midi file 
            return midifile;
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


        /// <summary>
        /// Validates pitch range is at least one octave long (12 semi-tones).
        /// </summary>
        /// <param name="minPitch"> The requested lowest bound for a pitch. </param>
        /// <param name="maxPitch"> The requested upper bound for a pitch. </param>
        /// <param name="errorMessage"> Detailed error message incase the the validation fails,
        /// i.e., when the returned result is false. If the validation succeeds and the 
        /// the returned result is true, this out parameter would be set to null. </param>
        /// <returns> True if range is valid (at least one octave long), and false otherwise. </returns>
        public static bool IsPitchRangeValid(int minPitch, int maxPitch, out string errorMessage)
        {
            // if valid 
            if (maxPitch - minPitch >= MusicTheoryServices.SemitonesInOctave)
            {
                errorMessage = null;
                return true;
            }
            else // if invalid 
            {
                errorMessage = $"Error: Pitch range must be at least one octave long!\n"
                             + $"current range: minPitch={minPitch}, maxPitch={maxPitch}\n";
                return false;
            }
        }
    }
}
