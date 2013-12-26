namespace ShortBus.Tests.Example
{
    using System;

    public class ConsoleWriter : CommandHandler<PrintText>
    {
        protected override void HandleCore(PrintText command)
        {
            Console.WriteLine(command.Format, command.Args);
        }
    }

    public class CommandWithResultHandler : ICommandHandler<CommandWithResult, string>
    {
        public string Handle(CommandWithResult command)
        {
            return "foo";
        }
    }
}