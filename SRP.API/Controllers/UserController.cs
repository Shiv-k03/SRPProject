using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Helpers;
using System.Security.Claims;

namespace SRP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        
        /// Get user by ID (Admin only)        
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

       
        /// Get current user profile        
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _userService.GetUserProfileAsync(userId);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Get users by filter (Admin only)       
        [HttpPost("filter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByFilter([FromBody] UserFilterModel filter)
        {
            var result = await _userService.GetUserByFilterAsync(filter);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Update user (Admin only)        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            request.UpdatedBy = username;

            var result = await _userService.UpdateUserAsync(id, request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Deactivate user (Admin only)        
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var result = await _userService.DeactivateUserAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        
        /// Activate user (Admin only)        
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var result = await _userService.ActivateUserAsync(id);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}
