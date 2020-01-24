using System;
using CustomDataProviders.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;

namespace CustomDataProviders
{
    public class PluginBase : IPlugin
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IPluginExecutionContext PluginExecutionContext => ServiceProvider.Get<IPluginExecutionContext>();
        public IOrganizationService OrganizationService => ServiceProvider.GetOrganizationService(Guid.Empty);


        public IEntityDataSourceRetrieverService DataSourceRetriever =>
            ServiceProvider.Get<IEntityDataSourceRetrieverService>();

        public Entity XrmDataSource => DataSourceRetriever.RetrieveEntityDataSource();

        public IDataService DataService => GetDataService(OrganizationService, XrmDataSource);

        public void Execute(IServiceProvider serviceProvider)
        {
            // Set the Service Provider
            ServiceProvider = serviceProvider;
            Run();
        }

        public virtual void Run()
        {
        }

        public T Target<T>()
        {
            return PluginExecutionContext.InputParameterOrDefault<T>("Target");
        }

        public virtual IDataService GetDataService(IOrganizationService service, Entity dataSource)
        {
            return null;
        }
    }
}