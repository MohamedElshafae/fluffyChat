
using FluffyApp.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluffyApp.EF.Data
{
    public class AppDbContext: IdentityDbContext<User ,IdentityRole<Guid>, Guid>
    {
        public DbSet<User> Users {  get; set; }
        public DbSet<Tag> Tags { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("roles");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Ignore(e => e.PhoneNumberConfirmed);
                entity.Ignore(e => e.TwoFactorEnabled);
                entity.Ignore(e => e.LockoutEnd);
                entity.Ignore(e => e.LockoutEnabled);
                entity.Ignore(e => e.AccessFailedCount);
            });



            modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
            modelBuilder.Ignore<IdentityUserClaim<Guid>>();
            modelBuilder.Ignore<IdentityUserLogin<Guid>>();
            modelBuilder.Ignore<IdentityUserToken<Guid>>();
            modelBuilder.Ignore<IdentityUserRole<Guid>>();
        }


    }
}
