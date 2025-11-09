using MediatR;

namespace BuildingBlocks.CQRS;

public interface ICommand : ICommand<Unit>
{

}

//Abstraction for generic return type
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
