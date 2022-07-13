namespace EDO_FOMS.Client.Pages.Main
{
    public partial class Home
    {
        private void ShowChangeLogs() => _ = _dialogService.Show<ChangeLogs>();
    }
}
