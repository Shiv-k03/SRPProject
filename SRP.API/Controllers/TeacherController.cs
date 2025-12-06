using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Helpers;

namespace SRP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        
        /// Add or Update Teacher (Admin only)
        [HttpPost("addOrUpdate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateTeacher([FromBody] TeacherRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, request.TeacherId, HttpContext);
            var result = await _teacherService.AddOrUpdateTeacherAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get teacher by ID        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacherById(int id)
        {
            var result = await _teacherService.GetTeacherByIdAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get teachers by filter        
        [HttpPost("filter")]
        public async Task<IActionResult> GetTeacherByFilter([FromBody] TeacherFilterModel filter)
        {
            var result = await _teacherService.GetTeacherByFilterAsync(filter);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Delete teacher (Admin only)        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Assign subject to teacher (Admin or HOD)       
        [HttpPost("assign-subject")]
        [Authorize(Roles = "Admin,HOD")]
        public async Task<IActionResult> AssignSubjectToTeacher([FromBody] AssignSubjectToTeacherRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, null, HttpContext);
            var result = await _teacherService.AssignSubjectToTeacherAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Remove subject from teacher (Admin or HOD)        
        [HttpDelete("{teacherId}/subjects/{subjectId}")]
        [Authorize(Roles = "Admin,HOD")]
        public async Task<IActionResult> RemoveSubjectFromTeacher(int teacherId, int subjectId)
        {
            var result = await _teacherService.RemoveSubjectFromTeacherAsync(teacherId, subjectId);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get teacher summary       
        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetTeacherSummary(int id)
        {
            var result = await _teacherService.GetTeacherSummaryAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}
