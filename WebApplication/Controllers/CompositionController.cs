using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using CW.Soloist.DataAccess;
using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.Enums;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.WebApplication.ViewModels;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Compositors.GeneticAlgorithm;

namespace CW.Soloist.WebApplication.Controllers
{
    /// <summary>
    /// Controller responsible for managing requests for composing melodies.
    /// </summary>
    public class CompositionController : Controller
    {
        #region Private Instance Fields
        private IMidiFile _midiFile; // used for the composed melody output file
        private IUnitOfWork _databaseGateway; // abstract gateway to the underlying database
        private IEnumerable<PitchRecord> _pitchSelectList; // internal sequence for structing data to view  
        #endregion

        #region Constructor
        /// <summary> Constructs a composition controller by injecting a 
        /// instance of the unit of work dependency. </summary>
        /// <param name="unitOfWork"> database gateway that manages the in-memory updates to persisted entities.</param>
        public CompositionController(IUnitOfWork unitOfWork)
        {
            // use the injected dependency on the abstract unit of work
            _databaseGateway = unitOfWork;

            // build pitch selection list without hold and rest notes 
            _pitchSelectList = Enum.GetValues(typeof(NotePitch)).Cast<NotePitch>()
               .Except(new[] { NotePitch.HoldNote, NotePitch.RestNote })
               .Select(notePitch => new PitchRecord
               {
                   Pitch = notePitch,
                   Description = notePitch.GetDisplayName()
               });
        }
        #endregion

        #region Compose;    GET: Composition/Compose/<optional songId>
        [HttpGet]
        public async Task<ActionResult> Compose(int? songId = null)
        {
            // fetch id of current logged-in user (if user is indeed logged-in...)
            string userId = User?.Identity?.GetUserId();

            // fetch songs from db according to user's privilages  
            List<Song> authorizedSongs = User?.IsInRole(RoleName.Admin) ?? false
                ? await _databaseGateway.Songs.GetAllAsync()
                : await _databaseGateway.Songs.FindAsync(s => s.IsPublic || s.UserId.Equals(userId));

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

            // pass the view model to the view to render
            return View(viewModel);
        }
        #endregion

        #region Compose;    POST: Composition/Compose
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
            
            // assure all other validations passed okay 
            if (!ModelState.IsValid)
            {
                CompositionViewModel viewModel = new CompositionViewModel
                {
                    SongSelectList = new SelectList(_databaseGateway.Songs.GetAll().OrderBy(s => s.Title), nameof(Song.Id), nameof(Song.Title)),
                    PitchSelectList = new SelectList(_pitchSelectList, nameof(PitchRecord.Pitch), nameof(PitchRecord.Description)),
                };
                return View(viewModel);
            }

            // fetch song from datasource  
            Song song = _databaseGateway.Songs.Get(model.SongId);

            // get the chord and file paths on the file server
            string chordFilePath = await SongsController.GetSongPath(song.Id, _databaseGateway, User, SongFileType.ChordProgressionFile);
            string midiFilePath = await SongsController.GetSongPath(song.Id, _databaseGateway, User, SongFileType.MidiOriginalFile);

            // create a compositon instance 
            CompositionContext composition = new CompositionContext(
                chordProgressionFilePath: chordFilePath,
                midiFilePath: midiFilePath,
                melodyTrackIndex: song.MelodyTrackIndex);

            // build evaluators weights 
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

            // Compose some melodies and fetch the best one 
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
            string directoryPath = HomeController.GetFileServerPath() + "Outputs" + 
                Path.DirectorySeparatorChar + song.Id + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(directoryPath);
            string filePath = _midiFile.SaveFile(directoryPath);

            // return the file to the client client for downloading 
            string fileName = Path.GetFileName(filePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion

        #region GetOutputParentPath
        /// <summary>
        /// Gets the parent path on the hosting file server that contains the output 
        /// midi files of the generated composed melodies for all songs.
        /// </summary>
        /// <returns> The path of the parent output directory on the hosting file server. </returns>
        internal static string GetOutputParentPath()
        {
            // returns ~<FileServerPath>\Outputs\
            return HomeController.GetFileServerPath() + "Outputs" + Path.DirectorySeparatorChar;
        }
        #endregion

        #region GetSongOutputPath
        /// <summary>
        /// Gets the path on the hosting file server that contains the output 
        /// midi files of the generated composed melodies for the given song.
        /// </summary>
        /// <param name="songId">The id of the subject song.</param>
        /// <returns> The path of the given song output directory on the hosting file server. </returns>
        internal static string GetSongOutputPath(int songId)
        {
            // returns ~<Parent Path>\<songId>\
            return GetOutputParentPath() + songId + Path.DirectorySeparatorChar;
        }
        #endregion

        #region PitchRecord Internal Private Class
        /// <summary>
        /// Private internal class used for structing pitches for selection in views.
        /// </summary>
        private class PitchRecord
        {
            public NotePitch Pitch { get; set; }
            public string Description { get; set; }
        }
        #endregion
    }
}