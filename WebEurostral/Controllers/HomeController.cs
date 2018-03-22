using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEurostral.Models;
using Data;

namespace WebEurostral.Controllers
{
    public class HomeController : Controller
    {
        static EurostralModels eurost;
        // GET: Parametres
        public ActionResult Index()
        {
            //ParametresModel param = new ParametresModel();
            return View();//param);
        }

        // POST: /Parametres/ 
        [HttpPost]
        public String Index(EurostralModels eurost)//(Test tests)
        {
            //eurost.p = param2;
            DateTime dateDebut = new DateTime(2014, 12, 18);
            //double diff = (tests.DateCourante - dateDebut).TotalDays / 365;
            double diff = (eurost.DateCourante-dateDebut).TotalDays / 365.0;
            eurost.t = diff;
            //if (ModelState.IsValid
            //ViewData["Delta"] = tests.displayDelta0(tests.DateCourante);
            return "le t est de " + (eurost.t).ToString(); 
            //return RedirectToAction("Index");
            

            // If we got this far, something failed, redisplay form
            //return "hello";
            // return RedirectToAction("PrixEnt");
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

            double[,] past = new double[2,nbActions];

            DateTime date = new DateTime(2014, 12, 18);
            DateTime debutProduit = new DateTime(2014, 12, 18);
            DateTime finProduit = new DateTime(2022, 12, 08);
            Data.RecupData recup = new RecupData(new DateTime(2000, 1, 1), date);

           
            recup.Fetch();
            List<string> symbols = recup.getSymbols();
            Dictionary<DateTime,double> dico = recup.getData(symbols[0]);



            double[,] covLogR = recup.exportCov();
            double[,] pastDelta = recup.exportPast(t, 182, debutProduit, finProduit);
            double[,] pastPrice = recup.exportPast(t, 182, debutProduit, finProduit);

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