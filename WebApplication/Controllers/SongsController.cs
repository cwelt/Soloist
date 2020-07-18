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

namespace CW.Soloist.WebApplication.Controllers
{
    public class SongsController : Controller
    {
        private SoloistContext db = new SoloistContext();
        private string fileServerPath = HomeController.GetFileServerPath();

        // GET: Songs
        public async Task<ActionResult> Index()
        {
            return View(await db.Songs.ToListAsync());
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
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Songs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    MidiFileName = songViewModel.MidiFile.FileName,
                    ChordsFileName = songViewModel.ChordProgressionFile.FileName,
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
                songViewModel.ChordProgressionFile.SaveAs(chordsFilefullPath);

                // TODO: if saving on file server failed, rollback DB changes 

                // TODO: Show message on index view...
                ViewBag.Message = "Saved Successfully"; 

                return RedirectToAction("Index");
            }

            return View(songViewModel);
        }

        // GET: Songs/Edit/5
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
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Artist,MidiFileName,ChordsFileName")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(song);
        }

        // GET: Songs/Delete/5
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
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Song song = await db.Songs.FindAsync(id);
            db.Songs.Remove(song);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
