using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QR_Domain.Entities;

public class QRContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public QRContext(DbContextOptions<QRContext> options) : base(options)
    {
    }
    public DbSet<Tag> Tages { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Ignore(e => e.EmailConfirmed);
            entity.Ignore(e => e.PhoneNumberConfirmed);
            entity.Ignore(e => e.TwoFactorEnabled);
            entity.Ignore(e => e.LockoutEnd);
            entity.Ignore(e => e.LockoutEnabled);
            entity.Ignore(e => e.AccessFailedCount);
        });


    }
}
