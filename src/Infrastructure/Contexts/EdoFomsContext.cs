﻿using EDO_FOMS.Application.Interfaces.Services;
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
using EDO_FOMS.Domain.Entities.System;
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
        public DbSet<RouteDocType> RouteDocTypes { get; set; }
        public DbSet<RouteOrgType> OrganizationTypes { get; set; }
        public DbSet<RouteStage> RouteStages { get; set; }
        public DbSet<RouteStep> RouteSteps { get; set; }
        public DbSet<RouteStepMember> RouteStepMembers { get; set; }
        public DbSet<RouteFileParse> RouteFileParses { get; set; }
        public DbSet<RoutePacketFile> RoutePacketFiles { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<DocumentStatus> DocumentStatuses { get; set; }
        public DbSet<DocPacketFile> DocPacketFiles { get; set; }
        public DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<ParamGroup> ParamGroups { get; set; }
        public DbSet<Param> Params { get; set; }

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

            builder.Entity<Company>(entity => entity.ToTable(name: "Companies", schema: "dir"));

            builder.Entity<RouteOrgType>(entity => entity.ToTable(name: "RouteOrgTypes", schema: "dir"));
            builder.Entity<RouteStage>(entity => entity.ToTable(name: "RouteStages", schema: "dir"));
            builder.Entity<RouteStep>(entity => entity.ToTable(name: "RouteSteps", schema: "dir"));
            builder.Entity<RouteFileParse>(entity => {
                entity.ToTable(name: "RouteFileParses", schema: "dir");
                entity.HasKey(p => new { p.RouteId, p.PatternType });
            });
            builder.Entity<RoutePacketFile>(entity => entity.ToTable(name: "RoutePacketFiles", schema: "dir"));

            builder.Entity<RouteStepMember>(entity =>
            {
                entity.ToTable(name: "RouteStepMembers", schema: "dir");
                entity.HasKey(m => new { m.RouteStepId, m.UserId, m.Act, m.IsAdditional });
            });

            builder.Entity<DocumentType>(entity =>
            {
                entity.ToTable(name: "DocumentTypes", schema: "dir");

                entity.HasMany(r => r.Routes)
                    .WithMany(dt => dt.DocTypes)
                    .UsingEntity<RouteDocType>();
            });

            builder.Entity<Route>(entity =>
            {
                entity.ToTable(name: "Routes", schema: "dir");

                entity.HasMany(r => r.DocTypes)
                    .WithMany(dt => dt.Routes)
                    .UsingEntity<RouteDocType>();
            });

            builder.Entity<RouteDocType>(entity =>
            {
                entity.ToTable(name: "RouteDocTypes", schema: "dir");

                entity.HasOne(rdt => rdt.Route)
                    .WithMany(r => r.RouteDocTypes)
                    .HasForeignKey(rdt => rdt.RouteId);

                entity.HasOne(rdt => rdt.DocumentType)
                    .WithMany(dt => dt.RouteDocTypes)
                    .HasForeignKey(rdt => rdt.DocumentTypeId);

                entity.HasKey(rdt => new { rdt.RouteId, rdt.DocumentTypeId });
            });

            builder.Entity<Organization>(entity => entity.ToTable(name: "Organizations", schema: "org"));
            builder.Entity<Employee>(entity => entity.ToTable(name: "Employees", schema: "org"));
            builder.Entity<Certificate>(entity => entity.ToTable(name: "Certificates", schema: "org"));

            builder.Entity<Document>(entity => entity.ToTable(name: "Documents", schema: "doc"));
            builder.Entity<Agreement>(entity => entity.ToTable(name: "Agreements", schema: "doc"));
            builder.Entity<DocumentStatus>(entity => entity.ToTable(name: "DocumentStatuses", schema: "doc"));
            builder.Entity<DocPacketFile>(entity => entity.ToTable(name: "DocPacketFiles", schema: "doc"));
            builder.Entity<DocumentExtendedAttribute>(entity => entity.ToTable(name: "DocumentExtendedAttributes", schema: "doc"));

            builder.Entity<Department>(entity => entity.ToTable(name: "Departments", schema: "sys"));
            builder.Entity<JobTitle>(entity => entity.ToTable(name: "JobTitles", schema: "sys"));
            builder.Entity<Subscribe>(entity => entity.ToTable(name: "Subscribes", schema: "sys"));
            builder.Entity<ParamGroup>(entity => entity.ToTable(name: "ParamGroups", schema: "sys"));
            builder.Entity<Param>(entity => entity.ToTable(name: "Params", schema: "sys"));
            builder.Entity<Audit>(entity => entity.ToTable(name: "AuditTrails", schema: "sys"));

            builder.Entity<ChatHistory<EdoFomsUser>>(entity =>
            {
                entity.ToTable(name: "ChatHistory", schema: "sys");

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
                entity.ToTable(name: "Users", schema: "identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<EdoFomsRole>(entity => entity.ToTable(name: "Roles", schema: "identity"));
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable(name: "UserRoles", schema: "identity"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable(name: "UserClaims", schema: "identity"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable(name: "UserLogins", schema: "identity"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable(name: "UserTokens", schema: "identity"));

            builder.Entity<EdoFomsRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", schema: "identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}