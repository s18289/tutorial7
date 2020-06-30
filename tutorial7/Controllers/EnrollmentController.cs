using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tutorial7.DTO_s.Requests;
using tutorial7.DTO_s.Responses;
using tutorial7.Services;

namespace tutorial7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IStudentDbService studentDbService;

        public EnrollmentController(IStudentDbService _studentDbService)
        {
            studentDbService = _studentDbService;
        }

        [HttpPost(Name = "EnrollStudent")]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response = null;
            try
            {
                response = studentDbService.EnrollStudent(request);
                if (response == null) return NotFound("Student was not found");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Created("EnrollStudent", response);
        }


        [Authorize(Roles = "employee")]
        [HttpPost(Name = "PromoteStudent")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            PromoteStudentResponse response = null;
            try
            {
                response = studentDbService.PromoteStudents(request);
            }
            catch (SqlException ex)
            {}
            return Created("PromoteStudent", response);
        }
    }
}