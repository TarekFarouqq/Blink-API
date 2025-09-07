namespace Blink_API.Errors
{
    //
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A Bad Request, You Have Made",
                401 => "Authorized, You Are Not",
                404 => "Resource Was Not Found",
                500 => "Errors Are The Path To The Dark Side. Errors Lead To Anger. Anger Leads To Hate. Hate Leads To Career Change",
                _ => null
            };
        }
    }

 
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public ApiResponse(int statusCode, string? message = null, T? data = default)
            : base(statusCode, message)
        {
            Data = data;
        }
    }
}
