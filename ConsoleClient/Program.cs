using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.CompositionStrategies;
using CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy;
using CW.Soloist.CompositionService.Midi;
using System;
using System.Configuration;
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
            string filePath = ConfigurationManager.AppSettings["midiFile"];
            string filePathTest = ConfigurationManager.AppSettings["midiFileTest"];
            string playbackOnlyfilePath = ConfigurationManager.AppSettings["midiFilePlayback"];

            // get chords from file... 
            string chordsFilePath = ConfigurationManager.AppSettings["chordsFile"];

            // set the strategy compositor 
            CompositionStrategy compositionStrategy = CompositionStrategy.GeneticAlgorithmStrategy;

            // create a new composition with injected strategy
            var composition = new Composition(filePath, chordsFilePath);
            var newMidiFile = composition.Compose(compositionStrategy, MusicalInstrument.AltoSax);

            newMidiFile.Play();
            //newMidiFile.Stop();


        }
    }
}
