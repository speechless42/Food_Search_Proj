using System;
using System.Collections.Generic;
using System.Linq;
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
        //更改當中
        public ActionResult User_Sign_Up()   //使用者註冊
        {
            return View();
        }
        [HttpPost]
        public ActionResult User_Sign_Up(User user)   //使用者註冊
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
    }
}