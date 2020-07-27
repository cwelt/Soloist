using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Collections.Generic;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;
using CW.Soloist.WebApplication.Controllers;
using CW.Soloist.WebApplication.Models;
using CW.Soloist.WebApplication.ViewModels;
using CW.Soloist.CompositionService.Midi;

using CW.Soloist.CompositionService;

namespace CW.Soloist.WebApplication.Controllers
{
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string fileServerPath = HomeController.GetFileServerPath();

        #region Index
        // GET: Songs
        public async Task<ActionResult> Index(string message = null)
        {
            // fetch id of current logged-in user (if user is indeed logged-in...)
            string userId = User?.Identity?.GetUserId();

            // fetch songs from db according to user's privilages  
            var songs = User?.IsInRole(RoleName.Admin) ?? false
                ? await db.Songs.ToListAsync()
                : await db.Songs.Where(s => s.IsPublic || s.UserId.Equals(userId)).ToListAsync();

            // transfer db song list from to deticated DTO song list 
            List<SongViewModel> songsViewModel = new List<SongViewModel>(songs.Count);
            foreach (Song song in songs)
            {
                songsViewModel.Add(new SongViewModel(song)
                {
                    IsUserAuthorizedToEdit = IsUserAuthorized(song, AuthorizationActivity.Update),
                    IsUserAuthorizedToDelete = IsUserAuthorized(song, AuthorizationActivity.Delete),
                });
            }

            // if a message was passed for display, pass it to view 
            if (message != null) ViewBag.Message = message;

            // pass the song list for the view for rendering 
            return View(songsViewModel);
        }
        #endregion

        #region Details
        // GET: Songs/Details/5
        public async Task<ActionResult> Details(int? id, string message = null)
        {
            // assure id parameter is valid 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // fetch song from the database
            Song song = await db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }

            // check authorization 
            if (!IsUserAuthorized(song, AuthorizationActivity.Display))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // build a DTO view model of the song for the view to render 
            SongViewModel songViewModel = new SongViewModel(song);

            // add edit authorization data 
            songViewModel.IsUserAuthorizedToEdit = IsUserAuthorized(song, AuthorizationActivity.Update);
            songViewModel.IsUserAuthorizedToDelete = IsUserAuthorized(song, AuthorizationActivity.Delete);

            // try reading the chords & midi files and adding their content to the view model 
            string chordsFilePath = await GetSongPath(song.Id, SongFileType.ChordProgressionFile);
            string midiFilePath = await GetSongPath(song.Id, SongFileType.MidiOriginalFile);
            try
            {
                songViewModel.ChordProgression = System.IO.File.ReadAllText(chordsFilePath);
                songViewModel.MidiData = Composition.ReadMidiFile(midiFilePath);
            }
            catch (Exception ex) 
            {
                HomeController.WriteErrorToLog(HttpContext, ex.Message);
            }
            finally
            {
                songViewModel.MidiData?.Stream?.Dispose();
            }

            // if a message exists add it to view 
            songViewModel.StatusMessage = message;

            // pass the view model to the view to render 
            return View(songViewModel);
        }
        #endregion

        #region // GET: Songs/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            SongViewModel songViewModel = new SongViewModel
            {
                /* check if user is admin or not for indicating the privliage 
                 * of setting a song as public or private */
                IsAdminUser = User?.IsInRole(RoleName.Admin) ?? false
            };
            return View(songViewModel);
        }
        #endregion

        #region // POST: Songs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(SongViewModel songViewModel)
        {
            if (ModelState.IsValid)
            {
                // get current timestamp
                DateTime timestamp = DateTime.Now;

                // Create a new song instance based on the form data from the view model
                Song song = new Song
                {
                    Created = timestamp,
                    Modified = timestamp,
                    Title = songViewModel.Title,
                    Artist = songViewModel.Artist,
                    MidiFileName = songViewModel.MidiFileHandler.FileName,
                    ChordsFileName = songViewModel.ChordsFileHandler.FileName,
                    MelodyTrackIndex = songViewModel.MelodyTrackIndex,
                    IsPublic = songViewModel.IsPublic && User.IsInRole(RoleName.Admin),
                    UserId = this.User.Identity.GetUserId(),
                };

                // set name for a playback file 
                song.SetPlaybackName();

                // save the new song in the database 
                db.Songs.Add(song);
                await db.SaveChangesAsync();

                // save the files of the new song on the file server 
                IMidiFile playbackFile = null;
                try
                {
                    // create a new directory for the new song 
                    string directoryPath = await GetSongPath(song.Id);
                    Directory.CreateDirectory(directoryPath);

                    // save the midi file in the new directory 
                    string midiFileFullPath = directoryPath + song.MidiFileName;
                    songViewModel.MidiFileHandler.SaveAs(midiFileFullPath);

                    // save the midi playback file in the new directory 
                    string midiPlaybackFullPath = directoryPath + song.MidiPlaybackFileName;
                    playbackFile = Composition.CreateMidiPlayback(songViewModel.MidiFileHandler.InputStream, song.MelodyTrackIndex);
                    playbackFile.SaveFile(outputPath: midiPlaybackFullPath, pathIncludesFileName: true);

                    // save the chord progression file in the new directory 
                    string chordsFilefullPath = directoryPath + song.ChordsFileName;
                    songViewModel.ChordsFileHandler.SaveAs(chordsFilefullPath);
                }
                catch (Exception ex)
                {
                    // in case of failure, rollback DB changes and log error message 
                    db.Songs.Remove(song);
                    await db.SaveChangesAsync();
                    HomeController.WriteErrorToLog(HttpContext, ex.Message);
                }
                finally
                {
                    // release open unmanaged resources 
                    songViewModel.ChordsFileHandler?.InputStream?.Dispose();
                    songViewModel.MidiFileHandler?.InputStream?.Dispose();
                    playbackFile?.Stream?.Dispose();
                }

                // If creation was successful, redirect to new song details page
                string successMessage = $"The Song '{song.Title}' by '{song.Artist}' was successfully uploaded.";
                return RedirectToAction(nameof(Details), new { Id = song.Id, message = successMessage });
            }
            return View(songViewModel);
        }
        #endregion

        #region GET: Songs/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            // assure id parameter is valid 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // fetch song from the database
            Song song = await db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }

            // check authorization 
            if (!IsUserAuthorized(song, AuthorizationActivity.Update))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // build a DTO view model of the song for the view to render 
            SongViewModel songViewModel = new SongViewModel(song);

            // add authorization data 
            songViewModel.IsUserAuthorizedToEdit = true;
            songViewModel.IsUserAuthorizedToDelete = IsUserAuthorized(song, AuthorizationActivity.Delete);
            songViewModel.IsAdminUser = User?.IsInRole(RoleName.Admin) ?? false;

            // try reading the chords & midi files and adding their content to the view model 
            string chordsFilePath = await GetSongPath(song.Id, SongFileType.ChordProgressionFile);
            string midiFilePath = await GetSongPath(song.Id, SongFileType.MidiOriginalFile);
            try
            {
                songViewModel.ChordProgression = System.IO.File.ReadAllText(chordsFilePath);
                songViewModel.MidiData = Composition.ReadMidiFile(midiFilePath);
            }
            catch (Exception ex)
            {
                // failed to read chord/midi data. log the error
                HomeController.WriteErrorToLog(HttpContext, ex.Message);
            }
            finally
            {   
                // release unmanaged open resources 
                songViewModel.MidiData?.Stream?.Dispose();
            }

            // pass the view model to the view to render 
            return View(songViewModel);
        }
        #endregion

        #region POST: Songs/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Song updatedSong)
        {
            // validate song id 
            if (updatedSong.Id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // fetch song from the database
            Song databaseSong = await db.Songs.FindAsync(updatedSong.Id);
            if (databaseSong == null)
            {
                return HttpNotFound();
            }

            // check authorization 
            if (!IsUserAuthorized(databaseSong, AuthorizationActivity.Update))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            #region Validations on Upload Files - Canceled

/*            // iniitialization for file validations 
            IList<IBar> bars = null;
            IMidiFile midiData = null;
            SongFileType songFileType;
            string errorMessage = null;
            MelodyTrackIndex? melodyIndex = updatedSong.MelodyTrackIndex;
            HttpPostedFileBase MidiFileHandler = null;
            HttpPostedFileBase midiFileHandler = null;
            HttpPostedFileBase chordFileHandler = MidiFileHandler; // song.ChordsFileHandler;

            // chords file validations
            if (chordFileHandler != null)
            {
                // init 
                songFileType = SongFileType.ChordProgressionFile;

                // file metadata 
                if (!FileUploadValidation.IsFileMetadataValid(chordFileHandler, songFileType, out errorMessage))
                    ModelState.AddModelError(nameof(SongViewModel.ChordsFileHandler), errorMessage);

                // file content 
                if (!FileUploadValidation.IsChordsFileValid(chordFileHandler, songFileType, out bars, out errorMessage))
                    ModelState.AddModelError(nameof(SongViewModel.ChordsFileHandler), errorMessage);
            }

            // midi file validations
            if (midiFileHandler != null)
            {
                // init 
                songFileType = SongFileType.MidiOriginalFile;

                // file metadata 
                if (!FileUploadValidation.IsFileMetadataValid(midiFileHandler, songFileType, out errorMessage))
                    ModelState.AddModelError(nameof(MidiFileHandler), errorMessage);

                // file content 
                if (!FileUploadValidation.IsMidiFileValid(midiFileHandler, out midiData, out errorMessage))
                    ModelState.AddModelError(nameof(MidiFileHandler), errorMessage);
            }

            // validate melody track index against midi file if one of them has changed 
            if ((midiFileHandler != null) || (melodyIndex != databaseSong.MelodyTrackIndex))
            {
                try
                {   // lazy load midi data if it is absent 
                    string originalMidiPath = await GetSongPath(updatedSong.Id, SongFileType.MidiOriginalFile);
                    midiData = Composition.ReadMidiFile(originalMidiPath);

                    // validate midi content 
                    if (!Composition.IsMelodyTrackIndexValid((int?)melodyIndex, midiData, out errorMessage))
                        ModelState.AddModelError(nameof(SongViewModel.MelodyTrackIndex), errorMessage);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(nameof(MidiFileHandler), errorMessage);
                }
            }

            // validate midi file against chord file if one of them has changed 
            if (chordFileHandler != null || midiFileHandler != null)
            {
                try
                {
                    // lazy load midi & chord data from persistance file server if necessary
                    if (bars == null)
                    {
                        string persistedChordsPath = await GetSongPath(databaseSong.Id, SongFileType.ChordProgressionFile);
                        bars = Composition.ReadChordsFromFile(persistedChordsPath);
                    }

                    if (midiData == null)
                    {
                        string persistedMidiPath = await GetSongPath(databaseSong.Id, SongFileType.MidiOriginalFile);
                        midiData = Composition.ReadMidiFile(persistedMidiPath);
                    }

                    if (!Composition.AreBarsCompatible(bars, midiData, out errorMessage))
                        ModelState.AddModelError(nameof(MidiFileHandler), errorMessage);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(MidiFileHandler), ex.Message);
                }
            }*/

            #endregion

            if (ModelState.IsValid)
            {
                // map updated data 
                databaseSong.Artist = updatedSong.Artist;
                databaseSong.Title = updatedSong.Title;
                databaseSong.IsPublic = updatedSong.IsPublic;

                // update modified timestamp
                databaseSong.Modified = DateTime.Now;

                #region Update Files on FileServer - Canceled
                /*
                // update files on disk and file names on database record 
                string directoryPath = await GetSongPath(databaseSong.Id);
                string archivePath = directoryPath + @"Archive\";

                try
                {
                    if (midiFileHandler != null)
                    {
                        Directory.CreateDirectory(archivePath);
                        string originalMidiPath = await GetSongPath(databaseSong.Id, SongFileType.MidiOriginalFile);
                        string originalPlaybackPath = await GetSongPath(databaseSong.Id, SongFileType.MidiPlaybackFile);
                        System.IO.File.Move(originalMidiPath, archivePath);
                        System.IO.File.Move(originalPlaybackPath, archivePath);
                        string newMidiFileName = midiFileHandler.FileName;
                        string newPath = directoryPath + newMidiFileName;
                        databaseSong.MidiFileName = newMidiFileName;
                        midiFileHandler.SaveAs(newPath);

                        databaseSong.SetPlaybackName(newMidiFileName);
                        string playbackNewPath = directoryPath + databaseSong.MidiPlaybackFileName;
                        IMidiFile playbackFile = Composition.CreateMidiPlayback(midiFileHandler.InputStream, melodyIndex);
                        playbackFile.SaveFile(outputPath: playbackNewPath, pathIncludesFileName: true);
                    }

                    if (chordFileHandler != null)
                    {
                        Directory.CreateDirectory(archivePath);
                        string originalChordsPath = await GetSongPath(databaseSong.Id, SongFileType.ChordProgressionFile);
                        System.IO.File.Move(originalChordsPath, archivePath);
                        string newChordsFileName = chordFileHandler.FileName;
                        string newChordsPath = directoryPath + newChordsFileName;
                        databaseSong.ChordsFileName = newChordsFileName;
                        chordFileHandler.SaveAs(newChordsPath);
                    }


                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

                finally
                {
                    // dispose open resources 
                    chordFileHandler?.InputStream?.Dispose();
                    midiFileHandler?.InputStream?.Dispose();
                }
                */
                #endregion

                // commit change to database record
                db.Entry(databaseSong).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // If edit was successful, redirect to the updated song details page
                string successMessage = $"The song was successfully updated!";
                return RedirectToAction(nameof(Details), new { Id = databaseSong.Id, message = successMessage });
            }

            // model is not valid, render back the view
            return View(databaseSong);
        }
        #endregion

        #region // GET: Songs/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            // validate song id 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // fetch song from the database
            Song song = await db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }

            // check authorization 
            if (!IsUserAuthorized(song, AuthorizationActivity.Delete))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // build a DTO view model of the song for the view to render 
            SongViewModel songViewModel = new SongViewModel(song);

            // add edit authorization data 
            songViewModel.IsUserAuthorizedToDelete = true;
            songViewModel.IsUserAuthorizedToEdit = IsUserAuthorized(song, AuthorizationActivity.Update);

            // try reading the chords & midi files and adding their content to the view model 
            string chordsFilePath = await GetSongPath(song.Id, SongFileType.ChordProgressionFile);
            string midiFilePath = await GetSongPath(song.Id, SongFileType.MidiOriginalFile);
            try
            {
                songViewModel.ChordProgression = System.IO.File.ReadAllText(chordsFilePath);
                songViewModel.MidiData = Composition.ReadMidiFile(midiFilePath);
            }
            catch (Exception ex)
            {
                // failed to read chord/midi data. log the error
                HomeController.WriteErrorToLog(HttpContext, ex.Message);
            }
            finally
            {
                // release unmanaged open resources 
                songViewModel.MidiData?.Stream?.Dispose();
            }

            // pass the view model to the view to render 
            return View(songViewModel);
        }
        #endregion

        #region // POST: Songs/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            // fetch song from database 
            Song song = await db.Songs.FindAsync(id);

            // check authorization 
            if (!IsUserAuthorized(song, AuthorizationActivity.Delete))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // try removing the pfysical files from the file server 
            string songDirectoryPath = await GetSongPath(song.Id);
            try
            {
                Directory.Delete(songDirectoryPath, true);
            }
            catch (Exception ex)
            {
                // log error message on server 
                string errorMessage = "An error occoured in attempt to remove song files: " + ex.Message;
                HomeController.WriteErrorToLog(HttpContext, errorMessage);

                // report error to user on original view 
                ModelState.AddModelError(string.Empty, errorMessage);
                return RedirectToAction(nameof(Delete), new { Id = id });
            }

            // remove record from database
            db.Songs.Remove(song);
            await db.SaveChangesAsync();

            // If delete succeeded, redirect song index with appropriate message 
            string successMessage = $"The song '{song.Title}' by '{song.Artist}' was successfully deleted.";
            return RedirectToAction(nameof(Index), new { Message = successMessage });
        }
        #endregion

        #region DownloadFile
        /// <summary>
        /// Downloads the requested song file from the server to the client. 
        /// </summary>
        /// <param name="id"> The song identification number. </param>
        /// <param name="songFileType"> File type - chords file, original midi file or playback file. </param>
        /// <returns> The requested file for the given song. </returns>
        public async Task<ActionResult> DownloadFile(int? id, SongFileType songFileType)
        {
            // assure valid input params 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // fetch song from database 
            Song song = await db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }

            // check authorization for the retrieved song 
            if (!IsUserAuthorized(song, AuthorizationActivity.Display))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // build path for the requested file resource on the file server 
            string filePath = await GetSongPath(song.Id, songFileType);

            // read file contents 
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            // return file content to the client 
            string fileName = Path.GetFileName(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion

        #region GetSongPath
        /// <summary>
        /// Gets the path of the given song's resources on the file server. 
        /// </summary>
        /// <param name="songId"> The id of the requested song. </param>
        /// <param name="songFileType"> Optional parameter for specifing a specific 
        /// file resource of the song such as midi or chords file. If no specific file 
        /// is requested, set this parameter to null and the path to the song's directory 
        /// would be returned. </param>
        /// <returns> Full physcial path on the file server of the given song resources. </returns>
        private async Task<string> GetSongPath(int songId, SongFileType? songFileType = null)
        {
            // init 
            Song song;
            string path, fileName;

            // fetch song from database 
            song = await db.Songs.FindAsync(songId);

            // check authorization for the retrieved song 
            if (song == null || !IsUserAuthorized(song, AuthorizationActivity.Display))
                return null;

            // get the song's directory path on the file server 
            path = HomeController.GetFileServerPath() + $@"Songs\{songId}\";

            // if a specific file is requested, get it's inner path  
            if (songFileType.HasValue)
            {
                switch (songFileType)
                {
                    case SongFileType.ChordProgressionFile:
                        fileName = song.ChordsFileName; break;
                    case SongFileType.MidiOriginalFile:
                        fileName = song.MidiFileName; break;
                    case SongFileType.MidiPlaybackFile:
                        fileName = song.MidiPlaybackFileName; break;
                    default:
                        fileName = string.Empty; break;
                }

                // add the inner path of the requested resource to the directory path 
                path += fileName;
            }

            // return the requested path 
            return path;
        }
        #endregion

        #region Authorization Checks
        /// <summary>
        /// <para> Checks if a user is authorized for the given song and activity. </para>
        /// This security check is intended to be used only to extend the default built-in 
        /// checks of the .NET framework. For exmaple, if the activity of creating a new song
        /// is permitted to all logged-in users, then the built-in authorization check via 
        /// the <see cref="AuthorizeAttribute"/> gets the job done without the need to use 
        /// any additional checks. This method is relevant for cutomizing and fine-tunning 
        /// the standard default checks, such as comparing the user id of the requested 
        /// resource records and the current logged-in user id.
        /// </summary>
        /// <param name="song"> The requested song the user is trying to access. </param>
        /// <param name="authActivity"> The requested activity on the song (update/delete, etc.).</param>
        /// <param name="user"> The user to check against. </param>
        /// <returns> True if user is authorized, false otherwise. </returns>
        public static bool IsUserAuthorized(Song song, AuthorizationActivity authActivity, IPrincipal user)
        {
            // assure there is a concrete song to check 
            if (song == null)
                return false;

            // if current user is logged-in fetch it's id
            string userId = user?.Identity?.GetUserId();

            // check authorization for the requested activity 
            switch (authActivity)
            {
                // Create - any logged in user can upload new songs for himself 
                case AuthorizationActivity.Create:
                    return user?.Identity?.IsAuthenticated ?? false;

                // Display - either song is public or user is song owner or admin
                case AuthorizationActivity.Display:
                    return song.IsPublic || song.UserId.Equals(userId) || user.IsInRole(RoleName.Admin);

                // Update & Delete - only admins and song owners
                case AuthorizationActivity.Update:
                case AuthorizationActivity.Cancel:
                case AuthorizationActivity.Delete:
                    return user.IsInRole(RoleName.Admin) || song.UserId.Equals(userId);
            }

            // if we got here we missed some test, default the security check to deny the access 
            return false;
        }


        /// <inheritdoc cref="IsUserAuthorized(Song, AuthorizationActivity, IPrincipal)"/>
        public bool IsUserAuthorized(Song song, AuthorizationActivity authActivity)
        {
            // delegate authorization check to the static method with current user 
            return IsUserAuthorized(song, authActivity, User);
        }
        #endregion

        #region Dispose Resources 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}