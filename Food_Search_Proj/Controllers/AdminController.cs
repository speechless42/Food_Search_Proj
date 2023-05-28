using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Food_Search_Proj.Models;
using Microsoft.Ajax.Utilities;

namespace Food_Search_Proj.Controllers
{
    public class AdminController : Controller
    {
        public Food_searchEntities DB = new Food_searchEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        //新增類別區域
        public ActionResult CreateCategoriesFood()
        {
            //自動編號，判斷是否回傳空值，若不是空值就搜尋最大值+1。相反則回傳0
            var COFID = 0;
            if ((from i in DB.Categories_Of_Food select i.Categories_Of_Food_ID).Any() == false)
            {
                ViewBag.COFID = COFID;
            }
            else { 
                COFID = (from i in DB.Categories_Of_Food select i.Categories_Of_Food_ID).Max();
                COFID += 1;
                ViewBag.COFID = COFID;
            }
            
            
            return View();
        }
        [HttpPost]
        public ActionResult CreateCategoriesFood(Categories_Of_Food C_O_F)
        {
            if (!ModelState.IsValid)
            {
                return View(C_O_F);
            }
            DB.Categories_Of_Food.Add(C_O_F);
            DB.SaveChanges();
            return RedirectToAction("PASS");
        }
        //新增食材區域
        public ActionResult CreateFood()
        {
            //自動編號，判斷是否回傳空值，若不是空值就搜尋最大值+1。相反則回傳0
            var FDID = 0;
            if ((from i in DB.Food select i.Food_ID).Any() == false)
            {
                ViewBag.FDID = FDID;
            }
            else
            {
                FDID = (from i in DB.Food select i.Food_ID).Max();
                FDID += 1;
                ViewBag.FDID = FDID;
            }
            //(1)透過分類名稱加入ID，先把名稱搜尋出。
            List<SelectListItem> CateogoriseOfFoodName = new List<SelectListItem>();
            foreach(var item in DB.Categories_Of_Food)
            {
                CateogoriseOfFoodName.Add(new SelectListItem { Text=item.Categories_Of_Food_Name , Value = item.Categories_Of_Food_ID.ToString() });
            }
            ViewBag.Categories_Of_Food_Value = CateogoriseOfFoodName;
            return View();
        }
        [HttpPost]
        public ActionResult CreateFood(Food food)
        {
            //(2)轉換字串為整數
            int COFID = int.Parse(food.Categories_Of_Food_Value);
            food.Categories_Of_Food_ID = COFID;
            DB.Food.Add(food);
            DB.SaveChanges();
            return RedirectToAction("PASS");
        }

        //新增食譜
        public ActionResult CreateDishes()
        {
            return View();
        }

        public ActionResult PASS() {
            return View();
        }


    }
}