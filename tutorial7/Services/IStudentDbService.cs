using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tutorial7.DTO_s.Requests;
using tutorial7.DTO_s.Responses;

namespace tutorial7.Services
{
    public interface IStudentDbService
    {
        public PromoteStudentResponse PromoteStudents(PromoteStudentRequest request);
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
    }
}