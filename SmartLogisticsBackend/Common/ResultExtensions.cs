namespace SmartLogisticsBackend.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess) return Results.Ok(result.Value);
        return MapError(result);
    }

  
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess) return Results.NoContent();
        return MapError(result);
    }

    private static IResult MapError(Result result) =>
        result.ErrorType switch
        {
            ResultErrorType.Conflict  => Results.Conflict(new { error = result.Error }),
            ResultErrorType.NotFound  => Results.NotFound(new { error = result.Error }),
            ResultErrorType.Forbidden => Results.Forbid(),
            ResultErrorType.Invalid   => Results.UnprocessableEntity(new { error = result.Error }),
            _                         => Results.Problem(result.Error)
        };
}