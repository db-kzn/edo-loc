using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Responses.Admin
{
    public class OrgResponse
    {
		public int Id { get; set; }

		public string Inn { get; set; }
		public string Ogrn { get; set; }
		public string Name { get; set; }

		public string UserId { get; set; } // Owner - User ID
		public string UserSnils { get; set; } // User SNISL

		public bool IsPublic { get; set; } // CA no public - hide
		public int TypeIx { get; set; } = (int)OrgTypes.Undefined; // N/D, FOND, SMO, MO, CA
		public int StateIx { get; set; } = (int)OrgStates.Undefined; // N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted

		public string Phone { get; set; }
		public string Email { get; set; }
	}
}
