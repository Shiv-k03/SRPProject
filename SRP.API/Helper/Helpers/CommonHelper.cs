using Microsoft.AspNetCore.Mvc;
using SRP.API.Helper.Base;
using System.Security.Claims;

namespace SRP.API.Helper.Helpers
{
    public static class CommonHelper
    {
        public static void SetUserInformation<T>(ref T request, int? recordId, HttpContext httpContext) where T : class
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                var createdByProperty = typeof(T).GetProperty("CreatedBy");
                var updatedByProperty = typeof(T).GetProperty("UpdatedBy");

                if (recordId == null || recordId == 0)
                {
                    // New record
                    createdByProperty?.SetValue(request, username);
                    updatedByProperty?.SetValue(request, username);
                }
                else
                {
                    // Existing record
                    updatedByProperty?.SetValue(request, username);
                }
            }
        }

        public static IActionResult ReturnActionResultByStatus(ResultModel result, ControllerBase cntbase)
        {
            if (result == null)
                return cntbase.NotFound(result);
            else if (result.StatusCode == ResultCode.SuccessfullyCreated)
                return cntbase.Created("", result);
            else if (result.StatusCode == ResultCode.SuccessfullyUpdated)
                return cntbase.Ok(result);
            else if (result.StatusCode == ResultCode.Success)
                return cntbase.Ok(result);
            else if (result.StatusCode == ResultCode.Unauthorized)
                return cntbase.Unauthorized(result);
            else if (result.StatusCode == ResultCode.DuplicateRecord)
                return cntbase.Conflict(result);
            else if (result.StatusCode == ResultCode.RecordNotFound)
                return cntbase.NotFound(result);
            else if (result.StatusCode == ResultCode.NotAllowed)
                return cntbase.BadRequest(result);
            else if (result.StatusCode == ResultCode.Invalid)
                return cntbase.BadRequest(result);
            else if (result.StatusCode == ResultCode.ExceptionThrown)
                return cntbase.StatusCode(500, result);

            return cntbase.BadRequest(result);
        }
    }
}
