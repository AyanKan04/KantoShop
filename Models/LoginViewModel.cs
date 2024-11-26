using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KantoShop.Models;
using System.ComponentModel.DataAnnotations;

namespace KantoShop.Models
{
    public class LoginViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
    }
}