using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Food_Search_Proj.Models;

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

        //*新增類別區域
        public ActionResult CreateCategoriesFood()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("Loss");
            //}
            //自動編號，判斷是否回傳空值，若不是空值就搜尋最大值+1。相反則回傳0
            var COFID = 0;
            if ((from i in DB.Categories_Of_Food select i.Categories_Of_Food_ID).Any() == false)
            {
                ViewBag.COFID = COFID;
            }
            else
            {
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
            return RedirectToAction("CreateCategoriesFood");
        }
        ///////////////
        //*新增食材區域
        public ActionResult CreateFood()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("Loss");
            //}
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
            foreach (var item in DB.Categories_Of_Food)
            {
                CateogoriseOfFoodName.Add(new SelectListItem { Text = item.Categories_Of_Food_Name, Value = item.Categories_Of_Food_ID.ToString() });
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
            return RedirectToAction("CreateFood");
        }
        ///////////////
        //驗證碼
        public string RandomCode()
        {
            string s = "0123456789abcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            int index;
            for (int i = 0; i < 4; i++)
            {
                index = rand.Next(0, s.Length);
                sb.Append(s[index]);
            }
            return sb.ToString();
        }
        //管理者登入
        public ActionResult Login()
        {
            string code = RandomCode();
            ViewBag.vertcode = code;
            Session["vertcode"] = code;
            return View();
        }
        [HttpPost]
        public ActionResult Login(string ID, string Pwd, string vertycode)
        {
            string code = Session["vertcode"].ToString();
            var x = DB.Manager.Where(m => m.Manager_ID == ID && m.Manager_Password == Pwd).FirstOrDefault();
            if (x == null)
            {

                TempData["Message"] = "帳號or密碼錯誤，請重新確認登入";
                return RedirectToAction("Login");

            }
            else if (vertycode != code)
            {
                TempData["Message"] = "驗證碼錯誤";
                return RedirectToAction("Login");
            }
            Session.Clear();
            Session["user"] = ID;
            return RedirectToAction("PASS");
        }
        ///////////////
        //管理者更改密碼
        public ActionResult ChangePassword()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Loss");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(AdminChangePassword adminChangePassword)
        {
            string UserID = Session["user"].ToString();
            if (!ModelState.IsValid)
            {
                return View(adminChangePassword);
            }
            Manager manager = (from id in DB.Manager
                               where id.Manager_ID == UserID
                               select id).FirstOrDefault();
            if (manager.Manager_Password == adminChangePassword.Admin_Password)
            {
                manager.Manager_Password = adminChangePassword.Admin_New_Password;
                DB.SaveChanges();

                return View();
            }
            ViewBag.Error = "舊密碼輸入錯誤";
            return View(adminChangePassword);
        }
        ///////////////
        //新增菜餚
        public ActionResult CreateDishes()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("Loss");
            //}
            //自動編號ID
            var DishID = 0;
            if ((from i in DB.Dishes select i.Dishes_ID).Any() == false)
            {
                ViewBag.DishID = DishID;
            }
            else
            {
                DishID = (from i in DB.Dishes select i.Dishes_ID).Max();
                DishID += 1;
                ViewBag.DishID = DishID;
            }
            //食材
            List<SelectListItem> DishesContainFood = new List<SelectListItem>() { };
            foreach (var item in DB.Food)
            {
                DishesContainFood.Add(new SelectListItem { Text = item.Food_Name, Value = item.Food_ID.ToString() });
            }
            ViewBag.User = Session["user"];
            ViewBag.ReviewResult = 2;
            ViewBag.Dishes_Contain_Food_Value = DishesContainFood;
            return View();


        }
        [HttpPost]
        public ActionResult CreateDishes(Dishes dishes)
        {
            //List<Dishes_Contain_Food> DCF = new List<Dishes_Contain_Food>();
            Dishes_Contain_Food DCF1 = new Dishes_Contain_Food();
            if((from i in DB.Dishes_Contain_Food select i.DCF_ID).Any() == false)
            {
                DCF1.DCF_ID = 0;
            }
            else
            {
                DCF1.DCF_ID = (from i in DB.Dishes_Contain_Food select i.DCF_ID).Max();
            }
            foreach (var ID in dishes.Dishes_Contain_Food_Value)
            {
                //DCF.Add(new Dishes_Contain_Food { Dishes_ID = dishes.Dishes_ID , Food_ID = int.Parse(ID) });
                if(DCF1.DCF_ID != 0)
                {
                    DCF1.DCF_ID += 1;
                }
                DCF1.Count = "999把";
                DCF1.Dishes_ID = dishes.Dishes_ID;
                DCF1.Food_ID = int.Parse(ID);
                DB.Dishes.
                DB.SaveChanges();
            }
            DB.Dishes.Add(dishes);
            DB.SaveChanges();
            ViewBag.show = dishes.Dishes_Contain_Food_Value[1];
            return RedirectToAction("PASS");
        }
        ///////////////
        //顯示菜餚
        public ActionResult ShowDishes()
        {
            var AllDishes = DB.Dishes.OrderByDescending(m => m.Dishes_ID).ToList();
            return View(AllDishes);
        }
        ///////////////
        //編輯菜餚
        public ActionResult EditDishes(int id)
        {
            Dishes dishes = DB.Dishes.Where(m => m.Dishes_ID == id).FirstOrDefault();
            return View(dishes);
        }
        [HttpPost]
        public ActionResult EditDishes(Dishes dishes)
        {
            Dishes Editdishe = DB.Dishes.Where(m => m.Dishes_ID == dishes.Dishes_ID).FirstOrDefault();
            Editdishe.Dishes_Name = dishes.Dishes_Name;
            Editdishe.Dishes_Methods = dishes.Dishes_Methods;
            Editdishe.Dishes_Remark = dishes.Dishes_Remark;
            DB.SaveChanges();
            return RedirectToAction("ShowDishes");
        }
        ///////////////
        //刪除菜餚
        public ActionResult DeleteDishes(int id)
        {
            Dishes dishes = DB.Dishes.Where(m => m.Dishes_ID == id).FirstOrDefault();
            DB.Dishes.Remove(dishes);
            DB.SaveChanges();
            return RedirectToAction("ShowDishes");
        }
        ///////////////
        //建立套餐
        public ActionResult CreateCombo()
        {
            return View();
        }
    }
}