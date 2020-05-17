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
            note1.Pitch = NotePitch.G9;
            Console.WriteLine("Note name after pitch change: {0}", note1.Name);
        }
    }
}
