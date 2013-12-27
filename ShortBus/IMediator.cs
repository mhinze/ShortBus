namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IMediator
    {
        Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query);
        Task<Response<TResponseData>> RequestAsync<TResponseData>(IAsyncQuery<TResponseData> query);

        Response<TResponseData> Send<TResponseData>(ICommand<TResponseData> command);
        Task<Response<TResponseData>> SendAsync<TResponseData>(IAsyncCommand<TResponseData> command);
    }
}