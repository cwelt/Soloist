using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="duration"> The Duration of the chord (<see cref="INoteDuration"/>).</param>
        public Chord(NoteName root, ChordType type, IDuration duration)
        {
            ChordRoot = root;
            ChordType = type;
            Duration = duration;
        }

        public IEnumerable<NotePitch> GetNotes(int minOctave, int maxOctave)
        {
            return MidiServices.GetNotes(this.ChordRoot, this.ChordType, minOctave, maxOctave);
        }

    }
}
