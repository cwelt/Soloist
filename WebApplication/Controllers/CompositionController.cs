﻿using CW.Soloist.CompositionService;
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
using System.Web.UI.WebControls;

namespace CW.Soloist.WebApplication.Controllers
{
    public class CompositionController : Controller
    {
        private IMidiFile _midiFile;
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnumerable<PitchRecord> _pitchSelectList;

        public CompositionController()
        {
            // build pitch selection list without hold and rest notes 
            _pitchSelectList = Enum.GetValues(typeof(NotePitch)).Cast<NotePitch>()
               .Except(new[] { NotePitch.HoldNote, NotePitch.RestNote })
               .Select(notePitch => new PitchRecord
               { 
                   Pitch = notePitch, 
                   Description = notePitch.GetDisplayName() 
               });
        }

        // GET: Composition
        [HttpGet]
        public ActionResult Compose()
        {
            CompositionParamsViewModel viewModel = new CompositionParamsViewModel
            {
                SongSelectList = new SelectList(db.Songs.OrderBy(s => s.Title), "Id", "Title"),
                MusicalInstrument = MusicalInstrument.ElectricGrandPiano,
                OverallNoteDurationFeel = OverallNoteDurationFeel.Medium,
                PitchSelectList = new SelectList(_pitchSelectList, "Pitch", "Description"),
                MinPitch = NotePitch.C4,
                MaxPitch = NotePitch.G6,
                useExistingMelodyAsSeed = false
            };

            @ViewBag.Title = "Compose";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compose(CompositionParamsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CompositionParamsViewModel viewModel = new CompositionParamsViewModel
                {
                    SongSelectList = new SelectList(db.Songs.OrderBy(s => s.Title), "Id", "Title"),
                    PitchSelectList = new SelectList(_pitchSelectList, "Pitch", "Description"),
                };
                return View("Compose", viewModel);
            }

            Song song = db.Songs.Where(s => s.Id == model.SongId)?.First();

            var path = HomeController.GetFileServerPath();
            path += $@"Songs\{song.Id}\";
            var chordFilePath = path + song.ChordsFileName;
            var midiFilePath = path + song.MidiFileName;

            Composition composition = new Composition(
                chordProgressionFilePath: chordFilePath,
                midiFilePath: midiFilePath,
                melodyTrackIndex: (byte?)song.MelodyTrackIndex);

            // Compose some melodies and fetch the first one 
            IMidiFile midiFile = composition.Compose(
                strategy: CompositionStrategy.GeneticAlgorithmStrategy,
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

        private class PitchRecord
        {
            public NotePitch Pitch { get; set; }
            public string Description { get; set; }
        }
    }
}