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

namespace CW.Soloist.WebApplication.Controllers
{
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string fileServerPath = HomeController.GetFileServerPath();

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
                songsViewModel.Add(new SongViewModel
                {
                    Id = song.Id,
                    Title = song.Title,
                    Artist = song.Artist,
                    IsUserAuthorizedToEdit = IsUserAuthorized(song, AuthorizationActivity.Update),
                    IsUserAuthorizedToDelete = IsUserAuthorized(song, AuthorizationActivity.Delete),
                });
            }

            // pass the song list for the view for rendering 
            return View(songsViewModel);
        }

        // GET: Songs/Details/5
        public async Task<ActionResult> Details(int? id)
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
            if (!IsUserAuthorized(song, AuthorizationActivity.Display))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                //return new HttpUnauthorizedResult("You are not authorized for this song");
            }

            return View(song);
        }

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
                /* TODO: 
                 * validate the uploaded file's length and content:
                 * check that chord file number of bars = midi num of bars 
                 * check that file have the expected media\MIME type 
                 * check that files are not empty of course. 
                 * check that files are not too large. 
                 * */

                // Create a new song instance 
                Song song = new Song
                {
                    Title = songViewModel.Title,
                    Artist = songViewModel.Artist,
                    MelodyTrackIndex = songViewModel.MelodyTrackIndex,  
                    MidiFileName = songViewModel.MidiFile.FileName,
                    ChordsFileName = songViewModel.ChordsFile.FileName,
                };

                // save the new song in the database 
                db.Songs.Add(song);
                await db.SaveChangesAsync();

                // create on the file server a new directory for the new song 
                string directoryPath = fileServerPath + $@"Songs\{song.Id}\";
                Directory.CreateDirectory(directoryPath);

                // save the midi file in the new directory 
                string midiFileFullPath = directoryPath + song.MidiFileName;
                songViewModel.MidiFile.SaveAs(midiFileFullPath);

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

                // Display - either song is public or user is song owner
                case AuthorizationActivity.Display:
                    return song.IsPublic || song.UserId.Equals(userId);

                // Update & Delete - only admins and song owners
                case AuthorizationActivity.Update:
                case AuthorizationActivity.Cancel:
                case AuthorizationActivity.Delete:
                    return user.IsInRole(RoleName.Admin) || song.UserId.Equals(userId);
            }

            // if we got here we missed some test, default the security check to deny access 
            return false;
        }


        /// <inheritdoc cref="IsUserAuthorized(Song, AuthorizationActivity, IPrincipal)"/>
        public bool IsUserAuthorized(Song song, AuthorizationActivity authActivity)
        {
            // delegate authorization check to the static method with current user 
            return IsUserAuthorized(song, authActivity, User);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
