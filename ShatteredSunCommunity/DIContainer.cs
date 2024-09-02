using NLog;
using ShatteredSunCommunity.Components.Pages;
using ShatteredSunCommunity.Components.PageSupport;
using ShatteredSunCommunity.Conversion;
using ShatteredSunCommunity.Extensions;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LogLevel = NLog.LogLevel;

namespace ShatteredSunCommunity
{
    public static class DIContainer
    {
        private static IServiceProvider serviceProvider;
        private static IServiceCollection services;

        public static void Initialize(IServiceCollection serviceCollection)
        {
            services = serviceCollection;
            ConfigureDefaultServices(services);
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
                var attr = type.GetCustomAttribute<SingletonServiceAttribute>();
                if (attr != null)
                {
                    var descriptor = new ServiceDescriptor(type, type, attr.Scope);
                    services.Add(descriptor);
                }
            }
            // then register interfaces
            foreach (var type in serviceTypes.Where(t => t.IsInterface))
            {
                var attr = type.GetCustomAttribute<SingletonServiceAttribute>();
                // default implementation should be same as interface name
                var implementationTypes = services
                    .Select(t => t.ServiceType)
                    .Where(t => null != t.GetInterface(type.Name))
                    .ToList();
                var implementationType =
                    implementationTypes.Count == 0 ? null :
                    implementationTypes.Count == 1 ? implementationTypes[0] :
                    implementationTypes.SingleOrDefault(t => type.Name[0] == 'I' && type.Name.Substring(1) == t.Name);
                if (implementationType != null)
                {
                    var descriptor = new ServiceDescriptor(type, implementationType, attr.Scope);
                    services.Add(descriptor);
                }
            }


            services.AddSingleton(GetSanctuarySunData);
            services.AddScoped(GetUnitViewFilters);
        }

        private static UnitViewFilters GetUnitViewFilters(IServiceProvider provider)
        {
            var data = provider.GetService<SanctuarySunData>();
            var instance = new UnitViewFilters()
            {
                GroupBy =
                {
                    { "faction", data.GetDistinct("faction", f=>f.Text) },
                    { "tier", data.GetDistinct("tier", f => f.Text) },
                },
                SortFilters =
                {
                    { "faction", data.GetDistinct("faction", f=>f.Text) },
                    { "tier", data.GetDistinct("tier", f => f.Text) },
                },
            };
            return instance;
        }

        private static SanctuarySunData GetSanctuarySunData(IServiceProvider provider)
        {
            var file = "ShatteredSunUnitData.json";
            var json = File.ReadAllText(file);
            var instance = JsonSerializer.Deserialize<SanctuarySunData>(json, JsonHelper.JsonOptions);
            foreach (var unit in instance.Units)
            {
                foreach (var field in unit.Values)
                {
                    var groups = field.PathParts.ToList();
                    if (field.UnitFieldType != UnitFieldTypeEnum.StringArray)
                    {
                        while (groups.Count < JsonHelper.ExpectedMaxGroups)
                            groups.Insert(1, string.Empty);
                    }
                    field.GroupParts = groups.ToArray();
                    field.ColSpan = JsonHelper.ExpectedMaxGroups - groups.Count + 1;
                    field.IsThumbnail = field.IsThumbnail();
                }
            }
            return instance;

        }

        public static T Get<T>() where T : class
        {
            if (serviceProvider == null)
                serviceProvider = services.BuildServiceProvider();

            var instance = serviceProvider.GetService<T>();
            Debug.Assert(instance != null);
            return instance;
        }
        public static T Get<T>(params object[] parameters) where T : class
        {
            if (serviceProvider == null)
                serviceProvider = services.BuildServiceProvider();

            var instance = ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
            Debug.Assert(instance != null);
            return instance;
        }
    }
}
