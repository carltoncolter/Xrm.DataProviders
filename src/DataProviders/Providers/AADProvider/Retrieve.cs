using CustomDataProviders.Interfaces;
using Microsoft.Xrm.Sdk;

namespace CustomDataProviders.AADODataProvider
{
    public class Retrieve : RetrieveBase
    {
        public override IDataService GetDataService(IOrganizationService service, Entity dataSource)
        {
            return new DataService(service, dataSource);
        }
    }
}