using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options) { }


        #region Db set
        public DbSet<Category> categories { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<CommentRep> commentReps { get; set; }
        public DbSet<image_user> image_Users { get; set; }
        public DbSet<Like> likes { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Post_Image> Post_Images { get; set; }
        public DbSet<role> roles { get; set; }
        public DbSet<Tick> ticks { get; set; }
        public DbSet<User> users { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>()
                .HasMany(p => p.posts)
                .WithOne(u => u.category)
                .HasForeignKey(f => f.category_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasMany(m => m.commentReps)
                .WithOne(o => o.comments)
                .HasForeignKey(f => f.comment_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasMany(h => h.post_Images)
                .WithOne(o => o.post)
                .HasForeignKey(f => f.post_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasMany(m => m.likes)
                .WithOne(o => o.post)
                .HasForeignKey(f => f.post_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasMany(m => m.comments)
                .WithOne(o => o.post)
                .HasForeignKey(f => f.post_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<role>()
                .HasMany(m => m.users)
                .WithOne(o => o.roles)
                .HasForeignKey(h => h.role_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tick>()
                .HasMany(h => h.users)
                .WithOne(o => o.tick)
                .HasForeignKey(h => h.tick_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.image_Users)
                .WithOne(o => o.user)
                .HasForeignKey(h => h.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.categories)
                .WithOne(o => o.user)
                .HasForeignKey(f => f.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.posts)
                .WithOne(o => o.user)
                .HasForeignKey(f => f.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.post_Images)
                .WithOne(o => o.user)
                .HasForeignKey(f => f.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.likes)
                .WithOne(o => o.user)
                .HasForeignKey(h => h.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.comments)
                .WithOne(o => o.user)
                .HasForeignKey(h => h.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(m => m.commentReps)
                .WithOne(o => o.user)
                .HasForeignKey(f => f.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentRep>()
                .HasMany(m => m.commentReps)
                .WithOne(o => o.commentRep)
                .HasForeignKey(f => f.comentRep2)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
