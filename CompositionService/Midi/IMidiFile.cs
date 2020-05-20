using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    interface IMidiFile
    {

        string Title { get; set; }
        int BeatsPerMinute { get; set; }
        int NumberOfBars {get; set; }
        ICollection<object> Tracks { get; set; }
        ICollection<object> Instruments { get; set; }

        void Play();
        void Stop();

    }
}
