namespace Repository;


/// <summary>
/// Represents the database context for a user authentication and Enteties realting to the User such as Cheep,Followers and Relatoins.
/// </summary>


public class DatabaseContext : IdentityDbContext<Author, IdentityRole<string>, string>
{

    /// <summary>
    /// Initializes a new instance of the DatabaseContext class.
    /// </summary>
    /// <param name="options">The Configuration options used by the DatabaseContext.</param>
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Follower> Followers { get; set; }
    public virtual DbSet<Author> Authors => Users;
    public virtual DbSet<Reaction> Reactions => Set<Reaction>();


    /// <summary>
    /// Configures the model relationships and constraints for the database context.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cheep>().Property(c => c.Text).HasMaxLength(160);
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
        modelBuilder.Entity<Follower>().HasKey(a => new{a.FollowerId, a.FollowedId});

        modelBuilder.Entity<Reaction>().HasKey(r => new { r.CheepId, r.AuthorName });
        modelBuilder.Entity<Reaction>().Property(m => m.ReactionType).HasConversion<string>();
    }

    /// <summary>
    /// Initializes the database by applying pending migrations and seeding initial data -- See Inizialise file.
    /// </summary>
    public void InitializeDB()
    {
        Database.Migrate();
        DbInitializer.SeedDatabase(this);
    }
}