using Autofac;

namespace Webtoys.CQRS.Modules
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<MediatorModule>();
        }
    }
}
