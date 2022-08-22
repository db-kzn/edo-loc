using AutoMapper.Execution;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public class Act
    {
        public ContactResponse Contact { get; set; } = null;
        public Dictionary<string, ContactResponse> Contacts { get; set; } = new();
    };

    public partial class RouteStepDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public RouteStepModel Step { get; set; }

        [Inject] private IDocumentManager DocManager { get; set; }

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = true;

        public static Act Members { get; set; } = new();
        public static Act Agreementers { get; set; } = new();
        public static Act Reviewers { get; set; } = new();

        protected override void OnInitialized()
        {
            Members.Contact = null;
            Agreementers.Contact = null;
            Reviewers.Contact = null;

            Members.Contacts.Clear();
            Agreementers.Contacts.Clear();
            Reviewers.Contacts.Clear();

            Step.Members.ForEach(m =>
            {
                if (!m.IsAdditional)
                {
                    Members.Contact = m.Contact;
                    AddContact(Members);
                }
                else if (m.Act == ActTypes.Agreement)
                {
                    Agreementers.Contact = m.Contact;
                    AddContact(Agreementers);
                }
                else if (m.Act == ActTypes.Review)
                {
                    Reviewers.Contact = m.Contact;
                    AddContact(Reviewers);
                }
            });

            //base.OnInitialized();
        }

        private async Task<IEnumerable<ContactResponse>> SearchContactsAsync(UserBaseRoles role, string search)
        {
            var request = new SearchContactsRequest()
            {
                BaseRole = role,
                OrgType = Step.OrgType,
                SearchString = search
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
        }

        private static string IconByOrgType(OrgTypes ot) => ot switch
        {
            OrgTypes.Fund => Icons.Material.Outlined.HealthAndSafety,
            OrgTypes.MO => Icons.Material.Outlined.MedicalServices,
            OrgTypes.SMO => Icons.Material.Outlined.Museum,
            OrgTypes.MEO => Icons.Material.Outlined.LocalPolice, //MilitaryTech,
            OrgTypes.Treasury => Icons.Material.Outlined.AccountBalance,
            _ => Icons.Material.Outlined.Business
        };
        private string LabelByStep(RouteStepModel step) =>
            $"{_localizer[(step.OnlyHead) ? "Chief" : "Employee"]} " +
            $"{_localizer["of the"]} {_localizer[step.OrgType.ToString()]}";
        private static string ContactName(ContactResponse c) =>
            $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";
        private static ContactResponse CloneContact(ContactResponse c) => new()
        {
            Id = c.Id,
            Surname = c.Surname,
            GivenName = c.GivenName,

            OrgId = c.OrgId,
            InnLe = c.InnLe,
            OrgShortName = c.OrgShortName
        };

        private static void AddContact(Act act)
        {
            if (act.Contact is null) { return; }
            var name = ContactName(act.Contact);
            if (!act.Contacts.ContainsKey(name)) { act.Contacts.Add(name, act.Contact); } //CloneContact(act.Contact)
            act.Contact = null;
        }
        private static void DelContact(Act act, MudChip chip) => act.Contacts.Remove(chip.Text);

        private void Ok()
        {
            Step.Members.Clear();

            AddContact(Members);
            AddContact(Agreementers);
            AddContact(Reviewers);

            foreach (var c in Members.Contacts.Values)
            {
                Step.Members.Add(new() { IsAdditional = false, Act = Step.ActType, UserId = c.Id, Contact = c });
            }

            foreach (var c in Agreementers.Contacts.Values)
            {
                Step.Members.Add(new() { IsAdditional = true, Act = ActTypes.Agreement, UserId = c.Id, Contact = c });
            }

            foreach (var c in Reviewers.Contacts.Values)
            {
                Step.Members.Add(new() { IsAdditional = true, Act = ActTypes.Review, UserId = c.Id, Contact = c });
            }

            MudDialog.Close(DialogResult.Ok(Step));
        }
        private void Delete() => MudDialog.Close(DialogResult.Ok<RouteStepModel>(null));
    }
}
