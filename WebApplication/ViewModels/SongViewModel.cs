using CW.Soloist.CompositionService.Midi;
using CW.Soloist.DataAccess.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CW.Soloist.WebApplication.ViewModels
{
    public class SongViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(30)]
        [DisplayName("Song Name")]
        public string Title { get; set; }

        [Required]
        [StringLength(70)]
        public string Artist { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Original MIDI File")]
        public HttpPostedFileBase MidiFile { get; set; }

        [DisplayName("Melody Track Number In MIDI File")]
        public MelodyTrackIndex? MelodyTrackIndex { get; set; }

        [Required]
        [DisplayName("Chords File")]
        public HttpPostedFileBase ChordsFile { get; set; }

        [DisplayName("Chord Progression")]
        public String ChordProgression { get; set; }


        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public bool IsUserAuthorizedToEdit { get; set; }
        public bool IsUserAuthorizedToDelete { get; set; }

        public SongViewModel() { }

        public SongViewModel(Song song)
        {
            Id = song.Id;
            Artist = song.Artist;
            Title = song.Title;
            MelodyTrackIndex = song.MelodyTrackIndex;
            Created = song.Created;
            Modified = song.Modified;
        }
    }
}