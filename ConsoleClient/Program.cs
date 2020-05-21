using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy;
using System;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Composition Services...");

            var geneticAlgorithmCompositor = new GeneticAlgorithmCompositor();
            var composition = new Composition { Compositor = geneticAlgorithmCompositor };



            string filePath = @"C:\Users\chwel\OneDrive\שולחן העבודה\Soloist\Soloist\Soloist\bin\Debug\‏‏after_20_years.mid";

            var newMidiFile = composition.Compose(filePath, "mockChordFile");
            newMidiFile.Play();
        }
    }
}
