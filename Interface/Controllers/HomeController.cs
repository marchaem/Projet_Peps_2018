﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Interface.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MyChart()
        {
            new System.Web.Helpers.Chart(width: 800, height: 200)
                .AddSeries(chartType: "column",
                xValue: new[] { "jan", "feb" },
                yValues: new[] { 1, 2, 5,10 })
                .AddLegend("Prix");
            return null;
                

        }
    }
}