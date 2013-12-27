namespace ShortBus
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    public class Mediator : IMediator
    {
        private readonly IDependencyResolver _dependencyResolver;

        public Mediator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public virtual Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof(IQueryHandler<,>), "Handle", query.GetType(), _dependencyResolver);

                response.Data = plan.Invoke(query);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public async Task<Response<TResponseData>> RequestAsync<TResponseData>(IAsyncQuery<TResponseData> query)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof(IAsyncQueryHandler<,>), "HandleAsync", query.GetType(), _dependencyResolver);

                response.Data = await plan.InvokeAsync(query);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public Response<TResponseData> Send<TResponseData>(ICommand<TResponseData> command)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof(ICommandHandler<,>), "Handle", command.GetType(), _dependencyResolver);

                response.Data = plan.Invoke(command);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public async Task<Response<TResponseData>> SendAsync<TResponseData>(IAsyncCommand<TResponseData> command)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof(IAsyncCommandHandler<,>), "HandleAsync", command.GetType(), _dependencyResolver);

                response.Data = await plan.InvokeAsync(command);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        class MediatorPlan<TResult>
        {
            readonly MethodInfo HandleMethod;
            readonly Func<object> HandlerInstanceBuilder;

            public MediatorPlan(Type handlerTypeTemplate, string handlerMethodName, Type messageType, IDependencyResolver dependencyResolver)
            {
                var handlerType = handlerTypeTemplate.MakeGenericType(messageType, typeof(TResult));

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
                return (TResult)HandleMethod.Invoke(HandlerInstanceBuilder(), new[] { message });
            }

            public async Task<TResult> InvokeAsync(object message)
            {
                return await (Task<TResult>)HandleMethod.Invoke(HandlerInstanceBuilder(), new[] { message });
            }
        }
    }
}