using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap;

namespace ShortBus
{
    public class Bus : IBus
    {
        static string handleMethod;
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

        static void AssertHandlerNotNull<TResponseData>(IQuery<TResponseData> query, object handler)
        {
            if (handler != null) return;
            var message = string.Format("handler not found for message of type {0}", query.GetType().Name);
            throw new InvalidOperationException(message);
        }

        TResponseData ProcessQueryWithHandler<TResponseData>(IQuery<TResponseData> query, object handler)
        {
            if (handleMethod == null)
            {
                // this is plain strong-typed reflection, just need the name in a rename-friendly way
                Expression<Action<IQueryHandler<IQuery<TResponseData>, TResponseData>>> method = x => x.Handle(null);
                handleMethod = ((MethodCallExpression) method.Body).Method.Name;
            }

            var methodCallExpression = Expression.Call(Expression.Constant(handler), handleMethod, null,
                                                       new Expression[] { Expression.Constant(query) });
            var function = Expression.Lambda<Func<TResponseData>>(methodCallExpression).Compile();
            var result = function();
            return result;
        }

        object GetHandler<TResponseData>(IQuery<TResponseData> query)
        {
            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResponseData));
            var handler = _container.TryGetInstance(handlerType);
            return handler;
        }
    }
}