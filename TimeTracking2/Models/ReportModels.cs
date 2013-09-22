using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTracking2.Models
{
    [Table("Reports")]
    public class Report
    {
        [Key, Column(Order = 1)]
        [Required]
        [Display(Name="Год")]
        [RegularExpression(@"^[0-9]{1,4}$", ErrorMessage = "Значение поля \"{0}\" должно соответствовать формату \"YYYY\"")]
        public int Year { get; set; }

        [Key, Column(Order = 2)]
        [Required]
        [Display(Name = "Месяц")]
        [Range(1, 12, ErrorMessage = "Значение поля \"{0}\" должно быть числом от 1 до 12")]
        public int Month { get; set; }

        [Key, Column(Order = 0)]
        [Required]
        [Display(Name = "Id сотрудника")]
        [RegularExpression(@"^[0-9]+$")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }

        [Required]
        [Display(Name = "Ставка")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Значение поля \"{0}\" должно быть неотрицательным числом")]
        public int HourlyRate { get; set; }

        [Required]
        [Display(Name = "Часы")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Значение поля \"{0}\" должно быть неотрицательным числом")]
        public int Hours { get; set; }
    }

    public class ReportsOfUserViewModel
    {
        public UserProfile UserProfile { get; set; }
        
        public IQueryable<Report> Reports { get; set; }
    }
}