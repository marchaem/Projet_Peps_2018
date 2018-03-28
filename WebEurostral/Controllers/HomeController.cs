﻿using System;
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

          /*  double r_eu = 0.002;
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

            WrapperClass wr1 = new WrapperClass(size, r, eurost.covLogR, spots, trends, 0.1, eurost.nb_iterations, 100, 8.0, 15, lambdas);*/
           
            eurost.prixEnt=eurost.wc.getPriceEurostral(eurost.t, eurost.pastPrice); // Il faut initialiser t dans Eurostral !!!!!
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



            eurost.covLogR = recup.exportCov(new DateTime(2014, 7, 1),debutProduit);
            eurost.pastDelta = recup.exportPast(eurost.t, 7, debutProduit, finProduit);
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

            WrapperClass wr1 = new WrapperClass(size, r, eurost.covLogR, spots, trends, 0.1, eurost.nb_iterations, 100, 8.0, 16, lambdas);
            eurost.wc = wr1;
            //WrapperClass wr1 = new WrapperClass();
            double H= 416;
            eurost.deltaEnt = wr1.getDeltaEurostral(eurost.pastPrice, eurost.t, H);
            //wr1.getPriceEurostral(eurost.t, pastPrice);
            // return (eurost.prixEnt);

            /*DateTime debutBackTest = new DateTime(2010, 03, 22);
            DateTime finBacktest = new DateTime(2018, 03, 22);
            double t0 = recup.DateToDouble(debutBackTest, finBacktest, finBacktest);
            double[,] donneesHistoriques = recup.exportPast(t0, 7, debutBackTest, finBacktest);
            eurost.PL=wr1.getPLEurostral(donneesHistoriques, H);*/

            // eurost.pastPrice = recup.exportPast(eurost.t, 182, debutProduit, finProduit);
            double [] track = new double[eurost.pastDelta.GetLength(0)-1];
            double[] pp = new double[eurost.pastDelta.GetLength(0)];
            double[] pock = new double[eurost.pastDelta.GetLength(0)];
            wr1.trackingError(eurost.pastDelta, eurost.t, H, pp, pock, track, eurost.pastDelta.GetLength(0));
            eurost.PandL = track;
            eurost.pock = pock;
            eurost.pp = pp;
            //Ajout
            for (int i = 0; i < 5; i++)
            {
                eurost.prixActifs[i] = eurost.pastPrice[(eurost.pastPrice.GetLength(0)) - 1, i];
            }

            eurost.prixEnt=eurost.wc.getPriceEurostral(0, eurost.pastPrice); // Il faut initialiser t dans Eurostral !!!!
            ViewData["price"] = eurost.prixEnt;

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

        public ActionResult AffPL()
        {
            // double[] tab1 = new double[3];

            /*
            eurost.PandL[0] = 1.2;
            eurost.PandL[1] = 3;
            eurost.PandL[2] = 2.5;*/
            eurost.datesbis = new int[eurost.pastDelta.GetLength(0)];
            for (int i = 0; i < eurost.pastDelta.GetLength(0)-1; i++)
                eurost.datesbis[i] = i;
            DateTime date1 = new DateTime(2014, 12, 18);
            DateTime date2 = new DateTime(2015, 12, 08);

           // DateTime date3 = new DateTime(2015, 12, 08);
            eurost.dates[0] = date1;
            eurost.dates[1] = date2;
           // eurost.dates[2] = date3;
            // ViewData("PandL") = eurost.PandL;

            return View(eurost);
        }
    }
}