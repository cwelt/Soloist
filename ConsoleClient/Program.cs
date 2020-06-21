using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Compositors.GeneticAlgorithm;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
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
            var composition = new Composition(chordsFilePath, filePath, melodyTrackIndex: 1);
            var newMidiFile = composition.Compose(
                compositionStrategy,
                musicalInstrument: MusicalInstrument.AcousticGrandPiano,
                overallNoteDurationFeel: OverallNoteDurationFeel.Extreme,
                pitchRangeSource: PitchRangeSource.Custom,
                minPitch: NotePitch.E2,
                maxPitch: NotePitch.E8,
                useExistingMelodyAsSeed: false);

            newMidiFile.Play();
            //newMidiFile.Stop();
        }
    }
}
