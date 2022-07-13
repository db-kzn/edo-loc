using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Preferences;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EDO_FOMS.Client.Infrastructure.Settings;
using EDO_FOMS.Shared.Constants.Localization;

namespace EDO_FOMS.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args)
                          .AddRootComponents()
                          .AddClientServices();

            var host = builder.Build();

            var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();

            if (storageService != null)
            {
                var preference = await storageService.GetPreference() as ClientPreference;

                CultureInfo culture = (preference != null) ? new CultureInfo(preference.LanguageCode)
                    : new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "ru-RU");
                
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            await builder.Build().RunAsync();
        }
    }
}