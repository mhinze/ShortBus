namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IMediator
    {
        Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query);
        Task<Response<TResponseData>> RequestAsync<TResponseData>(IAsyncQuery<TResponseData> query);

        Response Send<TMessage>(TMessage message);
        Task<Response> SendAsync<TMessage>(TMessage message);
    }
}