namespace ShortBus
{
    public interface IHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IHandler<TMessage>
    {
        void Handle(TMessage message);
    }
}