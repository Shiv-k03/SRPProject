using Microsoft.AspNetCore.Authorization;
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
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        
        /// Add or Update Subject (Admin only)        
        [HttpPost("addOrUpdate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateSubject([FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, request.SubjectId, HttpContext);
            var result = await _subjectService.AddOrUpdateSubjectAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get subject by ID       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var result = await _subjectService.GetSubjectByIdAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get subjects by filter        
        [HttpPost("filter")]
        public async Task<IActionResult> GetSubjectByFilter([FromBody] SubjectFilterModel filter)
        {
            var result = await _subjectService.GetSubjectByFilterAsync(filter);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Delete subject (Admin only)        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = await _subjectService.DeleteSubjectAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}