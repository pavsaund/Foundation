using Aksio.Execution;
using Aksio.Reflection;
using Autofac;

namespace Aksio.DependencyInversion
{
    /// <summary>
    /// Represents a <see cref="Module">autofac module</see> for default convention (IFoo -> Foo).
    /// </summary>
    public class DefaultConventionModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            var conventionBasedTypes = ContainerBuilderExtensions.Types!.All.Where(_ =>
            {
                var interfaces = _.GetInterfaces();
                if (interfaces.Length > 0) return interfaces.Any(i => i.Namespace == _.Namespace && i.Name == $"I{_.Name}");
                return false;
            });

            foreach (var conventionBasedType in conventionBasedTypes)
            {
                var interfaceToBind = conventionBasedType.GetInterfaces().Single(_ => _.Name == $"I{conventionBasedType.Name}");
                if (interfaceToBind.IsGenericType)
                {
                    var result = builder.RegisterGeneric(conventionBasedType).As(interfaceToBind);
                    if (conventionBasedType.HasAttribute<SingletonAttribute>()) result.SingleInstance();
                }
                else
                {
                    var result = builder.RegisterType(conventionBasedType).As(interfaceToBind);
                    if (conventionBasedType.HasAttribute<SingletonAttribute>()) result.SingleInstance();
                }
            }
        }
    }
}
