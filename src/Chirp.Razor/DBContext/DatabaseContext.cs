namespace Chirp.Razor;

public class DatabaseContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
    public string DbPath { get; set; }

    protected void OnConfiguring()
    {
        //var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.GetTempPath();
        DbPath = Path.Join(path, "ChirpDB.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
     => options.UseSqlite($"Data Source={DbPath}");

}