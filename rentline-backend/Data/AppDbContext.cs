
using Microsoft.EntityFrameworkCore;
using rentline_backend.Domain;
using System;
using System.Linq.Expressions;

namespace rentline_backend.Data;

public class AppDbContext : DbContext
{
    public Guid? CurrentOrgId { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Lease> Leases => Set<Lease>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<Invite> Invites => Set<Invite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global query filter for IMultiTenant
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IMultiTenant).IsAssignableFrom(entityType.ClrType))
            {
                // e => CurrentOrgId == null || e.OrganizationId == CurrentOrgId.GetValueOrDefault()
                var parameter = Expression.Parameter(entityType.ClrType, "e");

                var orgProp = Expression.Property(
                    parameter,
                    nameof(IMultiTenant.OrganizationId) // Guid
                );

                var currentOrg = Expression.Property(
                    Expression.Constant(this),
                    nameof(CurrentOrgId) // Guid?
                );

                var hasNoOrg = Expression.Equal(
                    currentOrg,
                    Expression.Constant(null, typeof(Guid?))
                );

                // Guid? -> Guid without using nameof on a generic:
                var getVOD = typeof(Guid?).GetMethod("GetValueOrDefault", Type.EmptyTypes)!;
                var currentOrgNonNull = Expression.Call(currentOrg, getVOD); // Guid

                var equals = Expression.Equal(orgProp, currentOrgNonNull);

                var body = Expression.OrElse(hasNoOrg, equals);
                var lambda = Expression.Lambda(body, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Properties)
            .WithOne(p => p.Organization) // or .WithOne() if no nav back
            .HasForeignKey(p => p.OrganizationId);

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Property>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasMany(x => x.Units).WithOne(u => u.Property).HasForeignKey(u => u.PropertyId);
        });

        modelBuilder.Entity<Unit>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Property).WithMany(p => p.Units).HasForeignKey(x => x.PropertyId);
        });

        modelBuilder.Entity<Lease>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantUserId);
        });

        modelBuilder.Entity<MaintenanceRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId);
            e.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedByUserId);
        });

        modelBuilder.Entity<Invite>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Token).IsUnique();
            e.Property(x => x.Email).IsRequired();
        });
    }
}
