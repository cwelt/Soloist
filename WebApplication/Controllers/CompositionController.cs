using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;
using CW.Soloist.WebApplication.ViewModels;
using SoloistWebClient.Controllers;
using System;
using System.Collections;
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
        private SoloistContext db = new SoloistContext();

        // GET: Composition
        public ActionResult Index()
        {
            CompositionParamsViewModel viewModel = new CompositionParamsViewModel
            {
                Songs = db.Songs.ToList(),
                SongSelectList = new SelectList(db.Songs.OrderBy(s => s.Title), "Id", "Title"),
                CompositionStrategy = CompositionStrategy.GeneticAlgorithmStrategy,
                MusicalInstrument = MusicalInstrument.ElectricGrandPiano,
                OverallNoteDurationFeel = OverallNoteDurationFeel.Medium,
                MinPitch = NotePitch.G4,
                MaxPitch = NotePitch.C6,
                useExistingMelodyAsSeed = true
                
            };

            @ViewBag.Title = "Compose!!!";
            return View(viewModel);
        }
        [HttpPost]
        public FileResult Compose(CompositionParamsViewModel model)
        {
            Song song = db.Songs.Where(s => s.Id == model.SongId)?.First();

            var path = HomeController.GetFileServerPath();
            path += $@"Songs\{song.Id}\";
            var chordFilePath = path + song.ChordsFileName;
            var midiFilePath = path + song.MidiFileName;

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
            string directoryPath = $@"{HomeController.GetFileServerPath()}Outputs\{song.Id}\";
            //string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Outputs/";

            Directory.CreateDirectory(directoryPath);
            string filePath = _midiFile.SaveFile(directoryPath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}