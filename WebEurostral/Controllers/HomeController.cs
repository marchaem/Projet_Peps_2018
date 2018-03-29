using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEurostral.Models;
using Data;
using Wrapper;
using static WebEurostral.Models.EurostralModels;
using Newtonsoft.Json;
using System.Data;
using System.Runtime.InteropServices;

namespace WebEurostral.Controllers
{
    public class HomeController : Controller
    {
        static EurostralModels eurost;
        static int ind = 0;
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
        
            double diff = (eurost.DateCourante-dateDebut).TotalDays / 365.0;
            eurost.t = diff;
            //if (ModelState.IsValid
            //ViewData["Delta"] = tests.displayDelta0(tests.DateCourante);
            //return "le t est de " + (eurost.t).ToString(); 
            return RedirectToAction("DeltaEnt");
            

          
            // return RedirectToAction("PrixEnt");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult PrixEnt()
        {
           
           
            eurost.prixEnt=eurost.wc.getPriceEurostral(eurost.t, eurost.pastPrice); // Il faut initialiser t dans Eurostral !!!!!

            
            return View(eurost);
            //return RedirectToAction("DeltaEnt");
        }

        public ActionResult DeltaEnt()
        {
            if (ind != 0)
            {
                return View(eurost);
            }
            else
            {
                ind++;
                //ViewBag.Message = "Your contact page.";
                DateTime date = DateTime.Today;
                DateTime debutProduit = new DateTime(2014, 12, 18);
                DateTime finProduit = new DateTime(2022, 12, 08);
                RecupData recup = new RecupData(new DateTime(2005, 12, 18), date);
                List<String> Symbols = new List<String>();
                Symbols.Add("^STOXX50E");
                Symbols.Add("^GSPC");
                Symbols.Add("^AXJO");
                eurost.dataPoints_GSPC = new List<DataPoint>();
                eurost.dataPoints_STOXX50E = new List<DataPoint>();
                eurost.dataPoints_AXJO = new List<DataPoint>();
                Symbols.Add("USDEUR=X");
                Symbols.Add("AUDEUR=X");
                eurost.dataPoints_EURUSD = new List<DataPoint>();
                eurost.dataPoints_EURAUD = new List<DataPoint>();
                eurost.dataPoints_trackingError = new List<DataPoint>();
                eurost.dataPoints_PfC = new List<DataPoint>();
                eurost.dataPoints_prix = new List<DataPoint>();
                DateTime debut = new DateTime(2014, 12, 18);
                DateTime fin = DateTime.Today;
                recup.Fetch();
                List<double> values = recup.getData("^GSPC").Values.ToList();
                List<DateTime> dates = recup.getData("^GSPC").Keys.ToList();
                List<double> values2 = recup.getData("^STOXX50E").Values.ToList();
                List<DateTime> dates2 = recup.getData("^STOXX50E").Keys.ToList();
                List<double> values3 = recup.getData("^AXJO").Values.ToList();
                List<DateTime> dates3 = recup.getData("^AXJO").Keys.ToList();
                List<double> values4 = recup.getData("USDEUR=X").Values.ToList();
                List<DateTime> dates4 = recup.getData("USDEUR=X").Keys.ToList();
                List<double> values5 = recup.getData("AUDEUR=X").Values.ToList();
                List<DateTime> dates5 = recup.getData("AUDEUR=X").Keys.ToList();
                for (int i = 0; i < values.Count; i++)
                {
                    eurost.dataPoints_GSPC.Add(new DataPoint(recup.DateToDouble(debutProduit,dates[i],finProduit)+2014, values[i]));
                }
                for (int i = 0; i < values2.Count; i++)
                {
                    eurost.dataPoints_STOXX50E.Add(new DataPoint(recup.DateToDouble(debutProduit, dates2[i], finProduit)+2014,values2[i]));
                }
                for (int i = 0; i < values3.Count; i++)
                {
                    eurost.dataPoints_AXJO.Add(new DataPoint(recup.DateToDouble(debutProduit, dates3[i], finProduit)+2014, values3[i]));
                }
                for (int i = 0; i < values4.Count; i++)
                {
                    eurost.dataPoints_EURUSD.Add(new DataPoint(recup.DateToDouble(debutProduit, dates4[i], finProduit)+2014, values4[i]));
                }
                for (int i = 0; i < values5.Count; i++)
                {
                    eurost.dataPoints_EURAUD.Add(new DataPoint(recup.DateToDouble(debutProduit, dates5[i], finProduit)+2014, values5[i]));
                }




                List<string> symbols = recup.getSymbols();
                Dictionary<DateTime, double> dico = recup.getData(symbols[0]);


                eurost.covLogR = recup.exportCov(new DateTime(2004, 12,18 ), debutProduit);
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
                double H = 416;
                eurost.deltaEnt = wr1.getDeltaEurostral(eurost.pastPrice, eurost.t, H);
                //wr1.getPriceEurostral(eurost.t, pastPrice);
                eurost.prixEnt = wr1.getPriceEurostral(eurost.t, eurost.pastPrice);

                /*DateTime debutBackTest = new DateTime(2010, 03, 22);
                DateTime finBacktest = new DateTime(2018, 03, 22);
                double t0 = recup.DateToDouble(debutBackTest, finBacktest, finBacktest);
                double[,] donneesHistoriques = recup.exportPast(t0, 7, debutBackTest, finBacktest);
                eurost.PL=wr1.getPLEurostral(donneesHistoriques, H);*/

                // eurost.pastPrice = recup.exportPast(eurost.t, 182, debutProduit, finProduit);
                double[] track = new double[eurost.pastDelta.GetLength(0) - 1];
                double[] pp = new double[eurost.pastDelta.GetLength(0)];
                double[] pock = new double[eurost.pastDelta.GetLength(0) - 1];
                wr1.trackingError(eurost.pastDelta, eurost.t, H, pp, pock, track, eurost.pastDelta.GetLength(0));
                eurost.PandL = track;
                eurost.pock = pock;
                eurost.pp = pp;
                //Ajout
                for (int i = 0; i < 5; i++)
                {
                    eurost.prixActifs[i] = eurost.pastPrice[(eurost.pastPrice.GetLength(0)) - 1, i];
                }

                for (int j = 0; j < track.Count<double>(); j++)
                {
                    eurost.dataPoints_trackingError.Add(new DataPoint(j, track[j]));
                }

                for (int k = 0; k < pock.Count<double>(); k++)
                {
                    eurost.dataPoints_PfC.Add(new DataPoint(k, eurost.pock[k]));
                }

                for (int k = 0; k < pp.Count<double>(); k++)
                {
                    eurost.dataPoints_prix.Add(new DataPoint(k, eurost.pp[k]));
                }
                ViewData["price"] = eurost.prixEnt;
                //return RedirectToAction("PrixEnt");
                return View(eurost);
            }
        }

        public ActionResult Sous_Jacents()
        {            
            ViewBag.DataPoints_GSPC = JsonConvert.SerializeObject(eurost.dataPoints_GSPC);
            ViewBag.DataPoints_STOXX50E = JsonConvert.SerializeObject(eurost.dataPoints_STOXX50E);
            ViewBag.DataPoints_AXJO = JsonConvert.SerializeObject(eurost.dataPoints_AXJO);
            ViewBag.DataPoints_EURUSD = JsonConvert.SerializeObject(eurost.dataPoints_EURUSD);
            ViewBag.DataPoints_EURAUD = JsonConvert.SerializeObject(eurost.dataPoints_EURAUD);
            return View(eurost);
        }

        public ActionResult TrackingError()
        {
            ViewBag.getTrack= JsonConvert.SerializeObject(eurost.dataPoints_trackingError);
            return View(eurost);
        }

        public ActionResult PorteFeuilleCouverture()
        {
            ViewBag.PfC= JsonConvert.SerializeObject(eurost.dataPoints_PfC);
            ViewBag.Prix= JsonConvert.SerializeObject(eurost.dataPoints_prix);
            return View(eurost);
        }

        public ActionResult Reload()
        {
            return RedirectToAction("DeltaEnt");
        }

        public ActionResult Test()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";


            DataTable dt = new DataTable("MyTable");
            dt.Columns.Add(new DataColumn("Col1", typeof(string)));
            dt.Columns.Add(new DataColumn("Col2", typeof(string)));
            dt.Columns.Add(new DataColumn("Col3", typeof(string)));

            for (int i = 0; i < 3; i++)
            {
                DataRow row = dt.NewRow();
                row["Col1"] = "col 1, row " + i;
                row["Col2"] = "col 2, row " + i;
                row["Col3"] = "col 3, row " + i;
                dt.Rows.Add(row);
            }

            return View(dt); //passing the DataTable as my Model
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