using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Food_Search_Proj.Models
{
    public class AdminChangePassword
    {
        [Display(Name = "管理者密碼")]
        [StringLength(20, ErrorMessage = "長度必須介於6~20字", MinimumLength = 6)]
        public string Admin_Password { get; set; }
        [Display(Name = "管理者新密碼")]
        [StringLength(20, ErrorMessage = "長度必須介於6~20字", MinimumLength = 6)]
        public string Admin_New_Password { get; set; }
        [Display(Name = "管理者新密碼")]
        [Compare("Admin_New_Password", ErrorMessage = "兩次輸入的密碼必須相符！")]
        [StringLength(20, ErrorMessage = "長度必須介於6~20字", MinimumLength = 6)]
        public string Admin_New_Password_Again { get; set; }
    }
}