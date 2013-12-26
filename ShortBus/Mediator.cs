namespace ShortBus
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    public class Mediator : IMediator
    {
        public virtual Response<TResponseData> Request<TResponseData>(IQuery<TResponseData> query)
        {
            var response = new Response<TResponseData>();

            try
            {
                var plan = new MediatorPlan<TResponseData>(typeof (IQueryHandler<,>), "Handle", query.GetType());

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
                var plan = new MediatorPlan<TResponseData>(typeof (IAsyncQueryHandler<,>), "HandleAsync", query.GetType());

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
                var plan = new MediatorPlan<TResponseData>(typeof (ICommandHandler<,>), "Handle", command.GetType());

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
                var plan = new MediatorPlan<TResponseData>(typeof (IAsyncCommandHandler<,>), "HandleAsync", command.GetType());

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

            public MediatorPlan(Type handlerTypeTemplate, string handlerMethodName, Type messageType)
            {
                var handlerType = handlerTypeTemplate.MakeGenericType(messageType, typeof (TResult));

                HandleMethod = GetHandlerMethod(handlerType, handlerMethodName, messageType);

                HandlerInstanceBuilder = () => DependencyResolver.Current.GetInstance(handlerType);
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

            public async Task<TResult> InvokeAsync(object message)
            {
                return await (Task<TResult>) HandleMethod.Invoke(HandlerInstanceBuilder(), new[] { message });
            }
        }
    }
}