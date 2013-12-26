namespace ShortBus.Tests.Example
{
    public class PrintText : ICommand
    {
        public virtual string Format { get; set; }
        public virtual object[] Args { get; set; }
    }

    public class PrintTextSpecial : PrintText
    {
        private string _format;

        public override string Format
        {
            get { return _format + " is special"; }
            set { _format = value; }
        }
    }

    public class CommandWithResult : ICommand<string>
    {
        
    }
}