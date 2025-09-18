using MediatR;
using FluentValidation;
using Ardalis.Result;

namespace FoodDiary.UseCases.Validation;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) 
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        if (IsResultType<TResponse>())
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
            var failureResult = CreateFailureResult<TResponse>(errorMessage);
            
            if (failureResult != null)
                return failureResult;
        }

        throw new ValidationException(failures);
    }

    private static bool IsResultType<T>() =>
        typeof(T).IsGenericType && 
        typeof(T).GetGenericTypeDefinition() == typeof(Result<>);

    private static T? CreateFailureResult<T>(string errorMessage)
    {
        if (!IsResultType<T>()) 
            return default;

        var resultType = typeof(T).GetGenericArguments()[0];
        var resultGenericType = typeof(Result<>).MakeGenericType(resultType);
        var errorMethod = resultGenericType.GetMethod("Error", new[] { typeof(string) });
        
        if (errorMethod == null) 
            return default;

        var result = errorMethod.Invoke(null, new object[] { errorMessage });
        return (T?)result;
    }
} 