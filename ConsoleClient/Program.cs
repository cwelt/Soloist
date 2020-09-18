using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Composers;
using CW.Soloist.CompositionService.Composers.GeneticAlgorithm;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.Enums;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;

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
            string playbackOnlyfilePath = ConfigurationManager.AppSettings["midiFilePlayback"];

            // source files for testing 
            string filePathTest = ConfigurationManager.AppSettings["midiFileTest"];
            string chordsPathTest = ConfigurationManager.AppSettings["chordsFileTest"];

            // chords file path
            string chordsFilePath = ConfigurationManager.AppSettings["chordsFile"];

            // set the strategy compositor 
            CompositionStrategy compositionStrategy = CompositionStrategy.GeneticAlgorithmStrategy;

            MelodyEvaluatorsWeights evaluatorsWeights = new MelodyEvaluatorsWeights
            {
                AccentedBeats = 30,
                ContourDirection = 10,
                ContourStability = 15,
                DensityBalance = 15,
                ExtremeIntervals = 25,
                PitchRange = 15,
                PitchVariety = 15,
                SmoothMovement = 20,
                Syncopation = 20
            };


            // create a new composition with injected strategy
            var composition = new CompositionContext(chordsFilePath, filePath, melodyTrackIndex: MelodyTrackIndex.TrackNumber1);
            var newMidiFiles = composition.Compose(
                compositionStrategy,
                musicalInstrument: MusicalInstrument.ElectricGrandPiano,
                overallNoteDurationFeel: OverallNoteDurationFeel.Intense,
                pitchRangeSource: PitchRangeSource.Custom,
                minPitch: NotePitch.C3,
                maxPitch: NotePitch.G6,
                useExistingMelodyAsSeed: false,
                customParams: evaluatorsWeights);

            var bestCompositions = newMidiFiles.Take(5).ToArray();
            for (int i = 0; i < bestCompositions.Count(); i++)
            {
                bestCompositions[i].SaveFile(fileNamePrefix: $"consoleTest_{i+1}_");
            }

            bestCompositions.FirstOrDefault()?.FadeOut();
            bestCompositions.FirstOrDefault()?.Play();
        }
    }
}
