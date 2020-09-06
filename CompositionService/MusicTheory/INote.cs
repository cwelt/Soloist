namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Represents a single musical note instance. 
    /// <para>
    /// Each note is represented by a note duration (<see cref="Duration"/>)
    /// and an absolute pitch (<see cref="Pitch"/>), 
    /// which also determines it's name (<see cref="Name"/>).
    /// </para>
    /// </summary>
    public interface INote
    {
        /// <summary>    (<see cref="NoteName"/>) .</summary>
        NoteName? Name { get; }

        /// <summary> The note's absolute pitch (<see cref="NotePitch"/>) .</summary>
        NotePitch Pitch { get; }

        /// <summary> The note's duration (<see cref="Duration"/>) .</summary>
        IDuration Duration { get; }
    }
}
