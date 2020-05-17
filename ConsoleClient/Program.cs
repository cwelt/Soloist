using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy;
using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            var geneticAlgorithmCompositor = new GeneticAlgorithmCompositor();
            var composition = new Composition { Compositor = geneticAlgorithmCompositor };
            composition.Compositor.Compose("demofile.mid", new object[] { "chord1", "chord2"});

            InternTestProgram.Main();
        }
    }
}
