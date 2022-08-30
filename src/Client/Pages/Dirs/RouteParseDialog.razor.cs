using EDO_FOMS.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Text.RegularExpressions;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteParseDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public FileParseModel Pattern { get; set; } = new();

        private FileParseResult Result { get; set; } = new();

        private string fileName = string.Empty;
        private readonly int timeOutSec = 1;

        protected override void OnInitialized()
        {
            fileName = Pattern.FileName ?? string.Empty;

            Result.HasFileName = !string.IsNullOrWhiteSpace(fileName);

            if (!Result.HasFileName) { return; }

            Result.HasFileMask = !string.IsNullOrWhiteSpace(Pattern.FileMask);

            if (Result.HasFileMask) {
                try
                {
                    Regex mask = new(Pattern.FileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
                    Result.FileMaskIsCorrect = mask.IsMatch(fileName);
                }
                catch (Exception)
                {
                    Result.FileMaskIsCorrect = false;
                }
            }

            Result.DocTitle = TryParse(Pattern.DocTitle);
            Result.DocNumber = TryParse(Pattern.DocNumber);
            Result.DocDate = TryParse(Pattern.DocDate);
            Result.DocNotes = TryParse(Pattern.DocNotes);

            Result.CodeMo = TryParse(Pattern.CodeMo);
            Result.CodeSmo = TryParse(Pattern.CodeSmo);
            Result.CodeFund = TryParse(Pattern.CodeFund);
        }

        private void Ok() => MudDialog.Close();

        private ParseResult TryParse(string pattern)
        {
            ParseResult result = new() { HasPattern = !string.IsNullOrWhiteSpace(pattern) };

            if (!result.HasPattern) { return result; }

            try
            {
                result.Match = Regex.Match(fileName, pattern, RegexOptions.None, TimeSpan.FromSeconds(timeOutSec));
                result.HasError = false;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.HasError = true;
            }

            return result;
        }
    }

    public class ParseResult
    {
        public bool HasPattern { get; set; } = false;
        public bool HasError { get; set; } = false;
        public string Message { get; set; }
        public Match Match { get; set; }
    }

    public class FileParseResult
    {
        public bool HasFileName { get; set; } = false;
        public bool HasFileMask { get; set; } = new();
        public bool FileMaskIsCorrect { get; set; } = new();

        public ParseResult DocTitle { get; set; } = new();
        public ParseResult DocNumber { get; set; } = new();
        public ParseResult DocDate { get; set; } = new();
        public ParseResult DocNotes { get; set; } = new();

        public ParseResult CodeMo { get; set; } = new();
        public ParseResult CodeSmo { get; set; } = new();
        public ParseResult CodeFund { get; set; } = new();
    }
}
