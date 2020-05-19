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
            IDuration durationHalf = new Duration(1, 2);
            IDuration durationQuarter = new Duration(1, 4);
            INote noteA = new Note(NotePitch.A4, durationHalf);
            Console.WriteLine(noteA);
            Console.WriteLine(durationQuarter);
            var chord = new Chord(NoteName.C, ChordType.Major7, durationHalf);
            Console.WriteLine(chord);

            var bar = new Bar(new Duration(4, 4), new List<IChord>() { chord, new Chord(NoteName.ASharp, ChordType.Dominant7Suspended4, new Duration()) });
            Console.WriteLine(bar);

        }
    }
}
