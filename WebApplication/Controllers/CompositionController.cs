using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using CW.Soloist.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Controllers
{
    public class CompositionController : Controller
    {
        private IMidiFile _midiFile;

        // GET: Composition
        public ActionResult Index()
        {
            CompositionParamsViewModel viewModel = new CompositionParamsViewModel
            {
                CompositionStrategy = CompositionStrategy.GeneticAlgorithmStrategy,
                MusicalInstrument = MusicalInstrument.BrightAcousticPiano,
                OverallNoteDurationFeel = OverallNoteDurationFeel.Medium,
                MinPitch = NotePitch.E2,
                MaxPitch = NotePitch.C6,
                useExistingMelodyAsSeed = false
                
            };
            return View(viewModel);
        }
        [HttpPost]
        public FileResult Compose(CompositionParamsViewModel model)
        {


            Song song = model.Songs.Where(s => s.Id == model.SongId).First();

            var chordFilePath = this.HttpContext.Server.MapPath(song.ChordPath);
            var midiFilePath = this.HttpContext.Server.MapPath(song.MidiPath);

            Composition composition = new Composition(
                chordProgressionFilePath: chordFilePath,
                midiFilePath: midiFilePath,
                melodyTrackIndex: 1);

            IMidiFile midiFile = composition.Compose(
                strategy: model.CompositionStrategy,
                overallNoteDurationFeel: model.OverallNoteDurationFeel,
                musicalInstrument: model.MusicalInstrument,
                minPitch: model.MinPitch,
                maxPitch: model.MaxPitch, 
                useExistingMelodyAsSeed: model.useExistingMelodyAsSeed)[0];

            _midiFile = midiFile;

            ViewBag.MidiFile = midiFile;


            // save file and return it for client to download
            string directoryPath = this.HttpContext.Server.MapPath("/Outputs/");
            //string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Outputs/";

            Directory.CreateDirectory(directoryPath);
            string filePath = _midiFile.SaveFile(directoryPath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}