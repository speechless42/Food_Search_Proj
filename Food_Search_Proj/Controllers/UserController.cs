using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            return RedirectToAction("Index");
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
            var UID = Session["user"].ToString();
            if (DB.User_Collect_Combo.Where(m => m.Combo_ID == id && m.User_ID == UID).Any() == true)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('你已收藏過');</script><meta http-equiv = \'refresh\' content = \'1; url = ../ShowCombo\' />");
            }
            else
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
                User_Collect_Combo UCC = new User_Collect_Combo();
                UCC.UCC_ID = UCCID;
                UCC.User_ID = UID;
                UCC.Combo_ID = id;
                DB.User_Collect_Combo.Add(UCC);
                DB.SaveChanges();
            }
            return RedirectToAction("ShowCombo");
        }
        //收藏菜餚
        public ActionResult CollectDishes(int id)
        {
            var UID = Session["user"].ToString();
            if (DB.User_Collect_Dishes.Where(m => m.Dishes_ID == id && m.User_ID == UID).Any() == true)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('你已收藏過');</script><meta http-equiv = \'refresh\' content = \'1; url = ../ShowDishes\' />");
            }
            else
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
                UCD.User_ID = UID;
                UCD.Dishes_ID = id;
                DB.User_Collect_Dishes.Add(UCD);
                DB.SaveChanges();
            }
            return RedirectToAction("ShowDishes");
        }
        //回饋文章
        public ActionResult CreateArticle()
        {
            //使用者先登入
            if (Session["user"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateArticle(Feedback_Article feedback_Article , HttpPostedFileBase image)
        {
            //if (!ModelState.IsValid) { return  View(feedback_Article); }
            if( image != null)
            {
                feedback_Article.Article_Photo = new byte[image.ContentLength];
                image.InputStream.Read(feedback_Article.Article_Photo, 0, image.ContentLength);
            }
            //ID 自動配置
            var CAID = 0;
            if ((from id in DB.Feedback_Article select id.Article_ID).Any() == false)
            {
                CAID = 0;
            }
            else
            {
                CAID = (from id in DB.Feedback_Article select id.Article_ID).Max();
                CAID += 1;
            }
            feedback_Article.Article_ID = CAID;
            //時間自動配置
            feedback_Article.Article_Date = DateTime.Now;
            //審核狀態自動配置
            feedback_Article.Article_Review_Result = 0;
            //使用者名稱自動配置
            feedback_Article.Article_User_ID = Session["user"].ToString();
            //照片處裡
            
            //全體存入資料庫
            DB.Feedback_Article.Add(feedback_Article);
            DB.SaveChanges();
            return RedirectToAction("ShowDishes");
        }

        //推薦菜餚
        public ActionResult RecommendDishes()
        {
            //使用者先登入
            if (Session["user"] == null)
            {
                return RedirectToAction("Index");
            }
            var DishID = 0;
            if ((from i in DB.Dishes select i.Dishes_ID).Any() == false)
            {
                ViewBag.DishID = 0;
            }
            else
            {
                DishID = (from i in DB.Dishes select i.Dishes_ID).Max();
                DishID += 1;
                ViewBag.DishID = DishID;
            }
            Session["DishesID"] = DishID;
            ViewBag.user = Session["user"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult RecommendDishes(Dishes dishes,HttpPostedFileBase image)
        {
            //if (!ModelState.IsValid) { return  View(); }
            if (image != null)
            {
                dishes.Dishes_Photo = new byte[image.ContentLength];
                image.InputStream.Read(dishes.Dishes_Photo, 0, image.ContentLength);
            }
            dishes.Dishes_Recommend_Date = DateTime.Now;
            dishes.Review_Manager_ID = "Root";
            DB.Dishes.Add(dishes);
            DB.SaveChanges();
            return RedirectToAction("RecommendDishes");
        }

        //更改帳戶資訊
        public ActionResult ChangeAccountDetail1()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangeAccountDetail1(string pwd)
        {
            string ID = Session["user"].ToString();
            var correct = DB.User.Where(m => m.User_ID == ID && m.User_Password == pwd).FirstOrDefault();
            if (correct == null) { 
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("ChangeAccountDetail2");
        }
        public ActionResult ChangeAccountDetail2()
        {
            string ID = Session["user"].ToString();
            User user = DB.User.Where(m => m.User_ID == ID).FirstOrDefault();
            return View(user);
        }
        [HttpPost]
        public ActionResult ChangeAccountDetail2(User user) {
            user.User_Password_Again = user.User_Password;
            DB.Entry(user).State = EntityState.Modified;
            DB.SaveChanges();
            return RedirectToAction("Pass");
        }
        public ActionResult Pass()
        {
            return View();
        }
        //詳細菜餚
        public ActionResult DetailsDishes(int id)
        {
            Dishes dishes = DB.Dishes.Where(m => m.Dishes_ID == id).FirstOrDefault();
            return View(dishes);
        }
    }

}