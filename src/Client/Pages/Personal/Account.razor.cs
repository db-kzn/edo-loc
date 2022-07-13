using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Personal
{
    public partial class Account
    {
        public string UserId { get; set; }

        private int delay;
        private int duration;

        protected override void OnInitialized()
        {
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;
        }
    }
}
