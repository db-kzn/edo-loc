using System;
using System.Collections.Generic;
using EDO_FOMS.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace EDO_FOMS.Infrastructure.Models.Identity
{
    public class EdoFomsRole : IdentityRole, IAuditableEntity<string>
    {
        public EdoFomsRole() : base()
        {
            RoleClaims = new HashSet<EdoFomsRoleClaim>();
        }

        public EdoFomsRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<EdoFomsRoleClaim>();
            Description = roleDescription;
        }

        public string Description { get; set; }
        public virtual ICollection<EdoFomsRoleClaim> RoleClaims { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}