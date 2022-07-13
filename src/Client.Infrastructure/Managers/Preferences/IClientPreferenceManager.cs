using EDO_FOMS.Shared.Managers;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();
    }
}