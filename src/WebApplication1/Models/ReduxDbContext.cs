using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class ReduxDbContext : DbContext
    {
        public ReduxDbContext(DbContextOptions<ReduxDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.ID).IsRequired();
            });
            modelBuilder.Entity<Todo>(entity =>
            {
                entity.Property(e => e.ID).IsRequired();
            });
            modelBuilder.Entity<Picture>(entity =>
            {
                entity.Property(e => e.ID).IsRequired();
                
            });
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.ID).IsRequired();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.ID).IsRequired();
              
            });
            modelBuilder.Entity<UserLike>(entity =>
            {
                entity.Property(e => e.LikeID).IsRequired();

            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.ID).IsRequired();

            });
        }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Todo> Todos { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLike> UserLikes { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
    }
}