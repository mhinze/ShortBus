namespace ShortBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class Mediator : IMediator
    {
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

        public async Task<Response<TResponseData>> RequestAsync<TResponseData>(IAsyncQuery<TResponseData> query)
        {
            var response = new Response<TResponseData>();

            var handler = GetAsyncHandler(query);

            try
            {
                response.Data = await ProcessQueryWithHandlerAsync(query, handler);
            }
            catch (Exception e)
            {
                response.Exception = e;
            }

            return response;
        }

        public virtual Response Send<TMessage>(TMessage message)
        {
            var allInstances = DependencyResolver.Current.GetInstances<ICommandHandler<TMessage>>();

            var response = new Response();
            List<Exception> exceptions = null;
            foreach (var handler in allInstances)
                try
                {
                    handler.Handle(message);
                }
                catch (Exception e)
                {
                    ( exceptions ?? ( exceptions = new List<Exception>() ) ).Add(e);
                }
            if (exceptions != null)
                response.Exception = new AggregateException(exceptions);
            return response;
        }

        public async Task<Response> SendAsync<TMessage>(TMessage message)
        {
            var handlers = DependencyResolver.Current.GetInstances<IAsyncCommandHandler<TMessage>>();

            return await Task
                .WhenAll(handlers.Select(x => sendAsync(x, message)))
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

        static async Task<Exception> sendAsync<TMessage>(IAsyncCommandHandler<TMessage> asyncCommandHandler, TMessage message)
        {
            try
            {
                await asyncCommandHandler.HandleAsync(message);
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        static TResponseData ProcessQueryWithHandler<TResponseData>(IQuery<TResponseData> query, object handler)
        {
            return (TResponseData) GetHandlerMethod(handler, query, "Handle").Invoke(handler, new object[] { query });
        }

        static Task<TResponseData> ProcessQueryWithHandlerAsync<TResponseData>(IAsyncQuery<TResponseData> query, object handler)
        {
            return (Task<TResponseData>) GetHandlerMethod(handler, query, "HandleAsync").Invoke(handler, new object[] { query });
        }

        static MethodInfo GetHandlerMethod(object handler, object query, string name)
        {
            return handler.GetType()
                .GetMethod(name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, CallingConventions.HasThis,
                    new[] { query.GetType() },
                    null);
        }

        object GetHandler<TResponseData>(IQuery<TResponseData> query)
        {
            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = DependencyResolver.Current.GetInstance(handlerType);
            return handler;
        }

        object GetAsyncHandler<TResponseData>(IAsyncQuery<TResponseData> query)
        {
            var handlerType = typeof (IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = DependencyResolver.Current.GetInstance(handlerType);
            return handler;
        }
    }
}