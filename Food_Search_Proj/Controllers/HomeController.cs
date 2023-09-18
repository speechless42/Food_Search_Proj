using Food_Search_Proj.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var AllDishes = DB.Dishes.Where(m => m.Food_Review_Result == 2).ToList();
            return View(AllDishes);
        }
        public ActionResult DetailsDishes(int id)
        {
            Dishes dishes = DB.Dishes.Where(m => m.Dishes_ID == id).FirstOrDefault();
            return View(dishes);
        }
        //搜尋菜餚名稱
        public ActionResult SearchDishes()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchDishes(string DishesName)
        {
            var Dishes = DB.Dishes.Where( m => m.Dishes_Name.Contains(DishesName));
            return View(Dishes);
        }
        //搜尋套餐名稱
        public ActionResult SearchCombo()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchCombo(string ComboName)
        {
            var Combo = DB.Combo.Where(m => m.Combo_Name.Contains(ComboName));
            return View(Combo);
        }
        //顯示套餐
        public ActionResult HomeShowCombo()
        {
            var AllCombo = DB.Combo.OrderByDescending(m => m.Combo_ID).ToList();
            return View(AllCombo);
        }
        //顯示套餐細節
        public ActionResult DetailsCombo(int id)
        {
            Combo combo = DB.Combo.Where(m => m.Combo_ID == id).FirstOrDefault();
            return View(combo);
        }
    }
}