using CW.Soloist.CompositionService.Midi;
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
        public string Description { get; set; }

        [Required]
        [DisplayName("MIDI File")]
        public HttpPostedFileBase MidiFile { get; set; }

        [DisplayName("Melody Track Number In MIDI File")]
        public MelodyTrackIndex? MelodyTrackIndex { get; set; }

        [Required]
        [DisplayName("Chords File")]
        public HttpPostedFileBase ChordProgressionFile { get; set; }

        public String ChordProgression { get; set; }
    }
}