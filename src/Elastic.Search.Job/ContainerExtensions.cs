using Autofac;
using Elastic.Search.Job.Infrastructure;

namespace Elastic.Search.Job
{
    public static class ContainerExtensions
    {
        public static ILifetimeScope BeginLifetimeScopeForClient(this IContainer container, string clientId)
        {
            ILifetimeScope scope = container.BeginLifetimeScope();
            
            ConnectionProvider connectionProvider = scope.Resolve<ConnectionProvider>();
            connectionProvider.SetClientId(clientId);
            
            return scope;
        }
    }
}
