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
        public async Task<UnitType> HandleAsync(TCommand message)
        {
            await HandleAsyncCore(message);

            return UnitType.Default;
        }

        protected abstract Task HandleAsyncCore(TCommand message);
    }
}