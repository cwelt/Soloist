using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // override default local culture settings 
            var englishCulture = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.CurrentCulture = englishCulture;
            CultureInfo.CurrentUICulture = englishCulture;

            Console.WriteLine("Testing Composition Services...");

            // midi (playback) file path 
            string filePath = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\after_20_years_playback.mid";
            string filePath2 = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\‏‏after_20_years.mid";
            string filePath3 = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\testbug.mid";
            string filePath4 = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\debug2.mid";
            string filePath5 = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\test.mid";

            // get chords from file... 
            string chordsFilePath = @"C:\Users\chwel\OneDrive\שולחן העבודה\twenty_years_chords.txt";

            // set the strategy compositor 
            var compositor = new GeneticAlgorithmCompositor();

            // create a new composition with injected strategy
            var composition = new Composition(filePath2, chordsFilePath);
            var newMidiFile = composition.Compose();

            newMidiFile.Play();
            //newMidiFile.Stop();


        }
    }
}
