namespace ShortBus
{
    public interface IQueryHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        TResponse Handle(TRequest request);
    }
}