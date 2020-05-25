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

            // set the strategy compositor 
            var compositor = new GeneticAlgorithmCompositor();

            // inject the depedency to the composition 
            var composition = new Composition { Compositor = compositor };


            // midi (playback) file path 
            string filePath = @"C:\Users\chwel\source\repos\CW.Soloist\CompositionService\obj\Debug\‏‏after_20_years.mid";

            // get chords from file... 
            string chordsFilePath = @"C:\Users\chwel\OneDrive\שולחן העבודה\twenty_years_chords.txt";



            var newMidiFile = composition.Compose(filePath, chordsFilePath);
            //newMidiFile.Play();
            //newMidiFile.Stop();


        }
    }
}
