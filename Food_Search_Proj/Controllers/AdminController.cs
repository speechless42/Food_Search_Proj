using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Food_Search_Proj.Models;

namespace Food_Search_Proj.Controllers
{
    public class AdminController : Controller
    {
        public Food_searchEntities DB = new Food_searchEntities();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return Redirect("~/Home/Index");
            }
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
            return RedirectToAction("Index");
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
            if (Session["user"] == null)
            {
                return RedirectToAction("Loss");
            }
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

            ViewBag.User = Session["user"];
            Session["DishesID"] = DishID;
            ViewBag.ReviewResult = 2;
            return View();


        }
        [HttpPost]
        public ActionResult CreateDishes(Dishes dishes,HttpPostedFileBase image)
        {
            if (ModelState.IsValid) { 
            if(image != null)
            {
                dishes.Dishes_Photo = new byte[image.ContentLength];
                image.InputStream.Read(dishes.Dishes_Photo, 0, image.ContentLength);
            }
            //自動填審核管理員帳號
            dishes.Review_Manager_ID = Session["user"].ToString();
            dishes.Referral_User_ID = Session["user"].ToString();
            //自動填審核結果
            dishes.Food_Review_Result = 2;
            //自動填日期
            dishes.Dishes_Recommend_Date = DateTime.Now;
            dishes.Food_Review_Date = DateTime.Now;
            DB.Dishes.Add(dishes);
            DB.SaveChanges();
            return RedirectToAction("PASS");
            }
            return RedirectToAction("Loss");
        }
        ///////////////
        //菜餚中的食材
        public ActionResult DishesOfFood(int count) {
            List<SelectListItem> DishesContainFood = new List<SelectListItem>() { };
            DishesContainFood.Add(new SelectListItem { Text = "請選擇食材", Value = "-1"});
            foreach (var item in DB.Food)
            {
                DishesContainFood.Add(new SelectListItem { Text = item.Food_ID.ToString() + "-" + item.Food_Name, Value = item.Food_ID.ToString() });
            }
            var DCFID = 0;
            if ((from i in DB.Dishes_Contain_Food select i.DCF_ID).Any() == false)
            {
                ViewBag.DCFID = DCFID;
            }
            else
            {
                DCFID = (from i in DB.Dishes_Contain_Food select i.DCF_ID).Max();
            }
            count += DCFID;
            ViewBag.DishesContainFoodName = DishesContainFood;
            ViewBag.DishID = Session["DishesID"].ToString();
            ViewBag.count = count;
            ViewBag.SelsectFood = "SelectFood" + count.ToString();
            ViewBag.BeSelsectFood = "BeSelectFood" + count.ToString();
            return PartialView("DishesOfFood");
        }
        ///////////////
        //顯示菜餚
        public ActionResult ShowDishes()
        {
            var AllDishes = DB.Dishes.OrderByDescending(m => m.Dishes_ID).ToList();

            List<SelectListItem> DishesContainFoodsss = new List<SelectListItem>() { };
            DishesContainFoodsss.Add(new SelectListItem { Text = "請選擇食材", Value = "-1" });

            foreach (var item in DB.Food)
            {
                DishesContainFoodsss.Add(new SelectListItem { Text = item.Food_ID.ToString() + "-" + item.Food_Name, Value = item.Food_ID.ToString() });
            }
            Session["DishesContainFoodName"] = DishesContainFoodsss;
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
        public ActionResult EditDishes(Dishes dishes,HttpPostedFileBase image)
        {
            if (image != null)
            {
                dishes.Dishes_Photo = new byte[image.ContentLength];
                image.InputStream.Read(dishes.Dishes_Photo, 0, image.ContentLength);
            }
            Dishes Editdishe = DB.Dishes.Where(m => m.Dishes_ID == dishes.Dishes_ID).FirstOrDefault();
            //Editdishe.Dishes_Name = dishes.Dishes_Name;
            //Editdishe.Dishes_Methods = dishes.Dishes_Methods;
            //Editdishe.Dishes_Remark = dishes.Dishes_Remark;
            //Editdishe.Dishes_Contain_Food = dishes.Dishes_Contain_Food;
            DB.Dishes.Remove(Editdishe);
            DB.Dishes.Add(dishes);
            DB.SaveChanges();
            return RedirectToAction("ShowDishes");
        }
        ///////////////
        //編輯菜餚的食材
        public ActionResult EditDishesOfFood(Dishes_Contain_Food DCF)
        {
            return PartialView("EditDishesOfFood");
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
            var ComboID = 0;
            if ((from i in DB.Combo select i.Combo_ID).Any() == false)
            {
                ViewBag.ComboID = ComboID;
            }
            else
            {
                ComboID = (from i in DB.Combo select i.Combo_ID).Max();
                ComboID += 1;
                ViewBag.ComboID = ComboID;
            }
            
            Session["ComboID"] = ComboID;
            return View();

        }
        [HttpPost]
        public ActionResult CreateCombo(Combo combo)
        {
            DB.Combo.Add(combo);
            DB.SaveChanges();
            return RedirectToAction("PASS");
        }
        ///////////////
        //套餐的菜餚
        public ActionResult ComboDishes(int count)
        {
            var SMID = 0;
            if ((from i in DB.Set_Menu_Includes_Dishes select i.SMID_ID).Any() == false)
            {
                SMID = 0;
            }
            else
            {
                SMID = (from i in DB.Set_Menu_Includes_Dishes select i.SMID_ID).Max();
                SMID += 1;
            }
            ViewBag.count = count + SMID;
            List<SelectListItem> SMIFName = new List<SelectListItem>();
            foreach(var item in DB.Dishes)
            {
                SMIFName.Add(new SelectListItem { Text = item.Dishes_ID.ToString() + "-" + item.Dishes_Name, Value = item.Dishes_ID.ToString() });
            }
            ViewBag.SMIFName = SMIFName;
            ViewBag.ComboID = Session["ComboID"].ToString();
            ViewBag.SelsectDishes = "SelectFood" + count.ToString();
            ViewBag.BeSelsectDishes = "BeSelectFood" + count.ToString();
            return PartialView("ComboDishes");
        }
        ///////////////
        //顯示套餐
        public ActionResult ShowCombo()
        {
            var AllCombo = DB.Combo.OrderByDescending(m => m.Combo_ID).ToList();

            List<SelectListItem> SMIFName = new List<SelectListItem>() { };
            SMIFName.Add(new SelectListItem { Text = "請選擇菜餚", Value = "-1" });

            foreach (var item in DB.Dishes)
            {
                SMIFName.Add(new SelectListItem { Text = item.Dishes_ID.ToString() + "-" + item.Dishes_Name, Value = item.Dishes_ID.ToString() });
            }

            Session["SMIFName"] = SMIFName;
            return View(AllCombo);
        }
        ///////////////
        //編輯套餐
        public ActionResult EditCombo(int id)
        {
            Combo combo = DB.Combo.Where(m => m.Combo_ID == id).FirstOrDefault();

            return View(combo);
        }
        [HttpPost]
        public ActionResult EditCombo(Combo combo)
        {
            Combo EditCombo = DB.Combo.Where(m => m.Combo_ID == combo.Combo_ID).FirstOrDefault();
            DB.Combo.Remove(EditCombo);
            DB.Combo.Add(combo);
            DB.SaveChanges();
            return RedirectToAction("ShowCombo");
        }
        ///////////////
        //編輯套餐的菜餚
        public ActionResult EditComboDishes(Set_Menu_Includes_Dishes SMID)
        {
            return PartialView("EditComboDishes");
        }
        //刪除套餐
        public ActionResult DeleteCombo(int id)
        {
            Combo combo = DB.Combo.Where(m => m.Combo_ID == id).FirstOrDefault();
            DB.Combo.Remove(combo);
            DB.SaveChanges();
            return RedirectToAction("ShowCombo");
        }
        ///////////////
        //列出新的回饋文章
        public ActionResult ShowFeedBackArticle()
        {
            var feedback_Article = DB.Feedback_Article.Where( m => m.Article_Review_Result == 0 ).OrderByDescending(m => m.Article_ID).ToList();
            return View(feedback_Article);
        }
        ///////////////
        //列出新的未審核菜餚
        public ActionResult ShowNoResultDishes()
        {
            var NoResultDishes = DB.Dishes.Where(m => m.Food_Review_Result == 0).OrderByDescending(m => m.Dishes_ID).ToList();
            return View(NoResultDishes);
        }
        ///////////////
        //審核未審核菜餚
        public ActionResult EDishes() { return View(); }   
        
    }
}