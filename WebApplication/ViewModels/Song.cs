using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CW.Soloist.WebApplication.ViewModels
{
    public class Song
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string ChordPath { get; set; }
        public string MidiPath { get; set; }


    }
}