namespace SimpleDB;

interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(string dir, int? limit = null);
    public void Store(string dir, T record);
}