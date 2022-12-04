using System;

namespace DealerManagementSystem.Models
{
    public static class Responses
    {
        public static Response Saved()
        {
            return new Response
            {
                StatusCode = 201,
                Success = true,
                Message = "Succefully saved"
            };
        }
        public static Response Updated()
        {
            return new Response
            {
                Success = true,
                Message = "Succefully updated"
            };
        }
        public static Response Deleted()
        {
            return new Response
            {
                Success = true,
                Message = "Succefully deleted"
            };
        }
        public static Response InternalSetrverError()
        {
            return new Response
            {
                Success = false,
                Message = "Internal server error.Please try again."
            };
        }
        public static Response Exception(Exception ex)
        {
            return new Response
            {
                Success = false,
                Message = ex.Message
            };
        }
        public static Response Error(string msg)
        {
            return new Response
            {
                Success = false,
                Message = msg
            };
        }
        public static Response Success(string msg)
        {
            return new Response
            {
                Success = true,
                Message = msg
            };
        }
        public static Response Success(dynamic obj)
        {
            return new Response
            {
                StatusCode = 200,
                Success = true
            };
        }

        public static Response BadRequest()
        {
            return new Response
            {
                Success = false,
                Message = "Bad Request",
                StatusCode = 400
            };
        }
        public static Response BadRequest(string msg)
        {
            return new Response
            {
                Success = false,
                Message = msg,
                StatusCode = 400
            };
        }
        public static Response Unauthorized()
        {
            return new Response
            {
                Success = false,
                Message = "Invalid user name or password",
                StatusCode = 401
            };
        }
        public static Response Unauthorized(string msg)
        {
            return new Response
            {
                Success = false,
                Message = msg,
                StatusCode = 401
            };
        }
        public static Response Ok()
        {
            return new Response
            {
                Success = true,
                Message = "Ok",
                StatusCode = 200
            };
        }
        public static Response Sessionout()
        {
            return new Response
            {
                Success = false,
                Message = "Your session has expired.",
                StatusCode = 408
            };
        }
    }
}
