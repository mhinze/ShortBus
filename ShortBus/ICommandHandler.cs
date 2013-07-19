namespace ShortBus
{
    public interface ICommandHandler<in TMessage>
    {
        void Handle(TMessage message);
    }
}