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

        public virtual Response<TResponseData> Request<TResponseData>(IRequest<TResponseData> request)
        {
            var response = new Response<TResponseData>();

            var handler = GetHandler(request);

            AssertHandlerNotNull(request, handler);

            try
            {
                response.Data = GetResponseData(request, handler);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public virtual Response Send<TMessage>(TMessage message)
        {
            var allInstances = _container.GetAllInstances<IHandler<TMessage>>();

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

        static void AssertHandlerNotNull<TResponseData>(IRequest<TResponseData> request, object handler)
        {
            if (handler == null)
                throw new InvalidOperationException(string.Format("handler not found for message of type {0}",
                                                                  request.GetType().Name));
        }

        static TResponseData GetResponseData<TResponseData>(IRequest<TResponseData> request, object handler)
        {
            var helperType = typeof (Helper<,>).MakeGenericType(request.GetType(), typeof (TResponseData));
            var helper = ((IHelper) Activator.CreateInstance(helperType));
            var responseData = (TResponseData) helper.ExecuteHandler(handler, request);
            return responseData;
        }

        object GetHandler<TResponseData>(IRequest<TResponseData> request)
        {
            var handlerType = typeof (IHandler<,>).MakeGenericType(request.GetType(), typeof (TResponseData));
            var handler = _container.TryGetInstance(handlerType);
            return handler;
        }

        class Helper<TRequest, TResponse> : IHelper where TRequest : IRequest<TResponse>
        {
            public object ExecuteHandler(object handler, object query)
            {
                return ((IHandler<TRequest, TResponse>) handler).Handle((TRequest) query);
            }
        }

        interface IHelper
        {
            object ExecuteHandler(object handler, object query);
        }
    }
}