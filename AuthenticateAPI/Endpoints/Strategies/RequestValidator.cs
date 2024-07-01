using FluentValidation;

namespace AuthenticateAPI.Endpoints.Strategies;

public static class RequestValidator
{
    public static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(request);
        return !result.IsValid ? Results.ValidationProblem(result.ToDictionary()) : null;
    }
}