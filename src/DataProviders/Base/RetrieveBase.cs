using System;
using CustomDataProviders.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;

namespace CustomDataProviders
{
    public abstract class RetrieveBase : PluginBase
    {
        public override void Run()
        {
            var entity = DataService.GetEntity(Target<EntityReference>());

            PluginExecutionContext.OutputParameters["BusinessEntity"] = entity;
        }

        public override abstract IDataService GetDataService(IOrganizationService service, Entity dataSource);
    }
}