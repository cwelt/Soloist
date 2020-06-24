using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.ViewModels
{
    public class CompositionParamsViewModel
    {
        [Required]
        [DisplayName("Overall Note Duration Feel")]
        public OverallNoteDurationFeel OverallNoteDurationFeel { get; set; }

        [Required]
        [DisplayName("Musical Instrument")]
        public MusicalInstrument MusicalInstrument { get; set; }

        [DisplayName("Pitch Lowest Bound")]
        public NotePitch MinPitch { get; set; }

        [DisplayName("Pitch Upper Bound")]
        public NotePitch MaxPitch { get; set; }

        [DisplayName("Composition Algorithm Strategy")]
        public CompositionStrategy CompositionStrategy { get; set; }

        [DisplayName("Use Existing Melody as Seed")]
        public bool useExistingMelodyAsSeed { get; set; }

        [DisplayName("Song")]
        public int SongId { get; set; }

        public List<Song> Songs { get; set; } = new List<Song>
        {
                 new Song
                {
                    Id = 2,
                    Description = "Shmulik Kraus - After 20 Years",
                    ChordPath = "/SampleData/twenty_years_chords.txt",
                    MidiPath = "/SampleData/after_20_years.mid"
                }
          /*  ,new Song
                {
                    Id = 1,
                    Description = "Beatles - Here There and Everywhere",
                    ChordPath = "/SampleData/here_there_chords.txt",
                    MidiPath = "/SampleData/beatles_here_there.mid"
                },

                new Song
                {
                    Id = 3,
                    Description = "Arik Einstein - Yoshev Al Ha-Gader",
                    ChordPath = "/SampleData/Klepter-Gader.txt",
                    MidiPath = "/SampleData/YoshevGader.mid"
                },
                                new Song
                {
                    Id = 4,
                    Description = "Gloria Gaynor - I Will Survive",
                    ChordPath = "/SampleData/survive.txt",
                    MidiPath = "/SampleData/Survive.mid"
                }*/
        };

        public SelectList SongSelectList { get; set; }

        public CompositionParamsViewModel()
        {
            SongSelectList = new SelectList(Songs.OrderBy(s => s.Description), nameof(Song.Id), "Description", SongId);
        }


    }
}