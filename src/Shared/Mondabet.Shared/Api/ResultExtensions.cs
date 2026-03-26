using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Mondabet.Shared.Domain;

namespace Mondabet.Shared.Api;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result, HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        return result.Match(
            onSuccess: value => Results.Ok(new ApiResponse<T>(value, null, traceId)),
            onFailure: error => error.Code switch
            {
                "NotFound" => Results.NotFound(
                    new ApiResponse<T>(default, new ErrorResponse(error.Code, error.Message), traceId)),
                "Validation" => Results.BadRequest(
                    new ApiResponse<T>(default, new ErrorResponse(error.Code, error.Message), traceId)),
                "Unauthorized" => Results.Json(
                    new ApiResponse<T>(default, new ErrorResponse(error.Code, error.Message), traceId),
                    statusCode: 401),
                "Forbidden" => Results.Json(
                    new ApiResponse<T>(default, new ErrorResponse(error.Code, error.Message), traceId),
                    statusCode: 403),
                "Conflict" => Results.Conflict(
                    new ApiResponse<T>(default, new ErrorResponse(error.Code, error.Message), traceId)),
                _ => Results.Json(
                    new ApiResponse<T>(default, new ErrorResponse("InternalError", "An unexpected error occurred"), traceId),
                    statusCode: 500)
            });
    }
}
