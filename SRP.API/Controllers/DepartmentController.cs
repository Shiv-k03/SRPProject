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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// Add or Update Department (Admin only)        
        [HttpPost("addOrUpdate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateDepartment([FromBody] DepartmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, request.DepartmentId, HttpContext);
            var result = await _departmentService.AddOrUpdateDepartmentAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        /// Get department by ID        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var result = await _departmentService.GetDepartmentByIdAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        /// Get departments by filter        
        [HttpPost("filter")]
        public async Task<IActionResult> GetDepartmentByFilter([FromBody] DepartmentFilterModel filter)
        {
            var result = await _departmentService.GetDepartmentByFilterAsync(filter);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        /// Delete department (Admin only)        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}
