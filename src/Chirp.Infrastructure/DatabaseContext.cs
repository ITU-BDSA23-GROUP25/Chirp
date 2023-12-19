namespace Repository;

public class DatabaseContext : IdentityDbContext<Author, IdentityRole<string>, string>
{

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Follower> Followers { get; set; }
    public virtual DbSet<Author> Authors => Users;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cheep>().Property(c => c.Text).HasMaxLength(160);
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
        modelBuilder.Entity<Follower>().HasKey(a => new{a.FollowerId, a.FollowedId});
    }

    public void InitializeDB()
    {
        Database.Migrate();
        DbInitializer.SeedDatabase(this);
    }
}