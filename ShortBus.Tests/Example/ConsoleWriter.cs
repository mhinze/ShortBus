namespace ShortBus.Tests.Example
{
    using System;

    public class ConsoleWriter : IRequestHandler<PrintText, UnitType>
    {
        public UnitType Handle(PrintText request)
        {
            Console.WriteLine(request.Format, request.Args);

            return UnitType.Default;
        }
    }

    public class CommandWithResultHandler : IRequestHandler<CommandWithResult, string>
    {
        public string Handle(CommandWithResult command)
        {
            return "foo";
        }
    }
}