using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CW.Soloist.CompositionService.Compositors
{

    /// <summary>
    /// Abstract compositor strategy class. 
    /// Subclasses of this class implement concrete composition strategies. 
    /// <para><remarks>
    /// Composition strategies are used by the <see cref="Composition"/> context class.
    /// This class is the abstract strategy class in the strategy design pattern.
    /// </remarks></para>
    /// </summary>
    public abstract class Compositor
    {
        /// <summary> Melody seed to base on the composition of the new melody. </summary>
        internal IList<IBar> Seed { get; private protected set; }

        /// <summary> The outcome of the <see cref="Compose"/> method. </summary>
        internal IList<IBar> ComposedMelody { get; private protected set; }

        /// <summary> The playback's harmony. </summary>
        internal IList<IBar> ChordProgression { get; private protected set; }

        /// <summary> Default duration denominator for a single note. </summary>
        internal IDuration DefaultDuration { get; private protected set; } = new Duration(1, Duration.EighthNoteDenominator);
        internal byte DefaultDurationDenomniator { get; private protected set; } = Duration.EighthNoteDenominator;
        internal float DefaultDurationFraction { get; private protected set; } = Duration.EighthNoteFraction;

        private protected readonly float[] PossibleDurationFractions =
        {
            Duration.HalfNoteFraction,
            Duration.QuaterNoteFraction,
            Duration.EighthNoteFraction,
            Duration.SixteenthNoteFraction,
            Duration.ThirtySecondNoteFraction
        };


        internal byte LongestAllowedDurationDenominator { get; private protected set; } = 2;
        internal float LongestAllowedFraction { get; private protected set; } = Duration.HalfNoteFraction;
        internal byte ShortestAllowedDurationDenominator { get; private protected set; } = 16;
        internal float ShortestAllowedFraction { get; private protected set; } = Duration.SixteenthNoteFraction;
        internal byte DefaultNumOfNotesInBar { get; private protected set; }




        /// <summary> Minimum octave of note pitch range for the composition. </summary>
        public byte MinOctave { get; private protected set; } = 4;

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        public byte MaxOctave { get; private protected set; } = 5;

        /// <summary> Lowest bound of a note pitch for the composition. </summary>
        public NotePitch MinPitch { get; private protected set; } = NotePitch.E2;

        /// <summary> Upper bound of a  note pitch for the composition. </summary>
        public NotePitch MaxPitch { get; private protected set; } = NotePitch.E6;


        /// <summary> Compose a solo-melody over a given playback. </summary>
        /// <param name="chordProgression"> The chords of the song in the playback. </param>
        /// <param name="melodyInitializationSeed"> Optional existing melody on which to base the composition on.</param>
        /// <param name="overallNoteDurationFeel"> Determines the overall feel and density regarding the amount of notes
        /// in the composed melody. For further details, see <see cref="OverallNoteDurationFeel"/>.</param>
        /// <param name="minPitch"> Lowest bound of a note pitch for the composition. </param>
        /// <param name="maxPitch"> Upper bound of a note pitch for the composition. </param>
        /// <returns> The composition of solo-melody</returns>
        internal IEnumerable<IList<IBar>> Compose(
            IList<IBar> chordProgression,
            IList<IBar> melodyInitializationSeed = null,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E6,
            params object[] customParams)
        {
            InitializeCompositionParams(chordProgression, melodyInitializationSeed, overallNoteDurationFeel, minPitch, maxPitch, customParams);

            return GenerateMelody();
        }


        private protected abstract IEnumerable<IList<IBar>> GenerateMelody();
        private protected virtual void InitializeCompositionParams(
            IList<IBar> chordProgression,
            IList<IBar> melodyInitializationSeed = null,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E6,
            params object[] additionalParams)
        {
            // initialize general parameters for the algorithm 
            ChordProgression = chordProgression;
            Seed = melodyInitializationSeed;
            MinPitch = minPitch;
            MaxPitch = maxPitch;

            switch (overallNoteDurationFeel)
            {
                case OverallNoteDurationFeel.Slow:
                    DefaultDurationDenomniator = Duration.QuaterNoteDenominator;
                    ShortestAllowedDurationDenominator = Duration.EighthNoteDenominator;
                    LongestAllowedDurationDenominator = Duration.WholeNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Medium:
                default:
                    DefaultDurationDenomniator = Duration.EighthNoteDenominator;
                    ShortestAllowedDurationDenominator = Duration.SixteenthNoteDenominator;
                    LongestAllowedDurationDenominator = Duration.HalfNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Intense:
                    DefaultDurationDenomniator = Duration.SixteenthNoteDenominator;
                    ShortestAllowedDurationDenominator = Duration.ThirtySecondNoteDenominator;
                    LongestAllowedDurationDenominator = Duration.HalfNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Extreme:
                    DefaultDurationDenomniator = Duration.ThirtySecondNoteDenominator;
                    ShortestAllowedDurationDenominator = Duration.ThirtySecondNoteDenominator;
                    LongestAllowedDurationDenominator = Duration.HalfNoteDenominator;
                    break;
            }
            DefaultDuration = new Duration(1, DefaultDurationDenomniator);
            DefaultDurationFraction = 1F / DefaultDurationDenomniator;
            ShortestAllowedFraction = 1F / ShortestAllowedDurationDenominator;
            LongestAllowedFraction = 1F / LongestAllowedDurationDenominator;
        }

        #region NoteSequenceInitializer()
        /// <summary>
        /// Initializes an entire note sequence for a given sequence of bars, 
        /// based on custom user preferences such as melody contour direction 
        /// and chord-note mapping source.
        /// </summary>
        /// <param name="barCollection"> The subject bar sequence that should be populated with notes.</param>
        /// <param name="mode"> Melody Contour direction of added notes: ascending, descending, zigzag, etc. </param>
        /// <param name="mappingSource"> Default mapping source for the notes: either from scale, chord, or a mix of the two (see <paramref name="toggleMappingSource"/> parameter). </param>
        /// <param name="toggleMappingSource"> If set to true, the mapping source would be toggled constantly between scale and chord sources. </param>
        private protected void NoteSequenceInitializer(IEnumerable<IBar> barCollection,
            NoteSequenceMode mode = NoteSequenceMode.BarZigzag,
            ChordNoteMappingSource mappingSource = ChordNoteMappingSource.Chord,
            bool toggleMappingSource = true)
        {
            // initialization 
            IBar bar;
            IChord chord;
            IDuration noteDuration;
            IDuration[] chordsNotesDurations;
            IList<IBar> bars = barCollection.ToList();

            // indices for the collection of the mapped notes  
            int j = 0, first, middle, last;

            // the mapped note pitches for a given chord 
            NotePitch[] chordMappedNotes;

            // last played pitch on the previous chord. initialize to middle pitch in range
            NotePitch prevChordLastPitch = (NotePitch)((byte)Math.Floor((byte)MinPitch + (byte)MaxOctave / 2F));

            // closest pitch to the previous chord's last pitch 
            NotePitch closestPitch;

            // step for incrementing/decrementing note index in a given iteration 
            int step = ((mode == NoteSequenceMode.Ascending) ? 1 : -1);

            // start proccess of populating bars with mapped notes 
            for (int barIndex = 0; barIndex < bars.Count; barIndex++)
            {
                // fetch current bar 
                bar = bars[barIndex];

                // remove any old existing "garbage" notes if such exist
                bar.Notes.Clear();

                // if BAR zigzag mode is requested, toggle step direction on each bar change 
                if (mode == NoteSequenceMode.BarZigzag)
                    step *= -1;

                // populate notes for the individual chords in current bar 
                for (int chordIndex = 0; chordIndex < bar.Chords.Count; chordIndex++)
                {
                    // fetch current chord
                    chord = bar.Chords[chordIndex];

                    // if CHORD zigzag mode is requested, toggle step direction on each chord change 
                    if (mode == NoteSequenceMode.ChordZigzag)
                        step *= -1;

                    // update chord-note mapping source if toggle is requested
                    if (toggleMappingSource)
                        mappingSource = (ChordNoteMappingSource)(((int)(mappingSource) + 1) % 2);

                    // get the mapped notes under the requested mapping source and pitch range
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        chordMappedNotes = chord.GetArpeggioNotes(MinPitch, MaxPitch).ToArray();
                    else chordMappedNotes = chord.GetScaleNotes(MinPitch, MaxPitch).ToArray();

                    // initialize indices for the mapped notes collection 
                    first = 0;
                    middle = chordMappedNotes.Length / 2;
                    last = chordMappedNotes.Length - 1;

                    // in zigzag mode, continue the sequence flow from last note 
                    if (mode == NoteSequenceMode.BarZigzag || mode == NoteSequenceMode.ChordZigzag)
                    {
                        /* select the closest pitch to previous chord's last pitch from within 
                         * the current chord mapped notes collection */
                        closestPitch = chordMappedNotes
                            .First(pitch1 => Math.Abs((byte)pitch1 - (byte)prevChordLastPitch)
                            .Equals(chordMappedNotes.Min(pitch2 =>
                                             Math.Abs((byte)pitch2 - (byte)prevChordLastPitch))));

                        // set index of next note to continue as close as possible to last played note 
                        j = Array.IndexOf(chordMappedNotes, closestPitch);
                    }

                    // if not zigzag mode, just reset index back to middle of the collection 
                    else j = middle;

                    // generate a sequence of durations for current chord notes 
                    chordsNotesDurations = GenerateDurations(chord.Duration.Fraction);

                    /* build the actual note population in current bar from the mapped notes collection 
                     * and the generated duration sequences */
                    for (int i = 0; i < chordsNotesDurations.Length; i++)
                    {
                        // fetch a note duration 
                        noteDuration = chordsNotesDurations[i];

                        // fetch a note for mapping and add it to note sequence 
                        bar.Notes.Add(new Note(chordMappedNotes[j], noteDuration));
                        if (j == first || j == last)
                            j = middle;
                        else j += step;
                    }

                    /* save last played pitch from the last chord for selecting 
                     * next note with minimum distance interval from it */
                    prevChordLastPitch = bar.Notes[bar.Notes.Count - 1].Pitch;
                }
            }
        }
        #endregion

        #region Generate Duration Sequences 

        #region Generate Durations Based on Overall Density Feel
        /// <summary>
        /// Generates a sequence of durartions that sum up to <paramref name="timeSpanLength"/>
        /// parameter, where the duration density, i.e., number of notes, is based on the 
        /// <paramref name="durationDefaultLength"/> parameter, or on the 
        /// <see cref="DefaultDurationFraction"/> property if the input parameter is set to null.
        /// </summary>
        /// <param name="timeSpanLength"> The time span length as a fraction of a whole bar, 
        /// which the duration sequence should up to. For exmaple, for half a bar this 
        /// parameter should be set to 0.5. </param>
        /// <param name="durationDefaultLength"> Default length for individual durations in the 
        /// resulting sequence. This length must fit inside the total time span length. 
        /// If it invalidly exceeds it, the method would set the default independently 
        /// to the highest possible length which is shorter or equal to total time span. </param>
        /// <returns></returns>
        private IDuration[] GenerateDurations(float timeSpanLength, float? durationDefaultLength = null)
        {
            // initialization 
            int randomIndex;
            int randomRatio;
            float[] possibleFractions;
            Random random = new Random();
            List<float> durationFractions = new List<float>();
            float defaultFraction = durationDefaultLength ?? DefaultDurationFraction;

            /* assure default fraction does not exceed the overall time span length
             * and reset default if necessary */
            if (defaultFraction > timeSpanLength)
            {
                defaultFraction = PossibleDurationFractions
                    .Where(fraction => fraction <= timeSpanLength)
                    .Max(fraction => fraction);
            }

            /* Randomly determine if to reserve the entire time span for the default  
             * duration's lengths or just half of it . */ 
            randomRatio = random.NextDouble() < 0.3 ? 2 : 1;
            
            // calcuate amount of reserved durations according to the random selection above
            int reservedDefaultDurations = (int)(timeSpanLength / DefaultDurationFraction) / randomRatio;

            // add the reserved duration lengths to the result list 
            for (int i = 0; i < reservedDefaultDurations; i++)
                durationFractions.Add(DefaultDurationFraction);

            // update time span according to the remaining length after the reservation 
            timeSpanLength -= reservedDefaultDurations * DefaultDurationFraction;

            // fill up the reamining length with random duration lengths 
            while (timeSpanLength > 0)
            {
                // fetch possible relevant duration fraction lengths 
                possibleFractions = PossibleDurationFractions
                    .Where(fraction => fraction <= timeSpanLength
                                    && fraction >= ShortestAllowedFraction
                                    && fraction <= LongestAllowedFraction)
                    .ToArray();

                // randmoly select one possible fraction length 
                randomIndex = random.Next(possibleFractions.Length);

                // add the fraction to the result list 
                durationFractions.Add(possibleFractions[randomIndex]);

                // update the remaining time span length 
                timeSpanLength -= possibleFractions[randomIndex];
            }

            // shuffle the order of the fractions 
            if (durationFractions.Distinct().Count() > 1)
                durationFractions.Shuffle();

            // return a projection of duration out of the fraction list 
            return durationFractions
                .Select(fraction => new Duration(1, (byte)(1 / fraction)))
                .ToArray();
        }
        #endregion

        #region Generate Durations Based on an Existing melody 
        /// <summary>
        /// Generates a sequence of durations <strong> based on an existing melody </strong> .
        /// This method parses the existing melody notes durations, and returns a 3D 
        /// duration array with the following structure: 
        /// <para>[Bar] [Chord] [Durations of notes for this chord].</para>
        /// <para> The resulting array could be used to retrieve duration for new 
        /// generated notes under a given chord. </para>
        /// <para> The durations themselves are based on the duration from the existing 
        /// melody, with a possible shuffle per chord, according to input parameter
        /// flag, see <paramref name="randomlyShuffleDurations"/>. </para>
        /// </summary>
        /// <param name="existingMelodyBars"> The existing melody which should serve as 
        /// a reference base for generated duration sequence. </param>
        /// <param name="randomlyShuffleDurations"> Flag to indicate wheter to 
        /// randomly shuffle the durations or not. If set to true (default), the 
        /// durations for a given chord might be shuffled  (depends on a "toss of a coin"). 
        /// If set to false, the original durations are returned. </param>
        /// <returns> 
        /// A 3D duration array with the following structure:
        /// <para>[Bar] [Chord] [Durations of notes for this chord].</para>
        /// </returns>
        private protected virtual IDuration[][][] GenerateDurations(
            IList<IBar> existingMelodyBars,
            bool randomlyShuffleDurations = true)
        {
            // initialization 
            IBar bar;
            IChord chord;
            int nextNoteIndex;
            Random random = new Random();
            IList<int> potentialChordNoteIndices;
            IEnumerable<int> filterdChordNoteIndices;

            // initialize a 3D array of durations [bar][chord][chord's notes' durations]
            IDuration[][][] durations = new IDuration[existingMelodyBars.Count][][];

            // iterate through all bars to fill up the 3D array with durations 
            for (int i = 0; i < existingMelodyBars.Count; i++)
            {
                // fetch current bar 
                bar = existingMelodyBars[i];

                // build array for the chords in current bar 
                durations[i] = new IDuration[existingMelodyBars[i].Chords.Count][];

                // initialize index of the next note in bar 
                nextNoteIndex = 0;

                // iterate through all the chords in current bar 
                for (int j = 0; j < existingMelodyBars[i].Chords.Count; j++)
                {
                    // fetch current chord 
                    chord = existingMelodyBars[i].Chords[j];

                    // get indices of the current chord's notes 
                    bar.GetOverlappingNotesForChord(chordIndex: j, out potentialChordNoteIndices);

                    // filter out overlapping note indices if such exist 
                    filterdChordNoteIndices = potentialChordNoteIndices
                        .Where(index => index >= nextNoteIndex);

                    // update index for next note in bar 
                    nextNoteIndex = filterdChordNoteIndices.Max(noteIndex => noteIndex) + 1;

                    // extract durations of the indiced notes into the 3D array  
                    durations[i][j] = filterdChordNoteIndices
                        .Select(chordNoteIndex => bar.Notes[chordNoteIndex].Duration)
                        .ToArray();

                    // randomly shuffle the durations order 
                    if (random.NextDouble() > 0.5)
                        durations[i][j].Shuffle();
                }
            }

            // return the 3D duration array of [bars][chords][chord notes durations]
            return durations;
        }
        #endregion

        #endregion

        #region Arpeggiator Initializers
        private protected void ArpeggiatorInitializer(IEnumerable<IBar> bars, NoteSequenceMode mode = NoteSequenceMode.BarZigzag)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode, mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected void ArpeggiatorInitializerAscending(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.Ascending, mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected void ArpeggiatorInitializerDescending(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.Descending, mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected void ArpeggiatorInitializerChordZigzag(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.ChordZigzag, mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected void ArpeggiatorInitializerBarZigzag(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.BarZigzag, mappingSource: ChordNoteMappingSource.Chord);
        }
        #endregion

        #region Scale Initializers
        private protected void ScaleratorInitializer(IEnumerable<IBar> bars, NoteSequenceMode mode = NoteSequenceMode.BarZigzag)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode, mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected void ScaleratorInitializerAscending(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.Ascending, mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected void ScaleratorInitializerDescending(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.Descending, mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected void ScaleratorInitializerChordZigzag(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.ChordZigzag, mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected void ScaleratorInitializerBarZigzag(IEnumerable<IBar> bars)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode: NoteSequenceMode.BarZigzag, mappingSource: ChordNoteMappingSource.Scale);
        }
        #endregion

        #region ScaleArpeggioeMixInitializer()
        private protected void ScaleArpeggioeMixInitializer(IEnumerable<IBar> bars, NoteSequenceMode mode = NoteSequenceMode.ChordZigzag)
        {
            // delegate the work to the generic method 
            NoteSequenceInitializer(bars, mode, mappingSource: ChordNoteMappingSource.Scale, toggleMappingSource: true); ;
        }
        #endregion

        #region ChangePitchForARandomNote() 
        /// <summary>
        /// <para> Selects a random note in a bar and changes it's pitch to another note.</para>    
        /// <para> The new pitch would be selected randomly from a list which is determined by the 
        /// <paramref name="mappingSource"/> parameter. 
        /// For example, if the chord that is played in parallel to the randomly selected note is C major, 
        /// and mapping source is set to chord, then one of the pitches C (do), E (mi), or G (sol) 
        /// would be selected, otherwise, if mapping source is set to scale, then possible pitches
        /// are C (do), D (re), E (mi), F (fa), G (sol), A (la) and B (si). 
        /// In short, scale mapping has a larger variety of possible new ptches. </para>
        /// <para> Pitch selected is in the range between <see cref="MinOctave"/> and <see cref="MaxOctave"/>.
        /// In addition to this global range, it is possible to define another constraint 
        /// which determines "how far" at most the new pitch can be relative to the original 
        /// pitch. The default is seven semitones at most, see <paramref name="semitoneRadius"/>. </para>
        /// <para>The new note with the randomly selected pitch will replace the original old note,
        /// but will preserve the original note's duration. </para>
        /// </summary>
        /// <remarks> 
        /// No pitch change is made to rest and hold notes. Incase the randomly 
        /// selected chord has no relevant notes played in parallel, or if the notes found 
        /// are not in the specified octave range, then no action would be taken and 
        /// the bar would remain intact. Therefore it is possible that some calls to this 
        /// method would sometimes make no change whatsoever. Incase a change is made 
        /// the method returns true, otherwise, it returns false. 
        /// </remarks>
        /// <param name="bar"> The bar in which to make the pitch replacement in. </param>
        /// <param name="mappingSource"> Determines whether the pitches should be selected from the chord arpeggio notes or from a scale mapped to the chord.</param>
        /// <param name="semitoneRadius"> Determines how many semitones at most could the new pitch be from the exisiting one.</param>
        /// <returns> True if a change has been made, or false if no change has been made, see remarks.</returns>
        private protected virtual bool ChangePitchForARandomNote(IBar bar, ChordNoteMappingSource mappingSource = ChordNoteMappingSource.Chord, byte semitoneRadius = 5)
        {
            // initialize random number generator 
            Random randomizer = new Random();

            // select a random chord from within the bar 
            int randomChordIndex = randomizer.Next(0, bar.Chords.Count);
            IChord chord = bar.Chords[randomChordIndex];

            // get non-rest-hold notes in bar that are played in parallel to selected chord 
            IList<INote> currentNotes = bar.GetOverlappingNotesForChord(chord, out IList<int> notesIndices)
                .Where(note => note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote)
                .ToList();

            // assure there is at least one note to replace under the selected chord 
            if (currentNotes.Count == 0)
                return false;

            // select a random note from within the current notes to be replaced 
            int randomNoteIndex = randomizer.Next(0, currentNotes.Count);
            INote oldNote = currentNotes[randomNoteIndex];
            int oldPitch = (int)oldNote.Pitch;

            // get candidate pitches according to mapping source and octave range
            IEnumerable<NotePitch> candidateNotes;
            if (mappingSource == ChordNoteMappingSource.Chord)
                candidateNotes = chord.GetArpeggioNotes(MinPitch, MaxPitch);
            else candidateNotes = chord.GetScaleNotes(MinPitch, MaxPitch);

            // filter candidats according to specified semitone radius
            int pitchUpperBound = oldPitch + semitoneRadius;
            int pitchLowerBound = oldPitch - semitoneRadius;
            NotePitch[] filteredPitches = candidateNotes
                .Where(pitch => pitch != oldNote.Pitch && (int)pitch >= pitchLowerBound && (int)pitch <= pitchUpperBound)
                .ToArray();

            // assure there is at least one pitch in the specified range
            if (filteredPitches.Length == 0)
                return false;

            // select a random pitch for the new note 
            int randomPitchIndex = randomizer.Next(0, filteredPitches.Length);
            NotePitch newPitch = filteredPitches[randomPitchIndex];

            // replace old note with new note with the selected pitch 
            INote newNote = new Note(newPitch, oldNote.Duration);
            int indexOfOldNoteInBar = bar.Notes.IndexOf(oldNote);
            bar.Notes.RemoveAt(indexOfOldNoteInBar);
            bar.Notes.Insert(indexOfOldNoteInBar, newNote);

            // indicate that a change has been made
            return true;
        }
        #endregion

        #region NoteDurationSplit()
        /// <summary>
        /// Replaces a random note in the given bar with two new shorter notes 
        /// with durations that sum up together to the original's note duration. 
        /// Regarding pitch, one of the new notes after split would have the original's note pitch,
        /// and the other note after split would have a pitch which is minor or major second away from
        /// the original note pitch.
        /// <para> The duration split is done according to the <paramref name="ratio"/>
        /// parameter. </para>
        /// <para> Incase the note's in the bar are too short to be splited, no action 
        /// is made and the method returns false. Otherwise, it return true. </para>
        /// </summary>
        /// <remarks> The shortest possible duration is determined by the corresponding 
        /// input parameter to the compositor's constructor. 
        /// parameter </remarks>
        /// <param name="bar"> The bar in which to make the note's duration split. </param>
        /// <param name="ratio"> The ratio of the duration split. </param>
        /// <returns> True if a split has been made, false otherwise. </returns>
        private protected virtual bool NoteDurationSplit(IBar bar, DurationSplitRatio ratio)
        {
            // extract denominator from the given ratio 
            int ratioDenominator = ((ratio == DurationSplitRatio.Equal) ? 2 : 4);

            // find candidate notes which are long enough for the given split ratio 
            INote[] candidateNotesForSplit = bar.Notes
                .Where(note => note.Duration.Denominator <= DefaultDurationDenomniator &&
                       note.Duration.Denominator <= ShortestAllowedDurationDenominator / ratioDenominator)
                .ToArray();

            // assure there is at least one candidate note which long enough for splitting  
            if (candidateNotesForSplit.Length == 0)
                return false;

            // select a random note from within the bar candidate notes 
            int randomNoteIndex = new Random().Next(candidateNotesForSplit.Length);
            INote existingNote = candidateNotesForSplit[randomNoteIndex];

            // set the two new durations for the two new notes after the split
            IDuration firstDuration, secondDuration;
            byte existingNumerator = existingNote.Duration.Numerator;
            byte existingDenominator = existingNote.Duration.Denominator;
            byte newDenominator = (byte)(existingDenominator * ratioDenominator);

            switch (ratio)
            {
                case DurationSplitRatio.Equal:
                default:
                    firstDuration = new Duration(existingNumerator, newDenominator);
                    secondDuration = new Duration(firstDuration);
                    break;
                case DurationSplitRatio.Anticipation:
                    firstDuration = new Duration(existingNumerator, newDenominator);
                    secondDuration = new Duration((byte)(existingNumerator * 3), newDenominator);
                    break;
                case DurationSplitRatio.Delay:
                    firstDuration = new Duration((byte)(existingNumerator * 3), newDenominator);
                    secondDuration = new Duration(existingNumerator, newDenominator);
                    break;
            }

            // set the pitches for the two new notes after the split
            NotePitch firstPitch, secondPitch;

            // if notes are hold or rest notes, retain them this way 
            if (existingNote.Pitch == NotePitch.HoldNote || existingNote.Pitch == NotePitch.RestNote)
                secondPitch = firstPitch = existingNote.Pitch;

            // otherwise,  
            else // try to set second pitch to one of the scale nearst pitches
            {
                firstPitch = existingNote.Pitch;
                IChord chord = bar.GetOverlappingChordsForNote(bar.Notes.IndexOf(existingNote)).FirstOrDefault();
                NotePitch minPitch = (NotePitch)((byte)firstPitch - PitchInterval.MajorThird);
                NotePitch maxPitch = (NotePitch)((byte)firstPitch + PitchInterval.MajorThird);

                // get nearest pitches from scale 
                IEnumerable<NotePitch> nearsetPitches = MusicTheoryServices
                    .GetNotes(chord, ChordNoteMappingSource.Scale, minPitch, maxPitch)
                    .Except(new[] { firstPitch }).Shuffle();

                // if nearest pitches are out of range, just retain the original first pitch 
                secondPitch = nearsetPitches.Any() ? nearsetPitches.First() : firstPitch;
            }

            // create two new notes with preseted durations & pitches
            INote firstNote = new Note(firstPitch, firstDuration);
            INote secondNote = new Note(secondPitch, secondDuration);

            // replace the existing note with the two new notes 
            int originalNoteIndex = bar.Notes.IndexOf(existingNote);
            bar.Notes.RemoveAt(originalNoteIndex);
            bar.Notes.Insert(originalNoteIndex, firstNote);
            bar.Notes.Insert(originalNoteIndex + 1, secondNote);

            // indicate that a change has been made
            return true;
        }
        #endregion

        #region PermutateNotes()
        /// <summary>
        /// Generates a permutation of the bar's note sequence and replaces the original
        /// sequence with the permutated sequence (for example, shuffled or reversed order).
        /// <para>The available permutations are defined in <see cref="Permutation"/>, 
        /// and the requested permutation should be mentioned in the <paramref name="permutation"/>
        /// input parameter. If no explicit permutation is requested, a default one would take place. </para>
        /// <para> Inorder to maintain the consonance of the notes under the chords which 
        /// are played underthem in parallel, the permutation is done for each chord
        /// individually, i.e., "inplace" in the bounds of the chord associated notes indices. </para>
        /// <para> The <paramref name="chords"/> defines a subsequence of bar's chord sequence
        /// to take action on. If no explicit chords are requested then the method will 
        /// default to permutate the entire bar.</para>
        /// </summary>
        /// <param name="bar"> The bar to operate on.</param>
        /// <param name="chords"> Subsequence of the chords in the bar that are
        /// the target of the permutation. If not set, then the default is to 
        /// permutate all chord in the bar, i.e., the notes of all chords in the bar.</param>
        /// <param name="permutation"> Kind of permutation to apply on the order of the note sequence. </param>
        private protected virtual void PermutateNotes(IBar bar, IEnumerable<IChord> chords = null, Permutation permutation = Permutation.Shuffled)
        {
            /* if no subset of chords has been requested, 
             * set it to the entire bar chord sequence */
            chords = chords ?? bar.Chords;

            foreach (IChord chord in chords)
            {
                // get selected chord's notes and their indices 
                IList<INote> chordNotes = bar.GetOverlappingNotesForChord(chord, out IList<int> chordNotesIndices);

                // get a permutation of the chord's note sequence 
                IList<INote> permutatedChordNotes;
                switch (permutation)
                {
                    case Permutation.Shuffled:
                    default:
                        chordNotes.Shuffle();
                        permutatedChordNotes = chordNotes;
                        break;
                    case Permutation.Reversed:
                        permutatedChordNotes = chordNotes.Reverse().ToList();
                        break;
                    case Permutation.SortedAscending:
                        permutatedChordNotes = chordNotes.Sort(SortOrder.Ascending);
                        break;
                    case Permutation.SortedDescending:
                        permutatedChordNotes = chordNotes.Sort(SortOrder.Descending);
                        break;
                }

                // apply the permutation to the bar's note sequence 
                int noteIndex;
                for (int i = 0; i < chordNotesIndices.Count; i++)
                {
                    noteIndex = chordNotesIndices[i];
                    bar.Notes.RemoveAt(noteIndex);
                    bar.Notes.Insert(noteIndex, permutatedChordNotes[i]);
                }
            }
        }
        #endregion

        #region ToggleAHoldNote()
        /// <summary>
        /// Replaces a random hold note with a concrete note pitch. 
        /// <para> This method selects from within the given bar sequence a bar  
        /// that contains a hold note, and replaces it with a "regular" note 
        /// by setting it's pitch to the adjacent preceding note, if such exists,
        /// or otherwise, to the adjacent succeeding note. </para>
        /// </summary>
        /// <param name="bars"> The bar sequence to operate on. </param>
        /// <param name="barIndex"> Index of a specific requested bar that contains a hold 
        /// note and is requested for a toggle. If this parameter is set to null, or if the 
        /// requested bar does not contain a hold note, then a random bar would be selected,
        /// if such exists (only bars which contain hold notes are relevant).
        /// </param>
        /// <returns> True if a note replacement has been made successfully, false otherwise. </returns>
        private protected virtual bool ToggleAHoldNote(IEnumerable<IBar> bars, int? barIndex)
        {
            // initialization 
            IBar selectedBar;
            int selectedBarIndex;
            IList<IBar> barsWithHoldNotes;
            IList<IBar> barList = bars.ToList();

            /* fetch a bar with hold note according to index from input paramter,
             * or a random one, if no valid bar has been requested.  */

            // if a specific valid bar has been requested, then fectch it directly 
            if (barIndex != null && barList[(int)barIndex].Notes
                .Any(note => note.Pitch == NotePitch.HoldNote))
            {
                selectedBarIndex = (int)barIndex;
                selectedBar = barList[selectedBarIndex];
            }

            // otherwise, select the bar randomly 
            else
            {
                // find all bars which contain hold notes 
                barsWithHoldNotes = bars
                    .Where(bar => bar.Notes.Any(note => note.Pitch == NotePitch.HoldNote)).ToList();

                // assure there at least one bar found 
                if (!barsWithHoldNotes.Any())
                    return false;

                // select a random bar from the collection found
                selectedBar = barsWithHoldNotes[new Random().Next(barsWithHoldNotes.Count)];
                selectedBarIndex = bars.ToList().IndexOf(selectedBar);
            }

            // get the hold note from the selected bar 
            INote holdNote = selectedBar.Notes.Where(note => note.Pitch == NotePitch.HoldNote).First();
            int holdNoteIndex = selectedBar.Notes.IndexOf(holdNote);

            // find adjacent preceding or succeeding sounded note (not a rest or a hold note)
            int adjacentNoteIndex, adjacentNoteBarIndex;
            INote adjacentNote =
                bars.GetPredecessorNote(excludeRestHoldNotes: true, selectedBarIndex, holdNoteIndex, out adjacentNoteIndex, out adjacentNoteBarIndex)
                ??
                bars.GetSuccessorNote(excludeRestHoldNotes: true, selectedBarIndex, holdNoteIndex, out adjacentNoteIndex, out adjacentNoteBarIndex);

            // assure an adjacent note has been found   
            if (adjacentNote != null)
            {
                // replace the hold note with the pitch found 
                INote newNote = new Note(adjacentNote.Pitch, holdNote.Duration);
                selectedBar.Notes.RemoveAt(holdNoteIndex);
                selectedBar.Notes.Insert(holdNoteIndex, newNote);

                // indicate the change has been made successfully
                return true;
            }

            // indicate that no change has been made  
            else return false;
        }
        #endregion

        #region SyncopizeANote()
        /// <summary>
        /// Syncopes a bar's first note by preceding it's start time to it's preceding bar,
        /// on behalf of the duration of it' preceding note (last note from preceding bar).
        /// <para> The bar containing the note to be syncoped must meet some requirments: 
        /// It cannot be empty, nor it's preceding bar either. It can't start with a 
        /// hold note or a rest note, and it's first note and it's preceding note must 
        /// have durations with a denominator which is a power of two. This constraint is 
        /// held inorder to maintain balanced durations when making arithmetic operations 
        /// on the duration of the syncoped note and it's preceding note. If the requested 
        /// bar does not meet these constraints then a random bar which does meet them is selected 
        /// instead. If no such bar is found then this method retuns false. otherwise it returns true.
        /// </para>
        /// </summary>
        /// <param name="bars"> The subject bar sequence to operate on. </param>
        /// <param name="barIndex"> The index of the bar which contains the requested 
        /// note to be syncoped. If no index is mentioned or if the mentioned index is 
        /// invalid in terms of the syncope operation, then a random bar would be selected instead.</param>
        /// <returns> Ture if a successful syncope has been made, false otherwise. </returns>
        private protected virtual bool SyncopizeANote(IList<IBar> bars, int? barIndex = null)
        {
            // initialization 
            IBar selectedBar = null;
            IBar precedingBar = null;
            int selectedBarIndex = -1;

            // if no specific legal bar index is requested then set it randomly  
            if (!barIndex.HasValue || !IsBarLegalForSyncope(bars[(int)barIndex], (int)barIndex))
            {
                // select only bars which meet the operation's requirements 
                IBar[] relevantBars = bars.Where(IsBarLegalForSyncope).ToArray();

                // assure a valid bar for the syncope operation has been found 
                if (!relevantBars.Any())
                    return false;

                // select a bar randomly from the bars found 
                int randomIndex = new Random().Next(relevantBars.Length);
                selectedBar = relevantBars[randomIndex];
                selectedBarIndex = bars.IndexOf(selectedBar);
            }
            else // selected the requested valid bar from the input parameter 
            {
                selectedBarIndex = (int)barIndex;
                selectedBar = bars[selectedBarIndex];
            }

            // fetch the first note from within the selected bar    
            INote originalNote = selectedBar.Notes[0];

            // fetch the preceding note (last note from the preceding bar)
            int precedingBarIndex = selectedBarIndex - 1;
            precedingBar = bars[precedingBarIndex];
            int precedingNoteIndex = bars[precedingBarIndex].Notes.Count - 1;
            INote precedingNote = bars[precedingBarIndex].Notes[precedingNoteIndex];

            /* replace bar's first note with a new hold syncoped note which will hold the 
             * pitch that would now be started early from the preceding bar */
            INote newNote = new Note(NotePitch.HoldNote, originalNote.Duration);
            selectedBar.Notes.RemoveAt(0);
            selectedBar.Notes.Insert(0, newNote);

            /* replace last note from preceding bar with new note that has the original's
             * note pitch, and make original note hold the note from preceding bar: */

            // case 1: preceding note's length is too short for splitting (8th note or shorter) - replace it directly
            if ((precedingNote.Duration.Numerator / (float)precedingNote.Duration.Denominator) <= Duration.EighthNoteFraction)
            {
                INote newPrecedingNote = new Note(originalNote.Pitch, precedingNote.Duration);
                precedingBar.Notes.RemoveAt(precedingNoteIndex);
                precedingBar.Notes.Insert(precedingNoteIndex, newPrecedingNote);
            }

            /* case 2: preceding note is long enough for splitting. 
             * split preceding note into two new notes:
             * the first would retain it's original pitch, and the second 
             * would contain the selected note's pitch from the succeeding bar 
             * inorder to form a syncope.
             * The duration of the second note would be set to an 8th note,
             * and the duration of the first note would be set to the remainding length. */
            else
            {
                INote newPrecedingNote1, newPrecedingNote2;
                //if (precedingNote.Duration.Denominator )
                Duration eigthDuration = new Duration(1, 8);
                newPrecedingNote1 = new Note(precedingNote.Pitch, precedingNote.Duration.Subtract(eigthDuration));
                newPrecedingNote2 = new Note(originalNote.Pitch, eigthDuration);

                // replace the old preceding note with the two new preceding notes 
                precedingBar.Notes.RemoveAt(precedingNoteIndex);
                precedingBar.Notes.Insert(precedingNoteIndex, newPrecedingNote1);
                precedingBar.Notes.Insert(precedingNoteIndex + 1, newPrecedingNote2);
            }

            // return true to indicate a successful change has been made 
            return true;

            #region Local Function - isBarLegalForSyncope()
            //local function for validation candidate bar for syncope
            bool IsBarLegalForSyncope(IBar bar, int index)
            {
                // assure the bar has a preceding bar before it 
                if (index <= 0 || index >= bars.Count)
                    return false;

                // assure the the bar and it's preceding bar are not empty 
                IBar predecessorBar = bars[index - 1];
                if (!bar.Notes.Any() || !predecessorBar.Notes.Any())
                    return false;

                // assure first note of bar is not a rest or hold note 
                if (bar.Notes[0].Pitch == NotePitch.HoldNote ||
                    bar.Notes[0].Pitch == NotePitch.RestNote)
                    return false;

                /* assure bar's first note and it's preceding note from
                 * last the preceding bar have a denominator which is a 
                 * power of two, inorder to keep the durations balanced */
                int predecessorNoteIndex = predecessorBar.Notes.Count - 1;
                INote predecessorNote = predecessorBar.Notes[predecessorNoteIndex];
                if (!bar.Notes[0].Duration.IsDenominatorPowerOfTwo() ||
                    !predecessorNote.Duration.IsDenominatorPowerOfTwo())
                    return false;

                // all required validations passed, return true 
                return true;
            }
            #endregion
        }
        #endregion
    }
}
