namespace ShortBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface IMediator
    {
        TResponseData Request<TResponseData>(IRequest<TResponseData> request);
        Task<TResponseData> RequestAsync<TResponseData>(IAsyncRequest<TResponseData> query);

        void Notify<TNotification>(TNotification notification);
        Task NotifyAsync<TNotification>(TNotification notification);
    }

    public class Mediator : IMediator
    {
        readonly IDependencyResolver _dependencyResolver;

        public Mediator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public virtual TResponseData Request<TResponseData>(IRequest<TResponseData> request)
        {
            var plan = new MediatorPlan<TResponseData>(typeof (IRequestHandler<,>), "Handle", request.GetType(), _dependencyResolver);
            return plan.Invoke(request);
        }

        public Task<TResponseData> RequestAsync<TResponseData>(IAsyncRequest<TResponseData> query)
        {
            var plan = new MediatorPlan<TResponseData>(typeof (IAsyncRequestHandler<,>), "HandleAsync", query.GetType(), _dependencyResolver);
            return plan.InvokeAsync(query);
        }

        public void Notify<TNotification>(TNotification notification)
        {
            var handlers = _dependencyResolver.GetInstances<INotificationHandler<TNotification>>();

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
                throw new AggregateException(exceptions);
        }

        public Task NotifyAsync<TNotification>(TNotification notification)
        {
            var handlers = _dependencyResolver.GetInstances<IAsyncNotificationHandler<TNotification>>();

            return Task.WhenAll(handlers.Select(x => x.HandleAsync(notification)));
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