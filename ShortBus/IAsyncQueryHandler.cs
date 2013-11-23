namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IAsyncQueryHandler<in TRequest, TResponse> where TRequest : IAsyncQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}