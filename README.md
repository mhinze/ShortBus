## ShortBus
ShortBus is an in-process mediator with low-friction API

### Requests
    public class DoSomething : IRequest<UnitType> { }

	public class DoesSomething : IRequestHandler<DoSomething, UnitType> {
		public UnitType Handle(DoSomething request) {
		   // does something
		   return UnitType.Default; // Similar to void
		}
	}

    _mediator.Send(new DoSomething());

### Notifications
    public class NotifySomething { }

    public class NotificationHandler : INotificationHandler<NotifySomething> {
        public void Handle(NotifySomething notification) {
            // does something
        }
    }

    _mediator.Notify(new NotifySomething());

### IOC Containers

ShortBus currently supports 6 IOC containers

* AutoFac
* Ninject
* Simple Injector
* Structure Map
* Unity
* Windsor

Example configuration of registering handlers using StructureMap:

    var container = new Container();
    container.Configure(i => i.Scan(s =>
    {
        s.AssemblyContainingType<IMediator>();
        s.Assembly(Assembly.GetExecutingAssembly());
        s.WithDefaultConventions();
        s.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
        s.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
    }));

### Low-friction API
No type parameter noise.

### What for?

* Query objects
* Enables subcutaneous testing
* Business concepts as first class citizens

### In Production
ShortBus is in production powering the server APIs for major ecommerce applications.
