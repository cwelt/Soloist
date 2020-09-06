namespace CW.Soloist.CompositionService.MusicTheory
{

    /// <inheritdoc cref="INote"/>
    internal class Note : INote
    {
        #region Private Internal Data
        // internal static constants 
        private const int NumberOfUniqueChromaticNotes = 12;
        private const int MidiMinimumPitchValue = 0;
        private const int MidiMaximumPitchValue = 127;
        #endregion

        #region Public Properties 
        public IDuration Duration { get; }
        public NoteName? Name { get; }

        public NotePitch Pitch
        {
            get; 
            /*
            get => _pitch;
            set
            {
                _pitch = value;
                _name = GetNoteNameByNotePitch(value);
            }*/
        }
        #endregion

        #region Constructors 
        /// <summary>
        /// <para  > Basic Constructor. </para>
        /// Construct a new <see cref="INote"/> instance with 
        /// the given <paramref name="pitch"/> and <paramref name="duration"/>.
        /// </summary>
        /// <param name="pitch"> MIDI absolute pitch value. </param>
        /// <param name="duration"> Duration of new note. </param>
        public Note(NotePitch pitch, IDuration duration)
        {
            Pitch = pitch;
            Name = GetNoteNameByNotePitch(pitch);
            Duration = new Duration(duration.Numerator, duration.Denominator);
        }

        /// <summary>
        /// Construct a note with given <paramref name="pitch"/> and a 
        /// duration composed with the quotient of the numerator 
        /// divided by the denominator. 
        /// </summary>
        /// <param name="pitch">MIDI absolute pitch value.</param>
        /// <param name="numerator">Duration numerator.</param>
        /// <param name="denominator">Duration denominator.</param>
        public Note(NotePitch pitch, byte numerator, byte denominator)
            : this(pitch, new Duration(numerator, denominator)) { }


        /// <summary> 
        /// Construct a note with given <paramref name="pitch"/> and default duration.
        /// </summary>
        /// <param name="pitch">Absolute pitch of the constructed note. </param>
        public Note(NotePitch pitch)
            : this(pitch, new Duration()) { }


        /// <summary> Copy Constructor </summary>
        /// <param name="note"> The <see cref="INote"/> instance to copy. </param>
        public Note(INote note)
            : this(note.Pitch, note.Duration) { }

        #endregion

        #region Methods

        #region GetNoteNameByNotePitch
        /// <summary>
        /// Returns the <see cref="NoteName"/> which corresponds to the 
        /// given <paramref name="notePitch"/>.
        /// </summary>
        /// <param name="notePitch"> MIDI absolute pitch value</param>
        /// <returns> name of the note represented by the given pitch. </returns>
        private static NoteName? GetNoteNameByNotePitch(NotePitch notePitch)
        {
            // get the enum's underlying value 
            int pitchValue = (int)notePitch;

            // ignore rest (silent) notes & hold notes 
            if (pitchValue < MidiMinimumPitchValue || pitchValue > MidiMaximumPitchValue)
                return null;

            // get the relevant note according to midi pitch values chart  
            int remainder = pitchValue % Note.NumberOfUniqueChromaticNotes;
            switch (remainder)
            {
                case 0: return NoteName.C;
                case 1: return NoteName.CSharp;
                case 2: return NoteName.D;
                case 3: return NoteName.DSharp;
                case 4: return NoteName.E;
                case 5: return NoteName.F;
                case 6: return NoteName.FSharp;
                case 7: return NoteName.G;
                case 8: return NoteName.GSharp;
                case 9: return NoteName.A;
                case 10: return NoteName.ASharp;
                case 11: return NoteName.B;
                default: return null;
            }
        }
        #endregion

        #region ToString
        /// <summary>
        /// Return a comma separated name-value pair list describing the note's state.
        /// </summary>
        /// <returns> A string representing the note's instance. </returns>
        public override string ToString()
        {
            string name = Name?.GetDisplayName() ?? "NIL";
            string pitch = Pitch.GetDisplayName() ?? Pitch.ToString();
            return $"{{Name={name}; Pitch={pitch}; Duration={Duration}}}";
        }
        #endregion

        #endregion
    }
}
