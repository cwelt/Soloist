using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CW.Soloist.WebApplication.ViewModels
{
    public class SongsViewModel
    {
        public IEnumerable<SongsViewModel> Songs { get; set; }
    }
}