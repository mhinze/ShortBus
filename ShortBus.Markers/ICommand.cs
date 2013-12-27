namespace ShortBus
{
    public interface ICommand : ICommand<UnitType> { }

    public interface ICommand<TResult> { }
}