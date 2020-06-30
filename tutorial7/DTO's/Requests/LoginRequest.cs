using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tutorial7.DTO_s.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Login and password are required")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Login and password are required")]
        public string Password { get; set; }
    }
}