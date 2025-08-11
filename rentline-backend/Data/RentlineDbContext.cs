using Microsoft.EntityFrameworkCore;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Entities;
using rentline_backend.Domain.Enums;
using System;
using System.Linq;

namespace rentline_backend.Data
{
    /// <summary>
    ///     EF Core database context for the Rentline backend. Registers
    ///     entities and configures global query filters for multi‑tenancy.
    /// </summary>
    /// <remarks>
    ///     Construct a new context. The current organisation identifier is
    ///     injected to scope queries. In production this should be
    ///     provided via the current user context (e.g. IHttpContextAccessor).
    /// </remarks>
    public class RentlineDbContext(DbContextOptions<RentlineDbContext> options) : DbContext(options)
    {
        public Guid? CurrentOrgId { get; set; }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<UnitKey> UnitKeys => Set<UnitKey>();
        public DbSet<UnitEquipment> UnitEquipment => Set<UnitEquipment>();
        public DbSet<UnitInspection> UnitInspections => Set<UnitInspection>();
        public DbSet<UnitInspectionIssue> UnitInspectionIssues => Set<UnitInspectionIssue>();
        public DbSet<UnitInspectionPhoto> UnitInspectionPhotos => Set<UnitInspectionPhoto>();

        // New domain sets introduced to support a full SaaS offering.
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Lease> Leases => Set<Lease>();
        public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
        public DbSet<Invite> Invites => Set<Invite>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply a global query filter to all OrgEntity types. This ensures
            // queries only return records for the current organisation.
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(OrgEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(RentlineDbContext)
                        .GetMethod(nameof(SetOrgFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                        ?.MakeGenericMethod(entityType.ClrType);
                    method?.Invoke(null, [builder, CurrentOrgId]);
                }
            }

            // Configure explicit relationships between entities. While many
            // associations can be inferred by EF Core conventions, defining
            // them here clarifies the intended multiplicity and delete
            // behaviour for each navigation property. This ensures that
            // foreign key constraints are created correctly and that
            // cascading rules are enforced consistently across the schema.

            // An organisation owns many users; users belong to a single
            // organisation via their OrgId. Deleting an organisation
            // cascades deletion of its users.
            builder.Entity<Organization>()
                .HasMany(o => o.Users)
                .WithOne()
                .HasForeignKey(u => u.OrgId)
                .OnDelete(DeleteBehavior.Cascade);

            // An organisation owns many properties; properties belong to
            // a single organisation via their OrgId. Deleting an
            // organisation cascades deletion of its properties.
            builder.Entity<Organization>()
                .HasMany(o => o.Properties)
                .WithOne()
                .HasForeignKey(p => p.OrgId)
                .OnDelete(DeleteBehavior.Cascade);

            // A property contains many units; a unit belongs to a single
            // property via PropertyId. Removing a property cascades to its
            // units.
            builder.Entity<Property>()
                .HasMany(p => p.Units)
                .WithOne(u => u.Property!)
                .HasForeignKey(u => u.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // A property can have many images; an image belongs to a
            // single property via PropertyId.
            builder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property!)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // A unit has many keys; a key belongs to a single unit via
            // UnitId. Deleting a unit cascades deletion of its keys.
            builder.Entity<Unit>()
                .HasMany(u => u.Keys)
                .WithOne(k => k.Unit!)
                .HasForeignKey(k => k.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            // A unit has many pieces of equipment; each piece of equipment
            // belongs to a single unit via UnitId.
            builder.Entity<Unit>()
                .HasMany(u => u.Equipment)
                .WithOne(e => e.Unit!)
                .HasForeignKey(e => e.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            // A unit has many inspections; an inspection belongs to a
            // single unit via UnitId.
            builder.Entity<Unit>()
                .HasMany(u => u.Inspections)
                .WithOne(i => i.Unit!)
                .HasForeignKey(i => i.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            // An inspection has many issues; each issue belongs to a
            // single inspection via UnitInspectionId.
            builder.Entity<UnitInspection>()
                .HasMany(i => i.Issues)
                .WithOne(ii => ii.Inspection!)
                .HasForeignKey(ii => ii.UnitInspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // An inspection has many photos; each photo belongs to a
            // single inspection via UnitInspectionId.
            builder.Entity<UnitInspection>()
                .HasMany(i => i.Photos)
                .WithOne(p => p.Inspection!)
                .HasForeignKey(p => p.UnitInspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // A lease belongs to a single unit and a single tenant user.
            // Units and users may have multiple leases over time, so we
            // avoid cascading deletes to preserve historical data.
            builder.Entity<Lease>()
                .HasOne(l => l.Unit)
                .WithMany()
                .HasForeignKey(l => l.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Lease>()
                .HasOne(l => l.TenantUser)
                .WithMany(u => u.Leases)
                .HasForeignKey(l => l.TenantUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // A maintenance request belongs to a unit and to the user who
            // created it. Requests should remain even if the unit or user
            // is removed, so deletions are restricted.
            builder.Entity<MaintenanceRequest>()
                .HasOne(r => r.Unit)
                .WithMany()
                .HasForeignKey(r => r.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<MaintenanceRequest>()
                .HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // An invite belongs to an organisation via OrgId. Invites are
            // removed when the organisation is deleted.
            builder.Entity<Invite>()
                .HasOne<Organization>()
                .WithMany()
                .HasForeignKey(inv => inv.OrgId)
                .OnDelete(DeleteBehavior.Cascade);

            // A payment belongs to a single lease. Deleting a lease
            // cascades deletion of its payments.
            builder.Entity<Payment>()
                .HasOne(p => p.Lease)
                .WithMany()
                .HasForeignKey(p => p.LeaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // A document may be associated with a property, unit, lease
            // and the user who uploaded it. All associations are optional
            // and deletions of the related entities are restricted so
            // documents remain for audit purposes.
            builder.Entity<Document>()
                .HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Document>()
                .HasOne(d => d.Unit)
                .WithMany()
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Document>()
                .HasOne(d => d.Lease)
                .WithMany()
                .HasForeignKey(d => d.LeaseId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Document>()
                .HasOne(d => d.UploadedByUser)
                .WithMany()
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SetOrgFilter<TEntity>(ModelBuilder modelBuilder, Guid orgId) where TEntity : OrgEntity
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.OrgId == orgId);
        }
    }
}