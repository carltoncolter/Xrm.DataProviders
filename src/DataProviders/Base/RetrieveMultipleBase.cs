using System;
using CustomDataProviders.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace CustomDataProviders
{
    public abstract class RetrieveMultipleBase : PluginBase
    {
        public override void Run()
        {
            var query = PluginExecutionContext.InputParameterOrDefault<QueryExpression>("Query");
            var entities = DataService.GetEntities(query);

            PluginExecutionContext.OutputParameters["BusinessEntityCollection"] = entities;
        }

        public override abstract IDataService GetDataService(IOrganizationService service, Entity dataSource);
    }
}