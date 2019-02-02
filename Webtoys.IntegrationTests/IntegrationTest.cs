using Autofac;
using Webtoys.CQRS.Modules;
using MediatR;
using Xunit;

namespace Webtoys.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class IntegrationTest
    {
        private IMediator _mediator;

        protected readonly IContainer Container;
        protected IMediator Mediator
        {
            get
            {
                _mediator = _mediator ?? Container.Resolve<IMediator>();
                return _mediator;
            }
        }

        public IntegrationTest()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules<DefaultModule>();
            Container = builder.Build();
        }
    }
}
