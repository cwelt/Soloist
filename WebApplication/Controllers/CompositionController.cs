using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;
using CW.Soloist.WebApplication.Models;
using CW.Soloist.WebApplication.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Compose()
        {
            // fetch id of current logged-in user (if user is indeed logged-in...)
            string userId = User?.Identity?.GetUserId();

            // fetch songs from db according to user's privilages  
            var songs = User?.IsInRole(RoleName.Admin) ?? false
                ? await db.Songs.ToListAsync()
                : await db.Songs.Where(s => s.IsPublic || s.UserId.Equals(userId)).ToListAsync();

            CompositionViewModel viewModel = new CompositionViewModel
            {
                SongSelectList = new SelectList(songs.OrderBy(s => s.Artist).Select(s => new
                {
                    Id = s.Id,
                    Title = s.Artist + " - " + s.Title
                }), "Id", "Title"),
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
        public async Task<ActionResult> Compose(CompositionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CompositionViewModel viewModel = new CompositionViewModel
                {
                    SongSelectList = new SelectList(db.Songs.OrderBy(s => s.Title), "Id", "Title"),
                    PitchSelectList = new SelectList(_pitchSelectList, "Pitch", "Description"),
                };
                return View("Compose", viewModel);
            }

            Song song = db.Songs.Where(s => s.Id == model.SongId)?.First();

            string chordFilePath = await SongsController.GetSongPath(song.Id, db, User, SongFileType.ChordProgressionFile);
            string midiFilePath = await SongsController.GetSongPath(song.Id, db, User, SongFileType.MidiOriginalFile);

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