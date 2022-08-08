using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Models.Chat;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EDO_FOMS.Domain.Entities.ExtendedAttributes;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Infrastructure.Models.Audit;

namespace EDO_FOMS.Infrastructure.Contexts
{
    public class EdoFomsContext : AuditableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public EdoFomsContext(DbContextOptions<EdoFomsContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStage> RouteStages { get; set; }
        public DbSet<RouteStageStep> RouteStageSteps { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<DocumentStatus> DocumentStatuses { get; set; }
        public DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }

        public DbSet<ChatHistory<EdoFomsUser>> ChatHistories { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }
            if (_currentUserService.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            base.OnModelCreating(builder);

            builder.Entity<Company>(entity => entity.ToTable(name: "Companies", "dir"));
            builder.Entity<DocumentType>(entity => entity.ToTable(name: "DocumentTypes", "dir"));
            builder.Entity<Route>(entity => entity.ToTable(name: "Routes", "dir"));
            builder.Entity<RouteStage>(entity => entity.ToTable(name: "RouteStages", "dir"));
            builder.Entity<RouteStageStep>(entity => entity.ToTable(name: "RouteStageSteps", "dir"));

            builder.Entity<Organization>(entity => entity.ToTable(name: "Organizations", "org"));
            builder.Entity<Certificate>(entity => entity.ToTable(name: "Certificates", "org"));

            builder.Entity<Document>(entity => entity.ToTable(name: "Documents", "doc"));
            builder.Entity<Agreement>(entity => entity.ToTable(name: "Agreements", "doc"));
            builder.Entity<DocumentStatus>(entity => entity.ToTable(name: "DocumentStatuses", "doc"));
            builder.Entity<DocumentExtendedAttribute>(entity => entity.ToTable(name: "DocumentExtendedAttributes", "doc"));

            builder.Entity<Subscribe>(entity => entity.ToTable(name: "Subscribes", "sys"));
            builder.Entity<Audit>(entity => entity.ToTable(name: "AuditTrails", "sys"));

            builder.Entity<ChatHistory<EdoFomsUser>>(entity =>
            {
                entity.ToTable("ChatHistory", "sys");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.ChatHistoryFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.ChatHistoryToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            builder.Entity<EdoFomsUser>(entity =>
            {
                entity.ToTable(name: "Users", "identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<EdoFomsRole>(entity => entity.ToTable(name: "Roles", "identity"));
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles", "identity"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims", "identity"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins", "identity"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens", "identity"));

            builder.Entity<EdoFomsRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}