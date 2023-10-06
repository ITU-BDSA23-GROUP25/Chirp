namespace DBContext;

public class DatabaseContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Author> Authors { get; set; }

    public DataBaseContext() 
    {
        //var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = path.GetTempPath();
        DbPath = System.IO.Path.Join(path, "ChirpDB.db");
    }


}