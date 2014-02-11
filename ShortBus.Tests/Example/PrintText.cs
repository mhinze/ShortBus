namespace ShortBus.Tests.Example
{
    public class PrintText : IRequest<UnitType>
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

    public class CommandWithResult : IRequest<string> {}
}