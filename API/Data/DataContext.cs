using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class DataContext(DbContextOptions options) :
                        IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole,
                        IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);


                builder.Entity<AppUser>()
                        .HasMany(ur => ur.UserRoles)
                        .WithOne(u => u.User)
                        .HasForeignKey(i => i.UserId)
                        .IsRequired();

                builder.Entity<AppRole>()
                        .HasMany(ur => ur.UserRoles)
                        .WithOne(u => u.Role)
                        .HasForeignKey(i => i.RoleId)
                        .IsRequired();

                builder.Entity<UserLike>()
                        .HasKey(k => new { k.SourceUserId, k.TargetUserId });

                builder.Entity<UserLike>()
                        .HasOne(s => s.SourceUser)
                        .WithMany(l => l.LikedUsers)
                        .HasForeignKey(s => s.SourceUserId)
                        .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<UserLike>()
                        .HasOne(t => t.TargetUser)
                        .WithMany(l => l.LikedByUsers)
                        .HasForeignKey(t => t.TargetUserId)
                        //.OnDelete(DeleteBehavior.NoAction);
                        .OnDelete(DeleteBehavior.Cascade);


                builder.Entity<Message>()
                        .HasOne(x => x.Recipient)
                        .WithMany(x => x.MessagesReceived)
                        .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<Message>()
                        .HasOne(x => x.Sender)
                        .WithMany(x => x.MessagesSent)
                        .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);

                //builder.ApplyUtcDateTimeConverter();
        }
}
