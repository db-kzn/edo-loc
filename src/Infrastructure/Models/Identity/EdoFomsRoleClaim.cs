using System;
using EDO_FOMS.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace EDO_FOMS.Infrastructure.Models.Identity
{
    public class EdoFomsRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
    {
        public EdoFomsRoleClaim() : base()
        {
        }

        public EdoFomsRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }

        public virtual EdoFomsRole Role { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}