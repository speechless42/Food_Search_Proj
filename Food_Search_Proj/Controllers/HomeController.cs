using Food_Search_Proj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Food_Search_Proj.Controllers
{

    public class HomeController : Controller
    {
        public Food_searchEntities DB = new Food_searchEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HomeShowDishes()
        {
            var AllDishes = DB.Dishes.OrderByDescending(m => m.Dishes_ID).ToList();
            return View(AllDishes);
        }
        public ActionResult DetailsDishes(int id)
        {
            Dishes dishes = DB.Dishes.Where(m => m.Dishes_ID == id).FirstOrDefault();
            return View(dishes);
        }
    }
}