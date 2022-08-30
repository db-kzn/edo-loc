using EDO_FOMS.Application.Features.Agreements.Commands;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Progress
{
    public partial class MembersDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public AgreementModel _agreement { get; set; } = new();

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;

        public Dictionary<string, ContactResponse> contacts = new();
        public ContactResponse contact = null;

        public async Task SendMembersAsync()
        {
            var members = ContactsToIds(contacts.Values.ToList());
            if (contact != null && !HasContact(contacts, contact)) { members.Add(contact.Id); }

            AddAgreementMembersCommand command = new()
            {
                Id = _agreement.AgreementId,
                Members = members
            };

            var response = await DocManager.PostMembersAsync(command);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task<IEnumerable<ContactResponse>> SearchMembersAsync(string search)
        {
            var response = await DocManager.GetAgreementMembersAsync((int)_agreement.EmplOrgId, search);
            return (response.Succeeded) ? response.Data : new();
        }

        public void DelMember(MudChip chip) => contacts.Remove(chip.Text);

        public void AddMember() => AddContact(contacts, ref contact);

        private static void AddContact(Dictionary<string, ContactResponse> dic, ref ContactResponse c)
        {
            if (!HasContact(dic, c))
            {
                dic.Add(ContactName(c), CloneContact(c));
            }
            c = null;
        }

        private static bool HasContact(Dictionary<string, ContactResponse> dic, ContactResponse c)
        {
            return dic.ContainsKey(ContactName(c));
        }

        private static ContactResponse CloneContact(ContactResponse c)
        {
            return new()
            {
                Id = c.Id,
                Surname = c.Surname,
                GivenName = c.GivenName,
                OrgId = c.OrgId,
                InnLe = c.InnLe
            };
        }

        private static string ContactName(ContactResponse c)
        {
            return $"[{c.InnLe}] {c.Surname} {c.GivenName}";
        }

        private static List<string> ContactsToIds(List<ContactResponse> list)
        {
            List<string> ids = new();
            list.ForEach((l) => { ids.Add(l.Id); });
            //foreach (var l in list) { ids.Add(l.Id); }
            return ids;
        }
    }
}
