namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IAsyncCommandHandler<in TMessage>
    {
        Task HandleAsync(TMessage message);
    }
}