using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEurostral.Models;
using Data;
using Wrapper;

namespace WebEurostral.Controllers
{
    public class HomeController : Controller
    {
        static EurostralModels eurost;
        // GET: Parametres
        public ActionResult Index()
        {
            //ParametresModel param = new ParametresModel();
            EurostralModels eurost = new EurostralModels();
            return View(eurost);//param);
        }

        // POST: /Parametres/ 
        [HttpPost]
        public ActionResult Index(EurostralModels eurost2)//(Test tests)
        {
            eurost = eurost2;
            DateTime dateDebut = new DateTime(2014, 12, 18);
            //double diff = (tests.DateCourante - dateDebut).TotalDays / 365;
            double diff = (eurost.DateCourante-dateDebut).TotalDays / 365.0;
            eurost.t = diff;
            //if (ModelState.IsValid
            //ViewData["Delta"] = tests.displayDelta0(tests.DateCourante);
            //return "le t est de " + (eurost.t).ToString(); 
            return RedirectToAction("DeltaEnt");
            

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
           /* int nbActions = 3;

            double[,] past = new double[2,nbActions];

            DateTime date = new DateTime(2014, 12, 18);
            DateTime debutProduit = new DateTime(2014, 12, 18);
            DateTime finProduit = new DateTime(2022, 12, 18);
            Data.RecupData recup = new RecupData(new DateTime(2000, 1, 1), date);

           
            recup.Fetch();
            List<string> symbols = recup.getSymbols();
            Dictionary<DateTime,double> dico = recup.getData(symbols[0]);


            eurost.covLogR = new double[5,5];
            eurost.pastDelta= new double[5, 5];
            eurost.pastPrice = new double[5, 5];
            eurost.covLogR = recup.exportCov();
            eurost.pastDelta = recup.exportPast(eurost.t, 182, debutProduit, finProduit);
            eurost.pastPrice = recup.exportPast(eurost.t, 182, debutProduit, finProduit);*/

            double r_eu = 0.002;
            double r_aus = 0.025;
            double r_us = 0.00025;
            int size = 5;
            double r = r_eu;



            double[] spots = new double[5];
            for (int i = 0; i < 5; i++)
            {
                spots[i] = eurost.pastPrice[0, i];
            }



            double[] trends = new double[5];
            trends[0] = r_eu;
            trends[1] = r_us - eurost.covLogR[1, 3];
            trends[2] = r_aus - eurost.covLogR[2, 4];
            trends[3] = r_eu - r_us;
            trends[4] = r_eu - r_aus;

            double[] lambdas = new double[5];
            for (int i = 0; i < 5; i++)
            {
                lambdas[i] = 0.05;
            }

            WrapperClass wr1 = new WrapperClass(size, r, eurost.covLogR, spots, trends, 0.1, 50000, 100, 8.0, 15, lambdas);
            //WrapperClass wr1 = new WrapperClass();
            eurost.prixEnt=wr1.getPriceEurostral(eurost.t, eurost.pastPrice); // Il faut initialiser t dans Eurostral !!!!!
            //wr1.getPriceEurostral(eurost.t, pastPrice);
           // return (eurost.prixEnt);

            ViewData["price"] = eurost.prixEnt;
            return View(eurost);
            //return RedirectToAction("DeltaEnt");
        }

        public ActionResult DeltaEnt()
        {
            //ViewBag.Message = "Your contact page.";
            int nbActions = 3;

            //double[,] past = new double[2,nbActions];

            DateTime date = DateTime.Today;
            DateTime debutProduit = new DateTime(2014, 12, 18);
            DateTime finProduit = new DateTime(2022, 12, 08);
            Data.RecupData recup = new RecupData(new DateTime(2000, 1, 1), date);

           
            recup.Fetch();
            List<string> symbols = recup.getSymbols();
            Dictionary<DateTime,double> dico = recup.getData(symbols[0]);



            eurost.covLogR = recup.exportCov();
            eurost.pastDelta = recup.exportPast(eurost.t, 182, debutProduit, finProduit);
            eurost.pastPrice = recup.exportPast(eurost.t, 182, debutProduit, finProduit);

            double r_eu = 0.002;
            double r_aus = 0.025;
            double r_us = 0.00025;
            int size = 5;
            double r = r_eu;
            

            double[] spots = new double[5];
            for (int i = 0; i < 5; i++)
            {
                spots[i] = eurost.pastPrice[0, i];
            }



            double[] trends = new double[5];
            trends[0] = r_eu;
            trends[1] = r_us - eurost.covLogR[1, 3];
            trends[2] = r_aus - eurost.covLogR[2, 4];
            trends[3] = r_eu - r_us;
            trends[4] = r_eu - r_aus;

            double[] lambdas = new double[5];
            for (int i = 0; i < 5; i++)
            {
                lambdas[i] = 0.05;
            }

            WrapperClass wr1 = new WrapperClass(size, r, eurost.covLogR, spots, trends, 0.1, 50000, 100, 8.0, 15, lambdas);
            //WrapperClass wr1 = new WrapperClass();
            double H= 416;
            eurost.deltaEnt = wr1.getDeltaEurostral(eurost.pastPrice, eurost.t, H);
            //wr1.getPriceEurostral(eurost.t, pastPrice);
            // return (eurost.prixEnt);

           // eurost.pastPrice = recup.exportPast(eurost.t, 182, debutProduit, finProduit);

            //Ajout
            for (int i = 0; i < 5; i++)
            {
                eurost.prixActifs[i] = eurost.pastPrice[(eurost.pastPrice.GetLength(0)) - 1, i];
            }

            // ViewData["price"] = eurost.prixEnt;
            //ViewBag.data = eurost.deltaEnt;
            //return RedirectToAction("PrixEnt");
            return View(eurost);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}