using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Configuration;
using System.Globalization;

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
            var newMidiFiles = composition.Compose(
                compositionStrategy,
                musicalInstrument: MusicalInstrument.VoiceOohs,
                overallNoteDurationFeel: OverallNoteDurationFeel.Medium,
                pitchRangeSource: PitchRangeSource.MidiFile,
                minPitch: NotePitch.E2,
                maxPitch: NotePitch.E6,
                useExistingMelodyAsSeed: true);

            for (int i = 0; i < newMidiFiles.Length; i++)
            {
                newMidiFiles[i].SaveFile(fileNamePrefix: $"consoleTest_{i+1}_");
            }

            newMidiFiles[0].Play();
            //newMidiFile.Stop();
        }
    }
}
