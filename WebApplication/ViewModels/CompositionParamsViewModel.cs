﻿using CW.Soloist.CompositionService.Compositors;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;
using CW.Soloist.DataAccess.DomainModels;
using System;
using System.Collections;
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

        public List<Song> Songs { get; set; } 

        public SelectList SongSelectList { get; set; }
    }
}