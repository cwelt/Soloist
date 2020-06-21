using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using CW.Soloist.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Controllers
{
    public class CompositionController : Controller
    {
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

            midiFile.Play();
            
            return null;
        }
    }
}