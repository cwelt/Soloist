using CW.Soloist.CompositionService.UtilEnums;
using System.Collections.Generic;

namespace CW.Soloist.CompositionService.MusicTheory
{
    internal class Chord : IChord
    {
        public NoteName ChordRoot { get; }
        public ChordType ChordType { get; }
        public IDuration Duration { get; set; }


        /// <summary>
        /// Construct a new <see cref="IChord"/> instance with the given
        /// <paramref name="root"/>, <paramref name="type"/> and <paramref name="duration"/>. 
        /// </summary>
        /// <param name="root"> The name of the note which is the chord's root. </param>
        /// <param name="type"> The type of the chord (<see cref="ChordType"/>).</param>
        /// <param name="duration"> The Duration of the chord (<see cref="IDuration"/>).</param>
        public Chord(NoteName root, ChordType type, IDuration duration)
        {
            ChordRoot = root;
            ChordType = type;
            Duration = duration;
        }

        public IEnumerable<NotePitch> GetArpeggioNotes(int minOctave, int maxOctave)
        {
            return MusicTheoryServices.GetNotes(this, ChordNoteMappingSource.Chord, minOctave, maxOctave);
        }

        public IEnumerable<NotePitch> GetScaleNotes(int minOctave, int maxOctave)
        {
            return MusicTheoryServices.GetNotes(this, ChordNoteMappingSource.Scale, minOctave, maxOctave);
        }

        public override string ToString()
        {
            return $"{{Root={ChordRoot}; ChordType={ChordType}; Duration={Duration}}}";
        }

    }
}
