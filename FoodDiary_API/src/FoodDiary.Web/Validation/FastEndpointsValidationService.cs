using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FoodDiary.Web.Validation;

public static class FastEndpointsValidationService
{
    public static async Task<bool> ValidateRequestAsync<TRequest>(TRequest request, HttpContext httpContext)
    {
        var validator = httpContext.RequestServices.GetService<IValidator<TRequest>>();
        
        if (validator == null)
            return true;

        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            await httpContext.Response.WriteAsJsonAsync(new { errors }, httpContext.RequestAborted);
            return false;
        }

        return true;
    }
} 