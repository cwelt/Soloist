using System.Collections.Generic;

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
    public interface IBar
    {
        /// <summary> 
        /// Time signature of current bar which contains the bar's duration: 
        /// number of beats in a bar, and the duration of a single beat, 
        /// for example 4/4 for standard western music and 3/4 for valse. 
        /// </summary>
        IDuration TimeSignature { get; }

        /// <summary> The chords which represent the harmony in the bar. </summary>
        IList<IChord> Chords { get; }

        /// <summary> The notes which represent the melody in the bar. </summary>
        IList<INote> Notes { get; set; }


        /// <summary>
        /// Returns a sublist of the bar's notes which overlap the time interval of the given chord,
        /// i.e, all the notes from <see cref="IBar.Notes"/> that are played in parallel time to 
        /// the given chord.
        /// </summary>
        /// <param name="chordIndex"> Index to the requested chord in this <see cref="IBar.Chords"/> sequence.</param>
        /// <param name="chordNotesIndices"> Indices of the chord notes within the bar's <see cref="Notes"/> sequence. </param>
        /// <returns> List of notes which are played in parallel time to the given chord.</returns>
        IList<INote> GetOverlappingNotesForChord(int chordIndex, out IList<int> chordNotesIndices);


        /// <summary>
        /// <inheritdoc cref="GetOverlappingNotesForChord(int, out IList{int})"/>
        /// </summary>
        /// <param name="chord"> Chord from within this bar's <see cref="IBar.Chords"/> sequence.</param>
        /// <param name="chordNotesIndices">Indices of the chord notes within the bar's <see cref="Notes"/> sequence.</param>
        /// <returns> <inheritdoc cref="GetOverlappingNotesForChord(int, out IList{int})"/> </returns>
        IList<INote> GetOverlappingNotesForChord(IChord chord, out IList<int> chordNotesIndices);




        /// <summary>
        /// Returns a sublist of the bar's chords which overlap the time interval of the given note,
        /// i.e, all the chords from <see cref="IBar.Chords"/> that are played in parallel time to 
        /// the given note.
        /// </summary>
        /// <param name="noteIndex"> Index to the requested note in this <see cref="IBar.Notes"/> note list property.</param>
        /// <returns> List of chords which are played in parallel time to the given note.</returns>
        IList<IChord> GetOverlappingChordsForNote(int noteIndex);

    }
}
