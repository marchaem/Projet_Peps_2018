using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class InformationsController : Controller
    {

        static Informations infos;

        // GET: Informations
        public ActionResult Index()
        {
            infos = new Informations();
            return View();
        }

        //
        // POST: /Informations/ 
        [HttpPost]
        public String Index(Informations info)
        {
            DateTime dateDebut = new DateTime(2014,12,18);
            double diff= (info.DateCourante - dateDebut).TotalDays / 365;
            infos.t = diff;
            if (ModelState.IsValid)
                return "la date est " + infos.t.ToString();
            // return RedirectToAction("Index", "Accueil");


            // If we got this far, something failed, redisplay form
            return "hello";
           // return View();
        }
    }
}