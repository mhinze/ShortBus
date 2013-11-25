using System;

namespace ShortBus.Tests.Example
{
    public class OverloadedConsoleWriter : ICommandHandler<PrintTextA>, ICommandHandler<PrintTextB>
    {
        public void Handle(PrintTextA message)
        {
            Console.WriteLine(message.Format, message.Args);
        }

        public void Handle(PrintTextB message)
        {
            Console.WriteLine(message.Format, message.Args);
        }
    }
}