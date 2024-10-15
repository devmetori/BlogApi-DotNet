using System.ComponentModel.DataAnnotations;
using System.Net;
using BlogApi.Shared.Enums;

namespace BlogApi.Shared.Exceptions;

public class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public ERROR_CODE Code { get; set; }
    public string  Message { get; set; } 
    public HttpException(HttpStatusCode statusCode, ERROR_CODE code, string message) : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Message = message;
    }

    public static void ThrowException(ERROR_CODE code,string message)
    {
        switch (code)
        {
            case ERROR_CODE.RECORD_NOT_FOUND:
                throw new HttpException(HttpStatusCode.NotFound, code, message);
            case ERROR_CODE.INVALID_CREDENTIALS:
            case ERROR_CODE.MISSING_REQUIRED_FIELD:
            case ERROR_CODE.FIELD_NOT_REQUIRED:
                throw new HttpException(HttpStatusCode.BadRequest,code, message);
            case ERROR_CODE.CONFLICT_OPERATION:
                throw new HttpException(HttpStatusCode.Conflict,code, message);
            case ERROR_CODE.NOT_ENOUGH_CREADENTIALS:
            case ERROR_CODE.TOKEN_EXPIRED:
            case ERROR_CODE.INVALID_TOKEN:
                throw new HttpException(HttpStatusCode.Unauthorized,code, message);
            case ERROR_CODE.NOT_ENOUGH_PERMISSIONS:
            case ERROR_CODE.INVALID_OPERATION:
                throw new HttpException(HttpStatusCode.Forbidden,code, message);
            default:
                throw new HttpException(HttpStatusCode.InternalServerError,code, message);
        }
    }
}