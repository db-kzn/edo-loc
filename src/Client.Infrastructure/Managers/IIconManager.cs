using EDO_FOMS.Client.Infrastructure.Model;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Client.Infrastructure.Managers
{
    public interface IIconManager : IManager
    {
        IconModel OrgTypeIcon(OrgTypes orgType);

        IconModel DocTypeIcon(DocIcons icon);

        IconModel AgrStateIcon(AgreementStates state);

        IconModel AgrActionIcon(AgreementActions action);

        IconModel ActTypeIcon(ActTypes act);
    }
}
