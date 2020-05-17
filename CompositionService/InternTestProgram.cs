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
            INoteDuration duration = new NoteDuration(1, 2);
            Console.WriteLine("Duration printout: {0}", duration);

            INote note1 = new Note(NotePitch.C4, duration);
            Console.WriteLine("Note name before pitch change: {0}", note1.Name);
            Console.WriteLine(note1);
            note1.Pitch = NotePitch.G9;
            Console.WriteLine("Note name after pitch change: {0}", note1.Name);
            Console.WriteLine(note1);

            var note2 = new Note(note1);
            note2.Pitch = NotePitch.RestNote;
            note2.Duration = new NoteDuration();
            Console.WriteLine("\nprinting after duplication");

            Console.WriteLine("note1" + note1 + "\n\nNote 2" + note2);
            var note3 = new Note(NotePitch.HoldNote);
            Console.WriteLine(note3);
        }
    }
}
