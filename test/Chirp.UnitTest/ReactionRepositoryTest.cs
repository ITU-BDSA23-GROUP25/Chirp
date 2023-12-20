using System.Diagnostics;
using System.Runtime.InteropServices;
using Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;

namespace ReactionRepository_test;


public class ReactionRepositoryTest
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly DatabaseContext _context;

    public ReactionRepositoryTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.Migrate();

        _reactionRepository = new ReactionRepository(_context);
        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);
    }
}