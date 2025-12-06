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
    public class MarkController : ControllerBase
    {
        private readonly IMarkService _markService;

        public MarkController(IMarkService markService)
        {
            _markService = markService;
        }

        
        /// Add or Update Mark (Teacher, HOD, Admin)        
        [HttpPost("addOrUpdate")]
        [Authorize(Roles = "Admin,HOD,Teacher")]
        public async Task<IActionResult> AddOrUpdateMark([FromBody] MarkRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            CommonHelper.SetUserInformation(ref request, request.MarkId, HttpContext);
            var result = await _markService.AddOrUpdateMarkAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get mark by ID        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarkById(int id)
        {
            var result = await _markService.GetMarkByIdAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get marks by filter        
        [HttpPost("filter")]
        public async Task<IActionResult> GetMarkByFilter([FromBody] MarkFilterModel filter)
        {
            var result = await _markService.GetMarkByFilterAsync(filter);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Delete mark (Admin only)        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            var result = await _markService.DeleteMarkAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}
