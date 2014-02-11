namespace ShortBus.Tests.Example
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::StructureMap;
    using NUnit.Framework;
    using ShortBus.StructureMap;

    [TestFixture]
    public class AsyncExample
    {
        [Test]
        public void Notification()
        {
            var handled = new List<int>();

            ObjectFactory.Initialize(i =>
            {
                i.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AddAllTypesOf((typeof (INotificationHandler<>)));
                });
                i.For<IList>().Use(handled);
            });

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

            var notification = new Notification();

            var mediator = new Mediator(resolver);

            mediator.Notify(notification);

            CollectionAssert.AreEquivalent(handled, new[] {1, 2});
        }

        [Test]
        public void RequestResponse()
        {
            ObjectFactory.Initialize(i => i.Scan(s =>
            {
                s.TheCallingAssembly();
                s.AddAllTypesOf((typeof (IAsyncRequestHandler<,>)));
            }));

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

            var query = new ExternalResourceQuery();

            var mediator = new Mediator(resolver);

            var task = mediator.RequestAsync(query);

            Assert.That(task.Result.Data, Is.EqualTo("success"));
            Assert.That(task.Result.HasException(), Is.False);
        }
    }

    public class ExternalResourceQuery : IAsyncRequest<string> {}

    public class ExternalResourceHandler : IAsyncRequestHandler<ExternalResourceQuery, string>
    {
        public Task<string> HandleAsync(ExternalResourceQuery request)
        {
            return Task.FromResult("success");
        }
    }

    public class Notification {}

    public class NotificationHandler1 : INotificationHandler<Notification>
    {
        private readonly IList _list;

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
        private readonly IList _list;

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