using MediatR;

namespace BuildingBlocks.CQRS;

public interface IQuery<out TResponce>:IRequest<TResponce>
    where TResponce : notnull
{

}
