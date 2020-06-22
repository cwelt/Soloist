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
                MusicalInstrument = MusicalInstrument.AcousticGrandPiano,
                OverallNoteDurationFeel = OverallNoteDurationFeel.Extreme,
                MinPitch = NotePitch.E2,
                MaxPitch = NotePitch.E7
            };
            return View(viewModel);
        }
        [HttpPost]
        public FileResult Compose(CompositionParamsViewModel model)
        {
            var chordFilePath = this.HttpContext.Server.MapPath("/SampleData/twenty_years_chords.txt");
            var midiFilePath = this.HttpContext.Server.MapPath("/SampleData/after_20_years.mid");

            Composition composition = new Composition(chordFilePath, midiFilePath, 1);
            IMidiFile midiFile = composition.Compose(
                strategy: CompositionStrategy.ArpeggioScaleMixStrategy,
                overallNoteDurationFeel: model.OverallNoteDurationFeel,
                musicalInstrument: model.MusicalInstrument,
                minPitch: model.MinPitch,
                maxPitch: model.MaxPitch);

            _midiFile = midiFile;

            ViewBag.MidiFile = midiFile;


            // save file and return it for client to download
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Outputs/";
            string filePath = _midiFile.SaveFile(directoryPath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DownloadFile(IMidiFile midiFile)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Outputs/";

            string filePath = _midiFile.SaveFile(directoryPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}