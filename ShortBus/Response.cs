using System;

namespace ShortBus
{
    public class Response
    {
        public virtual Exception Exception { get; set; }

        public virtual bool HasException()
        {
            return Exception != null;
        }
    }

    public class Response<TResponseData> : Response
    {
        public virtual TResponseData Data { get; set; }
    }
}