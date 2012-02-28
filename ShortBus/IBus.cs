namespace ShortBus
{
    public interface IBus
    {
        Response<TResponseData> Request<TResponseData>(IRequest<TResponseData> request);
        Response Send<TMessage>(TMessage message);
    }
}