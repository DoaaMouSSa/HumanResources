using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class AuthDto
    {
        public class Register
        {
            [Required(ErrorMessage = "البريد الالكترونى مطلوب")]
            [EmailAddress(ErrorMessage = "البريد الالكتروني غير صالح")]
            public string Email { get; set; }
            [Required(ErrorMessage = "كلمة المرور مطلوبة")]
            [MinLength(4, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل4 أحرف")]
            public string Password { get; set; }
        }
        public class Login
        {
            [Required(ErrorMessage = "البريد الالكترونى مطلوب")]
            public string Email { get; set; }
            [Required(ErrorMessage = "كلمة المرور مطلوبة")]
            [DataType(DataType.Password)]

            public string Password { get; set; }
            public bool RememberMe { get; set; }

        }
    }
}
