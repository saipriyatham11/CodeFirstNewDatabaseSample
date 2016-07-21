using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace new.Controllers
{
    public class romeController : Controller
    {
    public viewResult Index()
    {
        ViewBag.Countries = new list <string>()
        {
           "india",
           "usa",
           "uk"
        }
         return View();
    }
    }
}