namespace ShortBus
{
    public interface IAsyncCommand : IAsyncCommand<UnitType> { }

    public interface IAsyncCommand<TResult> { }
}