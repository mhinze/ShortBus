namespace ShortBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using StructureMap;

    public class Mediator : IMediator
    {
        readonly IContainer _container;

        public Mediator(IContainer container)
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
                    ( exceptions ?? ( exceptions = new List<Exception>() ) ).Add(e);
                }
            if (exceptions != null)
                response.Exception = new AggregateException(exceptions);
            return response;
        }

        public async Task<Response> SendAsync<TMessage>(TMessage message)
        {
            var handlers = _container.GetAllInstances<IAsyncCommandHandler<TMessage>>();

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
            var method = (from m in handler.GetType().GetMethods()
                          let parameters = m.GetParameters()
                          let returnType = m.ReturnType
                          where m.Name == "Handle" &&
                                parameters.Length == 1 &&
                                parameters[0].ParameterType.IsInstanceOfType(query) &&
                                returnType == typeof (TResponseData)
                          select m).First();


            return (TResponseData) method.Invoke(handler, new object[] {query});
        }

        static Task<TResponseData> ProcessQueryWithHandlerAsync<TResponseData>(IAsyncQuery<TResponseData> query, object handler)
        {
            var taskReturnType = typeof (Task<>).MakeGenericType(typeof (TResponseData));

            var method = (from m in handler.GetType().GetMethods()
                          let parameters = m.GetParameters()
                          let returnType = m.ReturnType
                          where m.Name == "HandleAsync" &&
                                parameters.Length == 1 &&
                                parameters[0].ParameterType.IsInstanceOfType(query) &&
                                returnType == taskReturnType
                          select m).First();

            return (Task<TResponseData>) method.Invoke(handler, new object[] { query });
        }

        object GetHandler<TResponseData>(IQuery<TResponseData> query)
        {
            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = _container.GetInstance(handlerType);
            return handler;
        }

        object GetAsyncHandler<TResponseData>(IAsyncQuery<TResponseData> query)
        {
            var handlerType = typeof (IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = _container.GetInstance(handlerType);
            return handler;
        }
    }
}