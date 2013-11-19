namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IAsyncQueryHandler<in TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }
}