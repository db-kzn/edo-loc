using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Client.Pages.Progress
{
    public partial class AgreementRejectDialog
    {
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        Question question = new();

        private void Ok() => MudDialog.Close(DialogResult.Ok(question.answer));
    }

    public class Question
    {
        [Required]
        public string answer = "";
    }
}
