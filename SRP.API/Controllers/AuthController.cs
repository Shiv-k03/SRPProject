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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// User login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            var result = await _authService.LoginAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }


        /// Change password (Authenticated users)        
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data model");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data model");

            var result = await _authService.RefreshTokenAsync(request);
            return CommonHelper.ReturnActionResultByStatus(result, this);
        }
    }
}
