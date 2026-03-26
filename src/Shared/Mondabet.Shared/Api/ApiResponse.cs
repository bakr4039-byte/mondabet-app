namespace Mondabet.Shared.Api;

public record ApiResponse<T>(T? Data, ErrorResponse? Error, string TraceId);

public record ErrorResponse(string Code, string Message);
