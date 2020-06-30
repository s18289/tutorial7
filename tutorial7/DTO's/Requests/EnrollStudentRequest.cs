using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tutorial7.DTO_s.Requests
{
    public class EnrollStudentRequest
    {
        [Required(ErrorMessage = "Index number of student is required")]
        [MaxLength(100)]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
        [Required(ErrorMessage = "First name of student is required")]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name of student is required")]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Birth Date of student is required")]
        public string BirthDate { get; set; }
        [Required(ErrorMessage = "Name of studies is required")]
        public string Studies { get; set; }
        [Required(ErrorMessage = "Create new password")]
        public string Password { get; set; }
    }
}