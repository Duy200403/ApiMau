
using ApiWebsite.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace ApiWebsite.Core.Base
{
    public static class ServiceRegistrar
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ServiceFactory>(s => s.GetRequiredService);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            foreach (var type in AssemblyHelper.ExportTypes)
            {
                if (!type.Name.EndsWith("Repository") && !type.Name.EndsWith("Service")) continue;
                
                if (typeof(IBaseService).IsAssignableFrom(type) || typeof(IGenericRepository).IsAssignableFrom(type))
                {
                    foreach (var serviceType in type.GetInterfaces().Where(t => !t.Name.StartsWith("System")))
                    {
                        services.TryAddScoped(serviceType, type);
                    }
                }
            }

            return services;
        }
    }
}