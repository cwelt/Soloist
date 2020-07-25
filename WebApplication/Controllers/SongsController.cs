using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;
using SoloistWebClient.Controllers;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using CW.Soloist.WebApplication.ViewModels;
using System.IO;
using System;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.WebApplication.Models;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using CW.Soloist.CompositionService;

namespace CW.Soloist.WebApplication.Controllers
{
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string fileServerPath = HomeController.GetFileServerPath();

        #region Index
        // GET: Songs
        public async Task<ActionResult> Index()
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

            // pass the song list for the view for rendering 
            return View(songsViewModel);
        }
        #endregion

        #region Details
        // GET: Songs/Details/5
        public async Task<ActionResult> Details(int? id)
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

            // try reading the chord progression and adding it's content to the view model 
            string chordsFilePath = await GetSongPath(song.Id, SongFileType.ChordProgressionFile);
            try
            {
                songViewModel.ChordProgression = System.IO.File.ReadAllText(chordsFilePath);
            }
            catch (Exception)
            {
                throw;
            }

            // pass the view model to the view to render 
            return View(songViewModel);
        }
        #endregion

        // GET: Songs/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Songs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(SongViewModel songViewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime timestamp = DateTime.Now;

                // Create a new song instance 
                Song song = new Song
                {
                    Created = timestamp,
                    Modified = timestamp,
                    Title = songViewModel.Title,
                    Artist = songViewModel.Artist,
                    MidiFileName = songViewModel.MidiFile.FileName,
                    ChordsFileName = songViewModel.ChordsFile.FileName,
                    MelodyTrackIndex = songViewModel.MelodyTrackIndex,
                    UserId = this.User.Identity.GetUserId(),
                    
                };

                // set name for a playback file 
                song.SetPlaybackName();

                // save the new song in the database 
                db.Songs.Add(song);
                await db.SaveChangesAsync();

                // create on the file server a new directory for the new song 
                string directoryPath = fileServerPath + $@"Songs\{song.Id}\";
                Directory.CreateDirectory(directoryPath);

                // save the midi file in the new directory 
                string midiFileFullPath = directoryPath + song.MidiFileName;
                songViewModel.MidiFile.SaveAs(midiFileFullPath);

                // save the midi playback file in the new directory 
                string midiPlaybackFullPath = directoryPath + song.MidiPlaybackFileName;
                IMidiFile playbackFile = Composition.CreateMidiPlayback(songViewModel.MidiFile.InputStream, song.MelodyTrackIndex);
                playbackFile.SaveFile(outputPath: midiPlaybackFullPath, pathIncludesFileName: true);

                // save the chord progression file in the new directory 
                string chordsFilefullPath = directoryPath + song.ChordsFileName;
                songViewModel.ChordsFile.SaveAs(chordsFilefullPath);

                // TODO: if saving on file server failed, rollback DB changes 

                // TODO: Show message on index view...
                ViewBag.Message = "Saved Successfully";

                return RedirectToAction("Index");
            }

            return View(songViewModel);
        }

        // GET: Songs/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

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

            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Artist,MidiFileName,ChordsFileName")] Song song)
        {
            // check authorization 
            if (!IsUserAuthorized(song, AuthorizationActivity.Update))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(song);
        }

        // GET: Songs/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

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

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Song song = await db.Songs.FindAsync(id);

            // check authorization 
            if (!IsUserAuthorized(song, AuthorizationActivity.Delete))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            db.Songs.Remove(song);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
