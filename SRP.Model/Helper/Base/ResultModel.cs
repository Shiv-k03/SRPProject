namespace SRP.Model.Helper.Base
{
    public class ResultModel
    {
        public ResultCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }

        public ResultModel()
        {
        }

        public ResultModel(ResultCode statusCode, string message, object? data = null)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static ResultModel Success(object? data = null, string message = "Operation successful")
        {
            return new ResultModel(ResultCode.Success, message, data);
        }

        public static ResultModel Created(object? data = null, string message = "Record created successfully")
        {
            return new ResultModel(ResultCode.SuccessfullyCreated, message, data);
        }

        public static ResultModel Updated(object? data = null, string message = "Record updated successfully")
        {
            return new ResultModel(ResultCode.SuccessfullyUpdated, message, data);
        }

        public static ResultModel NotFound(string message = "Record not found")
        {
            return new ResultModel(ResultCode.RecordNotFound, message);
        }

        public static ResultModel Duplicate(string message = "Duplicate record found")
        {
            return new ResultModel(ResultCode.DuplicateRecord, message);
        }

        public static ResultModel Invalid(string message = "Invalid request")
        {
            return new ResultModel(ResultCode.Invalid, message);
        }

        public static ResultModel Unauthorized(string message = "Unauthorized access")
        {
            return new ResultModel(ResultCode.Unauthorized, message);
        }

        public static ResultModel Exception(string message = "An error occurred", string message1 = null)
        {
            return new ResultModel(ResultCode.ExceptionThrown, message);
        }
    }

    public enum ResultCode
    {
        Success = 200,
        SuccessfullyCreated = 201,
        SuccessfullyUpdated = 200,
        RecordNotFound = 404,
        DuplicateRecord = 409,
        Invalid = 400,
        Unauthorized = 401,
        NotAllowed = 403,
        ExceptionThrown = 500
    }
}
