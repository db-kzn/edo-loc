using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteParseDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public FileParseModel Parse { get; set; } = new();

        private FileParseResult Result { get; set; } = new();

        protected override void OnInitialized()
        {
            var i = Parse.FileName ?? string.Empty;

            Result.HasFileName = !string.IsNullOrWhiteSpace(i);

            if (!Result.HasFileName) { return; }

            Result.DocTitle.HasPattern = !string.IsNullOrWhiteSpace(Parse.DocTitle);
            if (Result.DocTitle.HasPattern) { Result.DocTitle.Result = Regex.Match(i, Parse.DocTitle); }

            Result.DocNumber.HasPattern = !string.IsNullOrWhiteSpace(Parse.DocNumber);
            if (Result.DocNumber.HasPattern) { Result.DocNumber.Result = Regex.Match(i, Parse.DocNumber); }

            Result.DocDate.HasPattern = !string.IsNullOrWhiteSpace(Parse.DocDate);
            if (Result.DocDate.HasPattern) { Result.DocDate.Result = Regex.Match(i, Parse.DocDate); }

            Result.DocNotes.HasPattern = !string.IsNullOrWhiteSpace(Parse.DocNotes);
            if (Result.DocNotes.HasPattern) { Result.DocNotes.Result = Regex.Match(i, Parse.DocNotes); }

            Result.CodeMo.HasPattern = !string.IsNullOrWhiteSpace(Parse.CodeMo);
            if (Result.CodeMo.HasPattern) { Result.CodeMo.Result = Regex.Match(i, Parse.CodeMo); }

            Result.CodeSmo.HasPattern = !string.IsNullOrWhiteSpace(Parse.CodeSmo);
            if (Result.CodeSmo.HasPattern) { Result.CodeSmo.Result = Regex.Match(i, Parse.CodeSmo); }

            Result.CodeFund.HasPattern = !string.IsNullOrWhiteSpace(Parse.CodeFund);
            if (Result.CodeFund.HasPattern) { Result.CodeFund.Result = Regex.Match(i, Parse.CodeFund); }
        }
        private void Ok() => MudDialog.Close();
    }

    public class ParseResult
    {
        public bool HasPattern { get; set; } = false;
        public Match Result { get; set; }
    }

    public class FileParseResult
    {
        public bool HasFileName { get; set; } = false;
        public bool HasFileMask { get; set; } = false;

        public ParseResult DocTitle { get; set; } = new();
        public ParseResult DocNumber { get; set; } = new();
        public ParseResult DocDate { get; set; } = new();
        public ParseResult DocNotes { get; set; } = new();

        public ParseResult CodeMo { get; set; } = new();
        public ParseResult CodeSmo { get; set; } = new();
        public ParseResult CodeFund { get; set; } = new();
    }
}
