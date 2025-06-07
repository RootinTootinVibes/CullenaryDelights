using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<Step> Steps { get; set; }
    public DbSet<CommentAndRating> CommentsAndRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CommentAndRating>(b =>
        {
            b.HasKey(c => c.CommentID);
            b.Property(c => c.CommentID)
                .HasColumnName("CommentID")
                .ValueGeneratedOnAdd();
        });

        //Configuring cascading delete so when a recipe is deleted, so are the comments
        modelBuilder.Entity<CommentAndRating>()
            .HasOne(cr => cr.Recipe)
            .WithMany(r => r.CommentsAndRatings)
            .HasForeignKey(cr => cr.RecipeID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommentAndRating>()
            .HasOne(cr => cr.User)
            .WithMany(u => u.CommentAndRatings)
            .HasForeignKey(cr => cr.UserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}