using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CustomDataProviders.Interfaces
{
    public interface IDataService
    {
        EntityCollection GetEntities(QueryExpression query);
        Entity GetEntity(EntityReference reference);
    }
}