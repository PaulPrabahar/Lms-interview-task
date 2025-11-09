using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviours;

public class ValidationBehaviour<TRequest, TResponce>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponce>
        where TRequest : ICommand<TResponce>

{
    public async Task<TResponce> Handle(TRequest request, RequestHandlerDelegate<TResponce> next, CancellationToken cancellationToken)
    {
        var Context = new ValidationContext<TRequest>(request);

        var ValidationResult = await Task.WhenAll(validators.Select(v => v.ValidateAsync(Context, cancellationToken)));

        var failuers = ValidationResult.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

        if (failuers.Any()) 
        {
            throw new ValidationException(failuers);
        }

        return await next();
    }
}
