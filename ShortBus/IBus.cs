namespace ShortBus
{
    public interface IBus
    {
        Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query);
        Response Send<TMessage>(TMessage message);
    }
}