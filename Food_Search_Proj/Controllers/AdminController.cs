using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Food_Search_Proj.Controllers
{
    public class AdminController : Controller
    {
        Models.Food_searchEntities DN = new
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

    }
}