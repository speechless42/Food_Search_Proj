using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Food_Search_Proj.Models;

namespace Food_Search_Proj.Controllers
{
    public class UserController : Controller
    {
        public Food_searchEntities DB = new Food_searchEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        //使用者註冊
        public ActionResult User_Sign_Up()
        {
            return View();
        }
        [HttpPost]
        public ActionResult User_Sign_Up(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            ViewBag.ERROR = false;
            ViewBag.again_ERROR = false;
            User new_User = new User();
            new_User.User_ID = user.User_ID;
            new_User.User_Password = user.User_Password;
            new_User.User_Password_Again = user.User_Password_Again;
            new_User.User_Nick_Name = user.User_Nick_Name;
            new_User.User_Mail = user.User_Mail;
            new_User.User_Phone = user.User_Phone;
            new_User.User_Birth = user.User_Birth;
            foreach (var item in DB.User)
            {
                if (item.User_ID == user.User_ID)
                {
                    ViewBag.ERROR = true;
                    return View();
                }
            }

            if (user.User_Password != user.User_Password_Again)
            {
                ViewBag.again_ERROR = true;
                return View();
            }
            DB.User.Add(new_User);
            DB.SaveChanges();
            return RedirectToAction("User_Sign_Up");
        }

        //使用者登入
        public string RandomCode() //驗證碼
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
        public ActionResult User_Sign_In()
        {
            string code = RandomCode();
            ViewBag.vertcode = code;
            Session["vertcode"] = code;
            return View();
        }
        [HttpPost]
        public ActionResult User_Sign_In(string ID, string Pwd, string vertycode)
        {
            string code = Session["vertcode"].ToString();
            var x = DB.User.Where(m => m.User_ID == ID && m.User_Password == Pwd).FirstOrDefault();
            if (x == null)
            {
                TempData["Message"] = "帳號or密碼錯誤，請重新確認登入";
                return RedirectToAction("User_Sign_In");
            }
            else if (vertycode != code)
            {
                TempData["Message"] = "驗證碼錯誤";
                return RedirectToAction("User_Sign_In");
            }
            Session.Clear();
            Session["user"] = ID;
            return RedirectToAction("User_Sign_In");
        }

        //顯示菜餚
        public ActionResult ShowDishes()
        {
            var AllDishes = DB.Dishes.OrderByDescending(m => m.Dishes_ID).ToList();
            return View(AllDishes);
        }
        //顯示套餐
        public ActionResult ShowCombo()
        {
            var AllCombo = DB.Combo.OrderByDescending(m => m.Combo_ID).ToList();
            return View(AllCombo);
        }
        //收藏套餐
        public ActionResult CollectCombo(int id)
        {
            var UCCID = 0;
            if ((from i in DB.User_Collect_Combo select i.UCC_ID).Any() == false)
            {
                UCCID = 0;
            }
            else
            {
                UCCID = (from i in DB.User_Collect_Combo select i.UCC_ID).Max();
                UCCID += 1;
            }
            User_Collect_Combo UCC  = new User_Collect_Combo();
            UCC.Combo_ID = id;
            UCC.User_ID = Session["user"].ToString();
            UCC.UCC_ID = UCCID;
            DB.User_Collect_Combo.Add(UCC);
            DB.SaveChanges();
            return RedirectToAction("ShowCombo");
        }
        //收藏菜餚
        public ActionResult CollectDishes(int id)
        {
            var UCDID = 0;
            if ((from i in DB.User_Collect_Dishes select i.UCD_ID).Any() == false)
            {
                UCDID = 0;
            }
            else
            {
                UCDID = (from i in DB.User_Collect_Dishes select i.UCD_ID).Max();
                UCDID += 1;
            }
            User_Collect_Dishes UCD = new User_Collect_Dishes();
            UCD.UCD_ID = UCDID;
            UCD.User_ID = Session["user"].ToString();
            UCD.Dishes_ID = id;
            DB.User_Collect_Dishes.Add(UCD);
            DB.SaveChanges();
            return RedirectToAction("ShowDishes");
        }
        //回饋文章
        public ActionResult CreateArticle()
        {

            return View();
        }

    }

}