using NLog;
using ShatteredSunCommunity.Extensions;
using System.Diagnostics;
using System.Reflection;
using LogLevel = NLog.LogLevel;

namespace ShatteredSunCommunity
{
    public static class DIContainer
    {
        private static IServiceProvider serviceProvider;
        private static ServiceCollection services;

        static DIContainer()
        {
            services = new ServiceCollection();
            serviceProvider = services.BuildServiceProvider();
        }

        public static void Initialize(Action<ServiceCollection> configureServices)
        {

            ConfigureDefaultServices(services);
            configureServices(services);
            serviceProvider = services.BuildServiceProvider();
            var detailLog = Path.Combine(Assembly.GetExecutingAssembly().Location, "detailLog.txt");
            if (File.Exists(detailLog))
            {
                File.Delete(detailLog);
            }
            LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: detailLog);
            });

        }

        private static void ConfigureDefaultServices(IServiceCollection services)
        {
            var assemblies = new List<Assembly>
            {
                Assembly.GetExecutingAssembly()
            };
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                assemblies.Add(entryAssembly);

            var serviceTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsInterface || !t.IsAbstract)
                .Where(t => t.IsPublic)
                .Where(t => !t.IsEnum)
                .Where(t => !t.IsAssignableTo(typeof(Attribute)))
                .ToList();
            var debugTypes = new[] { "SteamInfo", "ISteamInfo", "GameMetadata", "IGameMetadata" };
            // first register non-interface types
            foreach (var type in serviceTypes.Where(t => !t.IsInterface))
            {
                var attr = type.GetCustomAttribute<SingletonServiceAttribute>() ?? ServiceScopeAttribute.Default;
                var descriptor = new ServiceDescriptor(type, type, attr.Scope);
                services.Add(descriptor);
            }
            // then register interfaces
            foreach (var type in serviceTypes.Where(t => t.IsInterface))
            {
                var attr = type.GetCustomAttribute<SingletonServiceAttribute>() ?? ServiceScopeAttribute.Default;
                // default implementation should be same as interface name
                var implementationTypes = services
                    .Select(t => t.ServiceType)
                    .Where(t => null != t.GetInterface(type.Name))
                    .ToList();
                var implementationType =
                    implementationTypes.Count == 0 ? null :
                    implementationTypes.Count == 1 ? implementationTypes[0] :
                    implementationTypes.SingleOrDefault(t => type.Name[0] == 'I' && type.Name.Substring(1) == t.Name);
                if (implementationType == null)
                    throw new InvalidOperationException($"Could not find any registered implementations for interface {type.Name}");
                var descriptor = new ServiceDescriptor(type, implementationType, attr.Scope);
                services.Add(descriptor);
            }
        }

        public static T Get<T>() where T : class
        {
            var instance = serviceProvider.GetService<T>();
            Debug.Assert(instance != null);
            return instance;
        }
        public static T Get<T>(params object[] parameters) where T : class
        {
            var instance = ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
            Debug.Assert(instance != null);
            return instance;
        }
    }
}
