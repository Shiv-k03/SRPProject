using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Request;
using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Helpers;

namespace SRP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// Add or Update Student (Admin only)        
        [HttpPost("addOrUpdate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateStudent([FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, request.StudentId, HttpContext);
            var result = await _studentService.AddOrUpdateStudentAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get student by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get students by filter        
        [HttpPost("filter")]
        [Authorize(Roles = "Admin,HOD,Teacher")]
        public async Task<IActionResult> GetStudentByFilter([FromBody] StudentFilterModel filter)
        {
            var result = await _studentService.GetStudentByFilterAsync(filter);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
      
        /// Delete student (Admin only)        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Assign subject to student (Admin or HOD)        
        [HttpPost("assign-subject")]
        [Authorize(Roles = "Admin,HOD")]
        public async Task<IActionResult> AssignSubjectToStudent([FromBody] AssignSubjectToStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, null, HttpContext);
            var result = await _studentService.AssignSubjectToStudentAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Remove subject from student (Admin or HOD)        
        [HttpDelete("{studentId}/subjects/{subjectId}")]
        [Authorize(Roles = "Admin,HOD")]
        public async Task<IActionResult> RemoveSubjectFromStudent(int studentId, int subjectId)
        {
            var result = await _studentService.RemoveSubjectFromStudentAsync(studentId, subjectId);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

       
        /// Get student summary       
        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetStudentSummary(int id)
        {
            var result = await _studentService.GetStudentSummaryAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get student detailed report       
        [HttpGet("{id}/detailed-report")]
        public async Task<IActionResult> GetStudentDetailedReport(int id)
        {
            var result = await _studentService.GetStudentDetailedReportAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}
