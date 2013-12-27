namespace ShortBus
{
    public interface ICommandHandler<in TCommand, out TResult>
    {
        TResult Handle(TCommand command);
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, UnitType> { }

    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        public UnitType Handle(TCommand command)
        {
            HandleCore(command);
            return UnitType.Default;
        }

        protected abstract void HandleCore(TCommand command);
    }
}