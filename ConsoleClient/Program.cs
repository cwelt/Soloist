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

            // set the strategy compositor 
            var compositor = new GeneticAlgorithmCompositor();

            // inject the depedency to the composition 
            var composition = new Composition { Compositor = compositor };


            // midi (playback) file path 
            string filePath = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\‏‏after_20_years.mid";

            // get chords from file... 



            var newMidiFile = composition.Compose(filePath, "mockChordFile");
            newMidiFile.Play();
            newMidiFile.Stop();
        }
    }
}
