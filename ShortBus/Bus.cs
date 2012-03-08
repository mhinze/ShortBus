using System;
using System.Collections.Generic;
using StructureMap;

namespace ShortBus
{
    public class Bus : IBus
    {
        readonly IContainer _container;

        public Bus(IContainer container)
        {
            _container = container;
        }

        public virtual Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query)
        {
            var response = new Response<TResponseData>();

            var handler = GetHandler(query);

            try
            {
                response.Data = ProcessQueryWithHandler(query, handler);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public virtual Response Send<TMessage>(TMessage message)
        {
            var allInstances = _container.GetAllInstances<ICommandHandler<TMessage>>();

            var response = new Response();
            List<Exception> exceptions = null;
            foreach (var handler in allInstances)
                try
                {
                    handler.Handle(message);
                }
                catch (Exception e)
                {
                    (exceptions ?? (exceptions = new List<Exception>())).Add(e);
                }
            if (exceptions != null)
                response.Exception = new AggregateException(exceptions);
            return response;
        }

        TResponseData ProcessQueryWithHandler<TResponseData>(IQuery<TResponseData> query, object handler)
        {
            return (TResponseData) handler.GetType().GetMethod("Handle").Invoke(handler, new object[] { query });
        }

        object GetHandler<TResponseData>(IQuery<TResponseData> query)
        {
            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = _container.GetInstance(handlerType);
            return handler;
        }
    }
}