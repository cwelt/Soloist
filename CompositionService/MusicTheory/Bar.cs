using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a single musical bar in a musical piece. 
    /// <para>
    /// Each bar contains a time signature (<see cref="IDuration"/>) which 
    /// represents the number of beats in the bar and the duration of each beat,
    /// a list of chords (<see cref="IChord"/>) which represent the harmony in
    /// the current bar, and a list of notes (<see cref="INote"/>).
    /// </para>
    /// </summary>
    internal class Bar : IBar
    {
        #region Internal private members 

        /// <summary>Number of beats in the bar (nominator from the time signature property). </summary>
        byte _beatsPerBar;

        /// <summary>Duration of a single beat in the bar (denominator from the time signature property). </summary>
        byte _beatsDuration;

        #endregion

        #region IBar properties 
        public IDuration TimeSignature { get; }
        public IList<IChord> Chords { get; }
        public IList<INote> Notes { get; set; }
        #endregion

        #region Constructors 
        /// <summary>
        /// Constructs a bar instance based on the given <paramref name="timeSignature"/>,
        /// <paramref name="chords"/> and <paramref name="notes"/>.
        /// </summary>
        /// <param name="timeSignature">Time signature of this bar.</param>
        /// <param name="chords"> Chord progression harmony for this bar.</param>
        /// <param name="notes"> Melody notes for this bar. </param>
        public Bar(IDuration timeSignature, IList<IChord> chords, IList<INote> notes)
        {
            this.TimeSignature = timeSignature;
            this._beatsPerBar = TimeSignature.Numerator;
            this._beatsDuration = TimeSignature.Denominator;
            this.Chords = chords;
            this.Notes = notes;
        }

        /// <summary>
        /// <para>Default constructor.</para>
        /// Initializes a bar with a default time signature of 4/4, 
        /// an empy list of chords and an empty list of notes.
        /// </summary>
        public Bar()
            : this(new Duration(4, 4), new List<IChord>(), new List<INote>()) { }

        /// <summary>
        /// Constructs an empty bar with the given time signature. />
        /// </summary>
        /// <param name="timeSignature"></param>
        public Bar(Duration timeSignature)
            : this(timeSignature, new List<IChord>(), new List<INote>()) { }

        /// <summary>
        /// Constructs a bar instance based on the given <paramref name="timeSignature"/>
        /// and chord progression (<paramref name="chords"/>). 
        /// The <see cref="Notes"/> property is initialized to an empty list. 
        /// </summary>
        /// <param name="timeSignature"></param>
        /// <param name="chords"></param>
        public Bar(IDuration timeSignature, IList<IChord> chords)
             : this(timeSignature, chords, new List<INote>()) { }

        /// <summary>
        /// <para> Copy constructor. </para>
        ///  Makes a shallow copy of the chords progression (<see cref="Chords"/>)
        ///  since it is intended to be immutable\read-only any ways, 
        ///  and a deep copy of the time signature (<see cref="TimeSignature"/>)
        ///  and notes (<see cref="Notes"/>). 
        /// </summary>
        /// <param name="bar"></param>
        public Bar(IBar bar)
            : this(new Duration(bar.TimeSignature), bar.Chords)
        {
            this.Notes = new List<INote>(bar.Notes.Count);
            foreach (INote note in bar.Notes)
                this.Notes.Add(new Note(note));
        }
        #endregion


        /// <inheritdoc cref="IBar.GetOverlappingNotesForChord(int, out IList{int})"/>
        public IList<INote> GetOverlappingNotesForChord(int chordIndex, out IList<int> chordNotesIndices)
        {
            // initialize empty result list 
            IList<INote> chordNotes = new List<INote>();
            chordNotesIndices = new List<int>();

            // initialize start & end time trackers as a fraction relative to this bar's duration 
            float chordStartTime = 0, chordEndTime = 0;
            float noteStartTime = 0, noteEndTime = 0;

            // calculate starting point for the given chord inside this bar instance  
            for (int i = 0; i < chordIndex; i++)
                chordStartTime += (float)Chords[i].Duration.Numerator / Chords[i].Duration.Denominator;

            // calcualte the ending point for the given chord inside this bar instance   
            chordEndTime = chordStartTime + ((float)Chords[chordIndex].Duration.Numerator / Chords[chordIndex].Duration.Denominator);

            // add all notes that overlap the chord's time interval 
            for (int i = 0; i < Notes.Count; i++)
            {
                noteStartTime = noteEndTime;
                noteEndTime = noteStartTime + ((float)Notes[i].Duration.Numerator / Notes[i].Duration.Denominator);
                if (noteStartTime < chordEndTime && noteEndTime > chordStartTime)
                {
                    chordNotes.Add(Notes[i]);
                    chordNotesIndices.Add(i);
                }
            }

            // return overlapping notes & their corresponding indices
            return chordNotes;
        }

        /// <inheritdoc cref="IBar.GetOverlappingNotesForChord(IChord, out IList{int})"/>
        public IList<INote> GetOverlappingNotesForChord(IChord chord, out IList<int> chordNotesIndices)
        {
            // assure the chord argument exists in this bar's instance chord sequence.  
            int chordIndex = Chords.IndexOf(chord);

            // if valid, redirect to overloaded version which accepts chord's index
            if (chordIndex >= 0)
                return GetOverlappingNotesForChord(chordIndex, out chordNotesIndices);
            
            else // invalid, return empty note sequence
            {
                chordNotesIndices = null;
                return null;
            }
        }

        /// <inheritdoc cref="IBar.GetOverlappingChordsForNote(int)"/>
        public IList<IChord> GetOverlappingChordsForNote(int noteIndex)
        {
            // initialize empty result list 
            IList<IChord> noteChords = new List<IChord>();

            // initialize start & end time trackers as a fraction relative to this bar's duration 
            float noteStartTime = 0, noteEndTime = 0;
            float chordStartTime = 0, chordEndTime = 0;

            // calculate starting point for the given note inside this bar instance  
            for (int i = 0; i < noteIndex; i++)
                noteStartTime += (float)Notes[i].Duration.Numerator / Notes[i].Duration.Denominator;

            // calcualte the ending point for the given note inside this bar instance   
            noteEndTime = noteEndTime + ((float)Notes[noteIndex].Duration.Numerator / Notes[noteIndex].Duration.Denominator);

            // add all notes that overlap the chord's time interval 
            foreach (IChord chord in Chords)
            {
                chordStartTime = chordEndTime;
                chordEndTime = chordEndTime + ((float)chord.Duration.Numerator / chord.Duration.Denominator);
                if (chordStartTime < noteEndTime && chordEndTime > noteStartTime)
                    noteChords.Add(chord);
            }

            // return overlapping chords 
            return noteChords;
        }



        public override string ToString()
        {
            // time signature 
            string timeSignature = $"{{TimeSignature={TimeSignature}; ";

            // chords 
            StringBuilder chords = new StringBuilder();
            chords.Append("Chords={");
            for (int i = 0; i < Chords.Count - 1; i++)
                chords.Append(Chords[i] + ",");
            chords.Append(Chords[Chords.Count - 1] + "}; ");



            // notes 
            StringBuilder notes = new StringBuilder();
            chords.Append("Notes={");
            for (int i = 0; i < Notes.Count - 1; i++)
                notes.Append(Notes[i] + ",");
            notes.Append(Notes[Notes.Count - 1] + "}; ");

            // assemble & return the result   
            return timeSignature + chords + notes + "}";
        }


    }
}
