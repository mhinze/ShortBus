## ShortBus
ShortBus is a synchronous mediator with low-friction API

### Command
    public class DoSomething : ICommand { }

	public class DoesSomething : ICommandHandler<DoSomething> {
		public void Handle(DoSomething command) {
		   // does something
		}
	}

    _bus.Send(new DoSomething());



### Query
    public class AskAQuestion : IQuery<Answer> { }

	public class Answerer : IQueryHandler<AskAQuestion, Answer> {
	    public void Handle(AskAQuestion query) {			
			return answer;
		}
	}

	var answer = _bus.Request(new AskAQuestion());
	
### StructureMap
ShortBus depends on StructureMap and it requires that you register 
handlers:

    ObjectFactory.Initialize(i => i.Scan(s =>
    {
        s.AssemblyContainingType<IBus>();
        s.TheCallingAssembly();
        s.WithDefaultConventions();
        s.ConnectImplementationsToTypesClosing((typeof (IQueryHandler<,>)));
        s.AddAllTypesOf(typeof (ICommandHandler<>));
    }));	

### Synchronous
ShortBus is synchronous.

### Low-friction API
No type parameter noise.

### What for?

* Query objects
* Enables subcutaneous testing
* Business concepts as first class citizens

    


