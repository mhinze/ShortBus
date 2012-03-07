namespace ShortBus
{
    public interface ICommandHandler<TMessage>
    {
        void Handle(TMessage message);
    }
}