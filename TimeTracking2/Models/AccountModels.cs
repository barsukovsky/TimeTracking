using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTracking2.Models
{

    /// <summary>
    /// Представляет профиль сотрудника
    /// </summary>
    [Table("UserProfiles")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[0-9]+$")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Логин")]
        [RegularExpression(@"(?i)^[a-z0-9]{1,100}$", ErrorMessage = "Поле \"{0}\" должно содержать не более 100 латинских символов и цифр")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Имя")]
        [MaxLength(256, ErrorMessage = "Поле \"{0}\" должно содержать не более 256 символов")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        [MaxLength(256, ErrorMessage = "Поле \"{0}\" должно содержать не более 256 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(256, ErrorMessage = "Поле \"{0}\" должно содержать не более 256 символов")]
        public string MiddleName { get; set; }

        [Display(Name = "Должность")]
        [MaxLength(256, ErrorMessage = "Поле \"{0}\" должно содержать не более 256 символов")]
        public string Appointment { get; set; }

        [Display(Name = "Ставка")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Значение поля \"{0}\" должно быть неотрицательным числом")]
        public int? CurrentHourlyRate { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }

    /// <summary>
    /// Используется при регистрации нового пользователя
    /// </summary>
    public class RegisterViewModel
    {
        public UserProfile UserProfile { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(100, ErrorMessage = "Поле \"{0}\" должно содержать от {2} до {1} символов", MinimumLength = 5)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Используется при смене пароля пользователя
    /// </summary>
    public class PasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "Поле \"{0}\" должно содержать от {2} до {1} символов", MinimumLength = 5)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают")]
        public string ConfirmPassword { get; set; }
    }


    /// <summary>
    /// Используется при аутентификации пользователя
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}