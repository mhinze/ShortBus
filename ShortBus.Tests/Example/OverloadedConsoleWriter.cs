using System;

namespace ShortBus.Tests.Example
{
    public class ConsoleWriter : ICommandHandler<PrintText>
    {
        public void Handle(PrintText message)
        {
            Console.WriteLine(message.Format, message.Args);
        }
    }
}