using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Models
{
    public class RegisterModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 32 символов")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 32 символов")]
        public string CondPassword { get; set; }

        [Display(Name = "Роль")]
        [Required(ErrorMessage = "Не указана роль")]
        public int RoleId { get; set; }
    }
}
