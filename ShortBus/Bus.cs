using System;
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

            AssertHandlerNotNull(query, handler);

            try
            {
                response.Data = GetResponseData(query, handler);
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
            foreach (var handler in allInstances)
                try
                {
                    handler.Handle(message);
                }
                catch (Exception e)
                {
                    // TODO This should handle continuing after exception
                    response.Exception = e;
                    break;
                }

            return response;
        }

        static void AssertHandlerNotNull<TResponseData>(IQuery<TResponseData> query, object handler)
        {
            if (handler == null)
                throw new InvalidOperationException(string.Format("handler not found for message of type {0}",
                                                                  query.GetType().Name));
        }

        static TResponseData GetResponseData<TResponseData>(IQuery<TResponseData> query, object handler)
        {
            var helperType = typeof (Helper<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var helper = ((IHelper) Activator.CreateInstance(helperType));
            var responseData = (TResponseData) helper.ExecuteHandler(handler, query);
            return responseData;
        }

        object GetHandler<TResponseData>(IQuery<TResponseData> query)
        {
            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = _container.TryGetInstance(handlerType);
            return handler;
        }

        class Helper<TRequest, TResponse> : IHelper where TRequest : IQuery<TResponse>
        {
            public object ExecuteHandler(object handler, object query)
            {
                return ((IQueryHandler<TRequest, TResponse>) handler).Handle((TRequest) query);
            }
        }

        interface IHelper
        {
            object ExecuteHandler(object handler, object query);
        }
    }
}