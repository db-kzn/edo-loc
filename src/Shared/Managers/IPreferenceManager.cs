using EDO_FOMS.Shared.Settings;
using System.Threading.Tasks;
using EDO_FOMS.Shared.Wrapper;

namespace EDO_FOMS.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}