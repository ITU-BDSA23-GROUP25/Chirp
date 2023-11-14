namespace Repository;

public class DatabaseContext : IdentityDbContext<Author>
{

public DatabaseContext()
    {
    }
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
    {
    }


    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
    string DbPath = Path.Join(Path.GetTempPath(), "db.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cheep>().Property(c => c.Text).HasMaxLength(160);
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(32);
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique();
    }

    public void InitializeDB()
    {
        Database.EnsureCreated();
        DbInitializer.SeedDatabase(this);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
     => options.UseSqlite($"Data Source={DbPath}");

}