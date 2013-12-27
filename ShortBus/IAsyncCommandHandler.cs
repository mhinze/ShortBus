namespace ShortBus
{
    using System.Threading.Tasks;

    public interface IAsyncCommandHandler<in TCommand, TResult> where TCommand : IAsyncCommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand message);
    }

    public abstract class AsyncCommandHandler<TCommand> : IAsyncCommandHandler<TCommand, UnitType>
        where TCommand : IAsyncCommand
    {
        public Task<UnitType> HandleAsync(TCommand message)
        {
            return HandleAsyncCore(message)
                .ContinueWith(_ => UnitType.Default);
        }

        protected abstract Task HandleAsyncCore(TCommand message);
    }
}