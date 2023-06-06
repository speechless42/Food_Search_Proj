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
        Food_searchEntities DB = new Food_searchEntities();
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
        public ActionResult User_Sign_In(string ID, string Pwd, string vertycode) {
            string code = Session["vertcode"].ToString();
            var x = DB.User.Where(m => m.User_ID == ID && m.User_Password == Pwd).FirstOrDefault();
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

        //使用者收藏菜餚
    
    }
        
}