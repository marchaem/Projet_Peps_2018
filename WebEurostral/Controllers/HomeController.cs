using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEurostral.Models;

namespace WebEurostral.Controllers
{
    public class HomeController : Controller
    {
        static EurostralModels eurost;
        // GET: Parametres
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Parametres/ 
        [HttpPost]
        public ActionResult Index(EurostralModels eurostP)//(Test tests)
        {
            
            DateTime dateDebut = new DateTime(2014, 12, 18);
            double diff = (eurostP.p.DateCourante - dateDebut).TotalDays / 365;
            eurostP.t = diff;
            //if (ModelState.IsValid
            //ViewData["Delta"] = tests.displayDelta0(tests.DateCourante);
            //return "le t est de " + (eurost.t).ToString(); 
            //return RedirectToAction("Index");
            

            // If we got this far, something failed, redisplay form
            //return "hello";
             return RedirectToAction("PrixEnt");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult PrixEnt()
        {
            //ViewBag.Message = "Your contact page.";
            int nbActions = 3;
            double[,] past = new double[,nbActions];
            eurost.Pricet(eurost.t, past); // Il faut initialiser t dans Eurostral !!!!!

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}