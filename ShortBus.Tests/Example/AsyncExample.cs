namespace ShortBus.Tests.Example
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::StructureMap;
    using NUnit.Framework;
    using StructureMap;
    using System.Reflection;

    [TestFixture]
    public class AsyncExample
    {
        [Test]
        public void Notification()
        {
            var handled = new List<int>();
            var container = new Container();
            container.Configure(i =>
            {
                i.Scan(s =>
                {
                    s.Assembly(Assembly.GetExecutingAssembly());
                    s.AddAllTypesOf(( typeof (INotificationHandler<>) ));
                });
                i.For<IList>().Use(handled);
            });

            var resolver = new StructureMapDependencyResolver(container);

            var notification = new Notification();

            var mediator = new Mediator(resolver);

            mediator.Notify(notification);

            CollectionAssert.AreEquivalent(new[] { 1, 2 }, handled);
        }

        [Test]
        public void RequestResponse()
        {
            var container = new Container();
            container.Configure(i => i.Scan(s =>
            {
                s.Assembly(Assembly.GetExecutingAssembly());
                s.AddAllTypesOf(( typeof (IAsyncRequestHandler<,>) ));
            }));

            var resolver = new StructureMapDependencyResolver(container);

            var query = new ExternalResourceQuery();

            var mediator = new Mediator(resolver);

            var task = mediator.RequestAsync(query);

            Assert.That(task.Result.Data, Is.EqualTo("success"));
            Assert.That(task.Result.HasException(), Is.False);
        }
    }

    public class ExternalResourceQuery : IAsyncRequest<string> { }

    public class ExternalResourceHandler : IAsyncRequestHandler<ExternalResourceQuery, string>
    {
        public Task<string> HandleAsync(ExternalResourceQuery request)
        {
            return Task.FromResult("success");
        }
    }

    public class Notification { }

    public class NotificationHandler1 : INotificationHandler<Notification>
    {
        readonly IList _list;

        public NotificationHandler1(IList list)
        {
            _list = list;
        }

        public void Handle(Notification notification)
        {
            _list.Add(1);
        }
    }

    public class NotificationHandler2 : INotificationHandler<Notification>
    {
        readonly IList _list;

        public NotificationHandler2(IList list)
        {
            _list = list;
        }

        public void Handle(Notification notification)
        {
            _list.Add(2);
        }
    }
}