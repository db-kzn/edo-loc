using System.Linq;
using EDO_FOMS.Shared.Constants.Localization;
using EDO_FOMS.Shared.Settings;

namespace EDO_FOMS.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "ru-RU";

        //TODO - add server preferences
    }
}