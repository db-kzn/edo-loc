using EDO_FOMS.Client.Infrastructure.Model;
using EDO_FOMS.Domain.Enums;
using MudBlazor;

namespace EDO_FOMS.Client.Infrastructure.Managers
{
    public class IconManager : IIconManager
    {
        private readonly string defaultIcon = Icons.Material.Outlined.HelpOutline;

        public IconModel DocTypeIcon(DocIcons icon)
        {
            return icon switch
            {
                DocIcons.AssignmentLate => new IconModel()
                {
                    Icon = Icons.Material.Outlined.AssignmentLate
                },
                DocIcons.AssignmentTurnedIn => new IconModel()
                {
                    Icon = Icons.Material.Outlined.AssignmentTurnedIn
                },
                DocIcons.CalendarToday => new IconModel()
                {
                    Icon = Icons.Material.Outlined.CalendarToday
                },

                DocIcons.ContactPage => new IconModel()
                {
                    Icon = Icons.Material.Outlined.ContactPage
                },
                DocIcons.Description => new IconModel()
                {
                    Icon = Icons.Material.Outlined.Description
                },
                DocIcons.Difference => new IconModel()
                {
                    Icon = Icons.Material.Outlined.Difference
                },

                DocIcons.EventRepeat => new IconModel()
                {
                    Icon = Icons.Material.Outlined.EventRepeat
                },
                DocIcons.FactCheck => new IconModel()
                {
                    Icon = Icons.Material.Outlined.FactCheck
                },
                DocIcons.HelpCenter => new IconModel()
                {
                    Icon = Icons.Material.Outlined.HelpCenter
                },

                DocIcons.Newspaper => new IconModel()
                {
                    Icon = Icons.Material.Outlined.Newspaper
                },
                DocIcons.NoteAdd => new IconModel()
                {
                    Icon = Icons.Material.Outlined.NoteAdd
                },
                DocIcons.Receipt => new IconModel()
                {
                    Icon = Icons.Material.Outlined.Receipt
                },

                DocIcons.TableChart => new IconModel()
                {
                    Icon = Icons.Material.Outlined.TableChart
                },

                _ => new IconModel()
                { 
                    Icon = defaultIcon
                }
            };
        }

        public IconModel AgrStateIcon(AgreementStates state)
        {
            return state switch
            {
                AgreementStates.Expecting => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.NotificationsPaused,
                    Label = "Expecting"
                },
                AgreementStates.Incoming => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.MarkunreadMailbox,
                    Label = "Incoming"
                },
                AgreementStates.Received => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.Drafts,
                    Color = Color.Primary,
                    Label = "In work"
                },
                AgreementStates.Verifed => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.DoneOutline,
                    Color = Color.Success,
                    Label = "Verified"
                },
                AgreementStates.Approved => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.Verified,
                    Color = Color.Success,
                    Label = "Approved"
                },
                AgreementStates.Signed => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.QrCode,
                    Color = Color.Success,
                    Label = "Signed"
                },
                AgreementStates.Refused => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.Report,
                    Color = Color.Warning,
                    Label = "Refused"
                },
                AgreementStates.Rejected => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.DisabledByDefault,
                    Color = Color.Error,
                    Label = "Rejected"
                },
                AgreementStates.Deleted => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.DeleteForever,
                    Label = "Deleted"
                },
                AgreementStates.Control => new IconModel()
                {
                    Icon = Icons.Material.TwoTone.Inventory,
                    Color = Color.Info,
                    Label = "Control"
                },
                _ => new IconModel() { Icon = defaultIcon }
            };
        }

        public IconModel AgrActionIcon(AgreementActions action)
        {
            return action switch
            {

                _ => new IconModel() { Icon = defaultIcon }
            };
        }

        public IconModel ActTypeIcon(ActTypes act)
        {
            return act switch
            {
                ActTypes.Signing => new IconModel()
                {
                    Icon = Icons.Material.Outlined.Draw,
                    Label = "To Sign"
                },
                ActTypes.Agreement => new IconModel()
                {
                    Icon = Icons.Material.Outlined.OfflinePin,
                    Label = "To Approve"
                },
                ActTypes.Review => new IconModel()
                {
                    Icon = Icons.Material.Outlined.MapsUgc,
                    Label = "To Check"
                },
                ActTypes.Initiation => new IconModel()
                {
                    Icon = Icons.Material.Outlined.SlowMotionVideo,
                    Label = "To Run"
                },
                ActTypes.Executing => new IconModel()
                {
                    Icon = Icons.Material.Outlined.SlowMotionVideo,
                    Label = "To Run"
                },
                _ => new IconModel() { Icon = defaultIcon }
            };
        }

        public IconModel OrgTypeIcon(OrgTypes orgType)
        {
            return orgType switch
            {
                OrgTypes.MO => new IconModel()
                {
                    Icon = Icons.Material.Outlined.MedicalServices,
                    Label = "MO",
                    Color = Color.Primary
                },

                OrgTypes.SMO => new IconModel()
                {
                    Icon = Icons.Material.Outlined.Museum,
                    Label = "SMO",
                    Color = Color.Success
                },
                OrgTypes.Fund => new IconModel()
                {
                    Icon = Icons.Material.Outlined.HealthAndSafety,
                    Label = "Fund OMS",
                    Color = Color.Error
                },
                OrgTypes.MEO => new IconModel()
                {
                    Icon = Icons.Material.Outlined.LocalPolice
                },
                OrgTypes.Treasury => new IconModel()
                {
                    Icon = Icons.Material.Outlined.AccountBalance
                },
                _ => new IconModel() { Icon = defaultIcon }
            };
        }

    }
}
