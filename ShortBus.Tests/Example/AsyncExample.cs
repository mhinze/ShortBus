namespace ShortBus.Tests.Example
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using StructureMap;

    [TestFixture]
    public class AsyncExample
    {
        [Test]
        public void RequestResponse()
        {
            ObjectFactory.Initialize(i => i.Scan(s =>
            {
                s.AssemblyContainingType<IMediator>();
                s.TheCallingAssembly();
                s.WithDefaultConventions();
                s.AddAllTypesOf(( typeof (IAsyncQueryHandler<,>) ));
            }));

            var query = new ExternalResourceQuery();

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var task = mediator.RequestAsync(query);

            Assert.That(task.Result.Data, Is.EqualTo("success"));
            Assert.That(task.Result.HasException(), Is.False);
        }
    }

    public class ExternalResourceQuery : IAsyncQuery<string> {}

    public class ExternalResourceHandler : IAsyncQueryHandler<ExternalResourceQuery, string>
    {
        public Task<string> HandleAsync(ExternalResourceQuery request)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                return "success";
            });
        }
    }
}