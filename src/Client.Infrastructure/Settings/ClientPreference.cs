using System.Linq;
using EDO_FOMS.Shared.Constants.Localization;
using EDO_FOMS.Shared.Settings;

namespace EDO_FOMS.Client.Infrastructure.Settings
{
    public record ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; }
        public bool IsRTL { get; set; }
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; }
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "ru-RU";
    }
}