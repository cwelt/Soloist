using CsQuery.ExtensionMethods.Internal;
using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Compositors.GeneticAlgorithm;
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
        public async Task<ActionResult> Compose(int? songId = null)
        {
            // fetch id of current logged-in user (if user is indeed logged-in...)
            string userId = User?.Identity?.GetUserId();

            // fetch songs from db according to user's privilages  
            List<Song> authorizedSongs = User?.IsInRole(RoleName.Admin) ?? false
                ? await db.Songs.ToListAsync()
                : await db.Songs.Where(s => s.IsPublic || s.UserId.Equals(userId)).ToListAsync();

            // project artist and song name from song list and sort the list accordingly
            var sortedProjectedSongs = authorizedSongs.Select(s => new
            {
                Id = s.Id,
                Title = s.Artist + " - " + s.Title
            }).OrderBy(s => s.Title).ToList();

            // if user requested a specific song, select it
            int? selectedSongId = authorizedSongs.Exists(s => s.Id == songId)
                ? songId
                : authorizedSongs.FirstOrDefault()?.Id;

            // build select list for the songs
            SelectList songSelectList = new SelectList(sortedProjectedSongs, "Id", "Title", songId);

            // build a view model with the prebuilt song list 
            CompositionViewModel viewModel = new CompositionViewModel
            {
                SongSelectList = songSelectList,
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
            // validate that sum of weights is equal to 100
            double weightSum = model.WeightSum;
            if (weightSum != 100)
            {
                string errorMessage =
                    $"The total weights of all fitness function " +
                    $"evaluators must sum up to 100.\n" +
                    $"The current sum is {weightSum}";
                this.ModelState.AddModelError(nameof(model.AccentedBeats), errorMessage);
            }
            
            // assure all validations are okay 
            if (!ModelState.IsValid)
            {
                CompositionViewModel viewModel = new CompositionViewModel
                {
                    SongSelectList = new SelectList(db.Songs.OrderBy(s => s.Title), "Id", "Title"),
                    PitchSelectList = new SelectList(_pitchSelectList, "Pitch", "Description"),
                };
                return View("Compose", viewModel);
            }

            // fetch song from datasource  
            Song song = db.Songs.Where(s => s.Id == model.SongId)?.First();

            // get the chord and file paths on the file server
            string chordFilePath = await SongsController.GetSongPath(song.Id, db, User, SongFileType.ChordProgressionFile);
            string midiFilePath = await SongsController.GetSongPath(song.Id, db, User, SongFileType.MidiOriginalFile);

            // create a compositon instance 
            Composition composition = new Composition(
                chordProgressionFilePath: chordFilePath,
                midiFilePath: midiFilePath,
                melodyTrackIndex: song.MelodyTrackIndex);

            // build evaluators weight 
            MelodyEvaluatorsWeights weights = new MelodyEvaluatorsWeights
            {
                AccentedBeats = model.AccentedBeats,
                ContourDirection = model.ContourDirection,
                ContourStability = model.ContourStability,
                DensityBalance = model.DensityBalance,
                ExtremeIntervals = model.ExtremeIntervals,
                PitchRange = model.PitchRange,
                PitchVariety = model.PitchVariety,
                SmoothMovement = model.SmoothMovement,
                Syncopation = model.Syncopation
            };

            // Compose some melodies and fetch the first one 
            IMidiFile midiFile = composition.Compose(
                strategy: CompositionStrategy.GeneticAlgorithmStrategy,
                overallNoteDurationFeel: model.OverallNoteDurationFeel,
                musicalInstrument: model.MusicalInstrument,
                minPitch: model.MinPitch,
                maxPitch: model.MaxPitch,
                useExistingMelodyAsSeed: model.useExistingMelodyAsSeed,
                customParams: weights)
                .FirstOrDefault();

            // save the midifile output internally 
            _midiFile = midiFile;
            ViewBag.MidiFile = midiFile;

            // save file on the file server  
            string directoryPath = $@"{HomeController.GetFileServerPath()}Outputs\{song.Id}\";
            Directory.CreateDirectory(directoryPath);
            string filePath = _midiFile.SaveFile(directoryPath);

            // return the file to the client client for downloading 
            string fileName = Path.GetFileName(filePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private class PitchRecord
        {
            public NotePitch Pitch { get; set; }
            public string Description { get; set; }
        }
    }
}