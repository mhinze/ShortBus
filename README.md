## ShortBus
ShortBus is an in-process mediator with low-friction API

### Command

    public class DoSomething : ICommand { }

    public class DoesSomething : ICommandHandler<DoSomething> {
        public void Handle(DoSomething command) {
           // does something
        }
    }

    _mediator.Send(new DoSomething());



### Query
    public class AskAQuestion : IQuery<Answer> { }

    public class Answerer : IQueryHandler<AskAQuestion, Answer> {
        public Answer Handle(AskAQuestion query) {			
            return answer;
        }
    }

    var answer = _mediator.Request(new AskAQuestion());
    
### StructureMap
ShortBus depends on StructureMap and it requires that you register 
handlers:

    ObjectFactory.Initialize(i => i.Scan(s =>
    {
        s.AssemblyContainingType<IMediator>();
        s.TheCallingAssembly();
        s.WithDefaultConventions();
        s.AddAllTypesOf(typeof (IQueryHandler<,>));
        s.AddAllTypesOf(typeof (ICommandHandler<>));
    }));	

### Low-friction API
No type parameter noise.

### What for?

* Query objects
* Enables subcutaneous testing
* Business concepts as first class citizens

### In Production
ShortBus is in production powering the server APIs for major ecommerce applications.
