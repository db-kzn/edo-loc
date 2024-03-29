﻿using Blazored.LocalStorage;
using EDO_FOMS.Client.Infrastructure.Authentication;
using EDO_FOMS.Client.Infrastructure.Managers;
using EDO_FOMS.Client.Infrastructure.Managers.Preferences;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using EDO_FOMS.Client.Infrastructure.Managers.ExtendedAttribute;
using EDO_FOMS.Domain.Entities.ExtendedAttributes;
using EDO_FOMS.Domain.Entities.Doc;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using EDO_FOMS.Client.Infrastructure.Services;

namespace EDO_FOMS.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "EdoFomsRt";

        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");

            return builder;
        }

        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .AddAuthorizationCore(options => RegisterPermissionClaims(options))
                .AddBlazoredLocalStorage()
                .AddMudServices(config =>
                {
                    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                    config.SnackbarConfiguration.PreventDuplicates = false;
                    config.SnackbarConfiguration.ShowCloseIcon = true;
                    config.SnackbarConfiguration.VisibleStateDuration = 10000;
                    config.SnackbarConfiguration.HideTransitionDuration = 100;
                    config.SnackbarConfiguration.ShowTransitionDuration = 100;
                    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                    config.SnackbarConfiguration.MaxDisplayedSnackbars = 10;
                })
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddScoped<ClientPreferenceManager>()
                .AddScoped<AuthStateProvider>()
                .AddScoped<AuthenticationStateProvider, AuthStateProvider>()
                //.AddSingleton<StateService>()
                .AddSingleton<IClientState, ClientState>()
                .AddManagers()
                .AddExtendedAttributeManagers()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            return builder;
        }

        public static IServiceCollection AddManagers(this IServiceCollection services)
        {
            var managers = typeof(IManager);

            var types = managers.Assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new { Service = t.GetInterface($"I{t.Name}"), Implementation = t })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        public static IServiceCollection AddExtendedAttributeManagers(this IServiceCollection services)
        {
            //TODO - add managers with reflection!

            return services
                .AddTransient(
                    typeof(IExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>),
                    typeof(ExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>)
                    );
        }

        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        }
    }
}