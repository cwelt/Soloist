﻿using CW.Soloist.CompositionService.MusicTheory;
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

        internal byte ShortestDuration { get; private protected set; } = 16;


        /// <summary> Minimum octave of note pitch range for the composition. </summary>
        public byte MinOctave { get; private protected set; } = 4;

        /// <summary> Maximum octave of note pitch range for the composition. </summary>
        public byte MaxOctave { get; private protected set;  } = 5;

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
        internal IList<IBar> Compose(
            IList<IBar> chordProgression,
            IList<IBar> melodyInitializationSeed = null,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E6)
        {
            InitializeCompositionParams(chordProgression, melodyInitializationSeed, overallNoteDurationFeel, minPitch, maxPitch);

            return GenerateMelody();
        }


        private protected abstract IList<IBar> GenerateMelody();
        private protected virtual void InitializeCompositionParams(
            IList<IBar> chordProgression,
            IList<IBar> melodyInitializationSeed = null,
            OverallNoteDurationFeel overallNoteDurationFeel = OverallNoteDurationFeel.Medium,
            NotePitch minPitch = NotePitch.E2,
            NotePitch maxPitch = NotePitch.E6)
        {
            // initialize general parameters for the algorithm 
            ChordProgression = chordProgression;
            Seed = melodyInitializationSeed;
            MinPitch = minPitch;
            MaxPitch = maxPitch;

            switch (overallNoteDurationFeel)
            {
                case OverallNoteDurationFeel.Slow:
                    DefaultDurationFraction = Duration.QuaterNoteFraction;
                    DefaultDurationDenomniator = Duration.QuaterNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Medium:
                default:
                    DefaultDurationFraction = Duration.EighthNoteFraction;
                    DefaultDurationDenomniator = Duration.EighthNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Intense:
                    DefaultDurationFraction = Duration.SixteenthNoteFraction;
                    DefaultDurationDenomniator = Duration.SixteenthNoteDenominator;
                    break;
                case OverallNoteDurationFeel.Extreme:
                    DefaultDurationFraction = Duration.ThirtySecondNoteFraction;
                    DefaultDurationDenomniator = Duration.ThirtySecondNoteDenominator;
                    break;
            }
            DefaultDuration = new Duration(1, DefaultDurationDenomniator);
        }



        #region NoteSequenceInitializer()
        private protected void NoteSequenceInitializer(IEnumerable<IBar> bars, 
            NoteSequenceMode mode = NoteSequenceMode.BarZigzag,
            ChordNoteMappingSource mappingSource = ChordNoteMappingSource.Chord,
            bool toggleMappingSource = true)
        {
            // durtion length of a single chord
            float chordDurationFraction;

            // number of notes of default duration length that fit in chord's duration length
            byte numberOfNotes;

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

            // populate bars with mapped notes 
            foreach (IBar bar in bars)
            {
                // remove any old existing "garbage" notes 
                bar.Notes.Clear();

                // if BAR zigzag mode is requested, toggle step direction on each bar change 
                if (mode == NoteSequenceMode.BarZigzag)
                    step *= -1;

                foreach (IChord chord in bar.Chords)
                {
                    // if CHORD zigzag mode is requested, toggle step direction on each chord change 
                    if (mode == NoteSequenceMode.ChordZigzag)
                        step *= -1;

                    // calculate amount of notes that fit in current chord 
                    chordDurationFraction = chord.Duration.Numerator / (float)(chord.Duration.Denominator);
                    numberOfNotes = (byte)(chordDurationFraction / DefaultDurationFraction);

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

                    // do the actual note population in current bar from the mapped collection 
                    for (int i = 0; i < numberOfNotes; i++)
                    {
                        bar.Notes.Add(new Note(chordMappedNotes[j], DefaultDuration));
                        if (j == first || j == last)
                            j = middle;
                        else j += step;
                    }

                    // save last played pitch from the last chord
                    prevChordLastPitch = bar.Notes[bar.Notes.Count - 1].Pitch;
                }
            }
        }
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

        #region DurationSplitOfARandomNote()
        /// <summary>
        /// Replaces a random note in the given bar with two new shorter notes 
        /// which preserve the original note's pitch, but have a new duration
        /// which sums up together to the original's note duration. 
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
        private protected virtual bool DurationSplitOfARandomNote(IBar bar, DurationSplitRatio ratio)
        {
            // extract denominator from the given ratio 
            int ratioDenominator = ((ratio == DurationSplitRatio.Equal) ? 2 : 4);

            // find candidate notes which are long enough for the given split ratio 
            INote[] candidateNotes = bar.Notes
                .Where(note => note.Duration.Denominator <= ShortestDuration / ratioDenominator)
                .ToArray();

            // assure there is at least one candidate note which long enough for splitting  
            if (candidateNotes.Length == 0)
                return false;

            // select a random note from within the bar candidate notes 
            int randomNoteIndex = new Random().Next(candidateNotes.Length);
            INote existingNote = candidateNotes[randomNoteIndex];

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

            // create two new notes with preseted durtions 
            INote firstNote = new Note(existingNote.Pitch, firstDuration);
            INote secondNote = new Note(existingNote.Pitch, secondDuration);

            // replace the existing note with the two new notes 
            int originalNoteIndex = bar.Notes.IndexOf(existingNote);
            bar.Notes.RemoveAt(originalNoteIndex);
            bar.Notes.Insert(originalNoteIndex, firstNote);
            bar.Notes.Insert(originalNoteIndex + 1, secondNote);

            // indicate that a change has been made
            return true;
        }
        #endregion


        #region GetPredecessorNote()
        /// <summary> 
        /// <para>Gets the note in the melody which preceds the note at the given indices.</para>
        /// If <paramref name="excludeRestHoldNotes"/> is set, then hold and rest notes  
        /// would be bypassed, and the first preceding note which is not a rest or hold note 
        /// would be returned. If no preceding note is found then null is returned. 
        /// </summary>
        /// <param name="melodyBars"> The bar sequence which contains the melody notes. </param>
        /// <param name="excludeRestHoldNotes">If set, rest notes and hold notes would be discarded during search for a preceding note.</param>
        /// <param name="barIndex"> Index of the bar containing the given note. </param>
        /// <param name="noteIndex"> Index of the note of whom it's predecessor is wanted. </param>
        /// <param name="precedingNoteBarIndex"> Index of the bar which contains the preceding note.</param>
        /// <param name="precedingNoteIndex">Index of the preceding note inside his containing note sequence.</param>
        /// <returns> Preceding note in the melody, or null if no predecessor note is found. </returns>
        private protected INote GetPredecessorNote(IList<IBar> melodyBars, bool excludeRestHoldNotes, int barIndex, int noteIndex, out int precedingNoteBarIndex, out int precedingNoteIndex)
        {
            // initialization 
            INote note = null;
            int startingNoteIndex = 0;

            /* start scanning backwards from current bar & current note:
             * outer loop is for bars, inner loop for notes in the individual bars */
            for (int i = barIndex; i >= 0; i--)
            {
                /* in current bar start searching right before the given note.
                 * in the rest of the bars start from the right edge end of the bar. */
                startingNoteIndex = ((i == barIndex) ? (noteIndex - 1) : (melodyBars[i].Notes.Count - 1));
                for (int j = startingNoteIndex; j >= 0; j--)
                {
                    note = melodyBars[i].Notes[j];
                    if (excludeRestHoldNotes || (note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote))
                    {
                        // set out params with the indices values and return the preceding note 
                        precedingNoteBarIndex = i;
                        precedingNoteIndex = j;
                        return note;
                    }
                }
            }

            // incase no preceding note is found, set the output accordingly 
            precedingNoteBarIndex = -1;
            precedingNoteIndex = -1;
            return null;
        }
        #endregion



        #region GetSuccessorNote()
        /// <summary> 
        /// <para>Gets the note in the melody which succeeds the note at the given indices.</para>
        /// If <paramref name="excludeRestHoldNotes"/> is set, then hold and rest notes  
        /// would be bypassed, and the first succeeding note which is not a rest or hold note 
        /// would be returned. If no succeeding note is found then null is returned. 
        /// </summary>
        /// <param name="melodyBars"> The bar sequence which contains the melody notes. </param>
        /// <param name="excludeRestHoldNotes">If set, rest notes and hold notes would be discarded during search for a preceding note. </param>
        /// <param name="barIndex"> Index of the bar containing the given note. </param>
        /// <param name="noteIndex"> Index of the note of which it's predecessor is wanted. </param>
        /// <param name="succeedingNoteBarIndex">Index of the bar which contains the successor note. </param>
        /// <param name="succeedingNoteIndex">Index of the successor note inside his containing note sequence. </param>
        private protected INote GetSuccessorNote(IList<IBar> melodyBars, bool excludeRestHoldNotes, int barIndex, int noteIndex, out int succeedingNoteBarIndex, out int succeedingNoteIndex)
        {
            // initialization 
            INote note = null;
            int startingNoteIndex = 0;

            // start scanning forwards from current bar & current note 
            for (int i = barIndex; i < melodyBars.Count; i++)
            {
                /* in current bar start searching right after the given note.
                 * in the rest of the bars start from the right beginning of the bar. */
                startingNoteIndex = ((i == barIndex) ? (noteIndex + 1) : 0);
                for (int j = startingNoteIndex; j < melodyBars[i].Notes.Count; j++)
                {
                    note = melodyBars[i].Notes[j];
                    if (excludeRestHoldNotes || (note.Pitch != NotePitch.RestNote && note.Pitch != NotePitch.HoldNote))
                    {
                        // set out params with the indices values and return the succeeding note 
                        succeedingNoteBarIndex = i;
                        succeedingNoteIndex = j;
                        return note;
                    }
                }
            }
            // incase no succeeding note is found, set the output accordingly 
            succeedingNoteBarIndex = -1;
            succeedingNoteIndex = -1;
            return null;
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
    }
}
