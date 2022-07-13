using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Features.MedOrgs.Queries
{
    public class MedOrgsResponse
    {
		public int Id { get; set; }

		public string Inn { get; set; }
		public string Ogrn { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }

		//public string UserId { get; set; } // Owner - User ID
		//public string UserSnils { get; set; } // User SNISL

		public bool IsPublic { get; set; } // CA no public - hide
		public OrgTypes Type { get; set; } = OrgTypes.MO; // N/D, FOND, SMO, MO, CA
		public OrgStates State { get; set; } = OrgStates.Active;// N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted

		public string Phone { get; set; }
		public string Email { get; set; }

		public DateTime CreatedOn { get; set; }
	}
}
