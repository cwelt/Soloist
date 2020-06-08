using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService
{
    public class InternTestProgram
    {
        public static void Main()
        {
            foreach (var chordType in Enum.GetValues(typeof(ChordType)))
            {
                IChord chord = new Chord(NoteName.C, (ChordType)chordType, new Duration());
                Console.Write($"{chordType}: ");
                foreach (var note in chord.GetScaleNotes(4, 4))
                {
                    Console.Write(note);
                }
                Console.WriteLine();
            }

        }
    }
}
