﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this._beatsPerBar = TimeSignature.Nominator;
            this._beatsDuration = TimeSignature.Denominator;
            this.Chords = chords;
            this.Notes = notes;
        }

        /// <summary>
        /// <para>Default constructor.</para>
        /// Initializes a bar with a time signature of 4/4, 
        /// an empy list of chords and an empty list of notes.
        /// </summary>
        public Bar() 
            : this (new Duration(4, 4), new List<IChord>(), new List<INote>()) {}

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
