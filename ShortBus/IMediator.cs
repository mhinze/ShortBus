namespace ShortBus
{
    public interface IMediator
    {
        Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query);
        Response Send<TMessage>(TMessage message);
    }
}