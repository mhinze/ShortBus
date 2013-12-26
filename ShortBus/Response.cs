namespace ShortBus
{
    using System;

    public sealed class Response : Response<UnitType> { }

    public class Response<TResponseData>
    {
        public virtual TResponseData Data { get; set; }

        public virtual Exception Exception { get; set; }

        public virtual bool HasException()
        {
            return Exception != null;
        }
    }
}