using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slms2asp.Models
{
    /// <summary>
    /// 
    /// Model returned by various error responses.
    /// 
    /// </summary>
    public class ErrorModel
    {
        public int Code;
        public string Message;

        public ErrorModel(int code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// 
        /// Error model object for 400 Bad Request.
        /// 
        /// </summary>
        /// <returns>ErrorModel</returns>
        public static ErrorModel BadRequest(string reason = "bad request") =>
            new ErrorModel(400, reason);

        /// <summary>
        /// 
        /// Error model object for 404 Not Found.
        /// 
        /// </summary>
        /// <returns>ErrorModel</returns>
        public static ErrorModel NotFound() =>
            new ErrorModel(404, "not found");

        /// <summary>
        /// 
        /// Error model object for 403 Forbidden.
        /// 
        /// </summary>
        /// <returns>ErrorModel</returns>
        public static ErrorModel Forbidden(string reason = "forbidden") =>
            new ErrorModel(403, reason);

        /// <summary>
        /// 
        /// Error model object for 401 Unauthorized.
        /// 
        /// </summary>
        /// <returns>ErrorModel</returns>
        public static ErrorModel Unauthorized() =>
            new ErrorModel(401, "unauthorized");

        /// <summary>
        /// 
        /// Error mdoel object when to creating object
        /// already exists.
        /// 
        /// </summary>
        /// <returns>ErrorModel</returns>
        public static ErrorModel AlreadyExists() =>
            new ErrorModel(400, "already exists");

        /// <summary>
        /// 
        /// Error model object for 429 Too Many Requests.
        /// 
        /// </summary>
        /// <returns>ErrorModel</returns>
        public static ErrorModel RateLimited() =>
            new ErrorModel(429, "you are being rate limited");
    }
}
