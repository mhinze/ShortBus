namespace ShortBus.Tests.Example
{
    using System;
    using System.Threading.Tasks;
    using global::StructureMap;
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
                s.TheCallingAssembly();
                s.AddAllTypesOf((typeof(IAsyncQueryHandler<,>)));
            }));

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

            var query = new ExternalResourceQuery();

            var mediator = new Mediator(resolver);

            var task = mediator.RequestAsync(query);

            Assert.That(task.Result.Data, Is.EqualTo("success"));
            Assert.That(task.Result.HasException(), Is.False);
        }

        [Test]
        public void SendResult()
        {
            ObjectFactory.Initialize(i => i.Scan(s =>
            {
                s.TheCallingAssembly();
                s.AddAllTypesOf((typeof(IAsyncCommandHandler<,>)));
            }));

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

            var query = new AddResource();

            var mediator = new Mediator(resolver);

            var result = mediator.SendAsync(query).Result;

            Assert.That(result.Data, Is.EqualTo(AddResourceHandler.Result));
        }
    }

    public class ExternalResourceQuery : IAsyncQuery<string> { }

    public class ExternalResourceHandler : IAsyncQueryHandler<ExternalResourceQuery, string>
    {
        public Task<string> HandleAsync(ExternalResourceQuery request)
        {
            return Task.FromResult("success");
        }
    }

    public class AddResource : IAsyncCommand<Guid> { }

    public class AddResourceHandler : IAsyncCommandHandler<AddResource, Guid>
    {
        public static Guid Result = new Guid("D5361D4E-26F2-4E16-932B-930243CBC830");

        public Task<Guid> HandleAsync(AddResource message)
        {
            return Task.FromResult(Result);
        }
    }
}