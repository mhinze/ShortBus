namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IRequestHandler<in TRequest, out TResponse> where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IAsyncRequestHandler<in TRequest, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }

    public interface INotificationHandler<in TNotification>
    {
        void Handle(TNotification notification);
    }

    public interface IAsyncNotificationHandler<in TNotification>
    {
        Task HandleAsync(TNotification notification);
    }
}