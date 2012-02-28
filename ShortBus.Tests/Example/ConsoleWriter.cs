using System;

namespace ShortBus.Tests.Example
{
    public class ConsoleWriter : IHandler<TextMessage>
    {
        public void Handle(TextMessage message)
        {
            Console.WriteLine(message.Format, message.Args);
        }
    }
}