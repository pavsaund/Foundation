using Aksio.Execution;
using Dolittle.SDK.Tenancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aksio.Resources
{
    /// <summary>
    /// Represents an implementation of <see cref="IResourceConfigurations"/>.
    /// </summary>
    [Singleton]
    public class ResourceConfigurations : IResourceConfigurations
    {
        static readonly string _resourcesJsonFilePath = Path.Join(".dolittle", "resources.json");

        readonly Dictionary<TenantId, Dictionary<Type, ResourceConfiguration>> _resourceConfigurationsByTenants = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceConfigurations"/> class.
        /// </summary>
        public ResourceConfigurations()
        {
            Populate();
        }

        /// <inheritdoc/>
        public TResource GetFor<TResource>(TenantId tenantId)
            where TResource : ResourceConfiguration
        {
            ThrowIfMissingResourceConfigurationsForTenant(tenantId);

            var resourceConfigurations = _resourceConfigurationsByTenants[tenantId];
            ThrowIfMissingResourceConfigurationOfType<TResource>(resourceConfigurations, tenantId);

            return (_resourceConfigurationsByTenants[tenantId][typeof(TResource)] as TResource)!;
        }

        /// <inheritdoc/>
        public IDictionary<TenantId, TResource> GetForAllTenants<TResource>()
            where TResource : ResourceConfiguration
        {
            return _resourceConfigurationsByTenants
                .Where(_ => _.Value.ContainsKey(typeof(TResource)))
                .ToDictionary(
                    _ => _.Key,
                    _ => (_.Value.Where(r => r.Key == typeof(TResource)).Select(r => r.Value).Single() as TResource)!);
        }

        static void ThrowIfMissingResourceConfigurationOfType<TResource>(Dictionary<Type, ResourceConfiguration> resourceConfigurations, TenantId tenantId)
            where TResource : ResourceConfiguration
        {
            if (!resourceConfigurations.ContainsKey(typeof(TResource))) throw new MissingResourceConfigurationOfType(tenantId, typeof(TResource));
        }

        void Populate()
        {
            if (File.Exists(_resourcesJsonFilePath))
            {
                var resourcesJson = File.ReadAllText(_resourcesJsonFilePath);
                var resourcesPerTenant = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, JObject>>>(resourcesJson);

                foreach ((var tenantIdString, var tenantResources) in resourcesPerTenant)
                {
                    var tenantId = (TenantId)tenantIdString;

                    var resourceConfigurations = new Dictionary<Type, ResourceConfiguration>();
                    _resourceConfigurationsByTenants[tenantId] = resourceConfigurations;

                    foreach ((var resourceType, var resourceConfiguration) in tenantResources)
                    {
                        switch (resourceType)
                        {
                            case "eventStore":
                                {
                                    resourceConfigurations[typeof(EventStoreConfiguration)] = resourceConfiguration.ToObject<EventStoreConfiguration>()!;
                                }

                                break;

                            case "readModels":
                                {
                                    resourceConfigurations[typeof(MongoDbReadModelsConfiguration)] = resourceConfiguration.ToObject<MongoDbReadModelsConfiguration>()!;
                                }

                                break;
                        }
                    }
                }
            }
        }

        void ThrowIfMissingResourceConfigurationsForTenant(TenantId tenantId)
        {
            if (!_resourceConfigurationsByTenants.ContainsKey(tenantId)) throw new MissingResourceConfigurationsForTenant(tenantId);
        }
    }
}
