using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tutorial7.DTO_s.Requests
{
    public class PromoteStudentRequest
    {
        [Required(ErrorMessage = "Name of study is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Semester is required")]
        public int Semester { get; set; }
    }
}