namespace SRP.Model.Helper.Base
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
        public string? Path { get; set; }

        public ErrorResponse()
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ErrorResponse(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
            Errors = new Dictionary<string, string[]>();
        }

        public ErrorResponse(string message, int statusCode, IDictionary<string, string[]> errors)
        {
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
