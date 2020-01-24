using Microsoft.Xrm.Sdk;

namespace CustomDataProviders.AADODataProvider
{
    public class DataSource : Entity
    {
        public DataSource()
        {
        }

        public DataSource(Entity source) : base("ft_aadprovider", source.Id)
        {
            foreach (var attr in source.Attributes) this[attr.Key] = attr.Value;
        }

        public string Resource => GetAttributeValue<string>("ft_resource");
        public string Username => GetAttributeValue<string>("ft_username");
        public string Password => GetAttributeValue<string>("ft_password");
        public string ClientId => GetAttributeValue<string>("ft_clientid");
    }
}