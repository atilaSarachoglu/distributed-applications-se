using Common.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repos
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Like> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region UsersEntityHandler
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(64);

                entity.Property(u => u.Email)
                      .HasMaxLength(255);

                entity.HasData(new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin",
                    IsAdmin = true,
                    Email = null
                });
            });
            #endregion

            #region PostsEntityHandler
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Title)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(p => p.Content)
                      .IsRequired()
                      .HasMaxLength(1_000);

                entity.Property(p => p.CreatedAt)
                      .IsRequired();

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region LikesEntityHandler
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.HasOne(l => l.User)
                      .WithMany()
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Post)
                      .WithMany()
                      .HasForeignKey(l => l.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region RepliesEntityHandler
            modelBuilder.Entity<Reply>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Content)
                      .IsRequired()
                      .HasMaxLength(1_000);

                entity.Property(r => r.CreatedAt)
                      .IsRequired();

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Post)
                      .WithMany()
                      .HasForeignKey(r => r.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLazyLoadingProxies().UseSqlServer("Server=localhost;Database=JustGirlyThingsDB;Trusted_Connection=True;");
        }
    }
}
