namespace ShortBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface IMediator
    {
        Response<TResponseData> Request<TResponseData>(IRequest<TResponseData> request);
        Task<Response<TResponseData>> RequestAsync<TResponseData>(IAsyncRequest<TResponseData> query);

        Response Notify<TNotification>(TNotification notification);
        Task<Response> NotifyAsync<TNotification>(TNotification notification);
    }

    public class Mediator : IMediator
    {
        readonly IDependencyResolver _dependencyResolver;

        public Mediator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public virtual Response<TResponseData> Request<TResponseData>(IRequest<TResponseData> request)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof (IRequestHandler<,>), "Handle", request.GetType(), _dependencyResolver);

                response.Data = plan.Invoke(request);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public async Task<Response<TResponseData>> RequestAsync<TResponseData>(IAsyncRequest<TResponseData> query)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof (IAsyncRequestHandler<,>), "HandleAsync", query.GetType(), _dependencyResolver);

                response.Data = await plan.InvokeAsync(query).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public Response Notify<TNotification>(TNotification notification)
        {
            var handlers = _dependencyResolver.GetInstances<INotificationHandler<TNotification>>();

            var response = new Response();
            List<Exception> exceptions = null;

            foreach (var handler in handlers)
                try
                {
                    handler.Handle(notification);
                }
                catch (Exception e)
                {
                    ( exceptions ?? ( exceptions = new List<Exception>() ) ).Add(e);
                }
            if (exceptions != null)
                response.Exception = new AggregateException(exceptions);
            return response;
        }

        public Task<Response> NotifyAsync<TNotification>(TNotification notification)
        {
            var handlers = _dependencyResolver.GetInstances<IAsyncNotificationHandler<TNotification>>();

            return Task
                .WhenAll(handlers.Select(x => notifyAsync(x, notification)))
                .ContinueWith(task =>
                {
                    var exceptions = task.Result.Where(exception => exception != null).ToArray();
                    var response = new Response();

                    if (exceptions.Any())
                    {
                        response.Exception = new AggregateException(exceptions);
                    }

                    return response;
                });
        }

        static async Task<Exception> notifyAsync<TNotification>(IAsyncNotificationHandler<TNotification> asyncCommandHandler, TNotification message)
        {
            try
            {
                await asyncCommandHandler.HandleAsync(message).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        class MediatorPlan<TResult>
        {
            readonly MethodInfo HandleMethod;
            readonly Func<object> HandlerInstanceBuilder;

            public MediatorPlan(Type handlerTypeTemplate, string handlerMethodName, Type messageType, IDependencyResolver dependencyResolver)
            {
                var handlerType = handlerTypeTemplate.MakeGenericType(messageType, typeof (TResult));

                HandleMethod = GetHandlerMethod(handlerType, handlerMethodName, messageType);

                HandlerInstanceBuilder = () => dependencyResolver.GetInstance(handlerType);
            }

            MethodInfo GetHandlerMethod(Type handlerType, string handlerMethodName, Type messageType)
            {
                return handlerType
                    .GetMethod(handlerMethodName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, CallingConventions.HasThis,
                        new[] { messageType },
                        null);
            }

            public TResult Invoke(object message)
            {
                return (TResult) HandleMethod.Invoke(HandlerInstanceBuilder(), new[] { message });
            }

            public Task<TResult> InvokeAsync(object message)
            {
                return (Task<TResult>) HandleMethod.Invoke(HandlerInstanceBuilder(), new[] { message });
            }
        }
    }
}