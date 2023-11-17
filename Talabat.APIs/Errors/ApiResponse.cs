namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int? StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statusCode , string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
        }

        private string? GetDefaultMessageForStatusCode(int? statusCode)
        {
            // 500 => internal server error
            //400 => bad request
            //401 => unauthorized
            //404 => not found

            // C# 7 new swich case
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "You Are not Authorized",
                404 => "Resource not found",
                500 => "Internal Server Error",
                _ => null
            };



        }


    }
}
