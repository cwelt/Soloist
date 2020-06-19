using CW.Soloist.CompositionService.Midi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SoloistWebClient.Controllers
{
    public class ComposerController : Controller
    {
        // GET: Composer
        public ActionResult Index()
        {
            IDictionary instruments = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(MusicalInstrument)))
            {
                instruments.Add(item, item.ToString());
            }

            var musicalInstruments = new SelectList(instruments);
            ViewBag.Instruments = musicalInstruments;


            return View();
        }
    }
}