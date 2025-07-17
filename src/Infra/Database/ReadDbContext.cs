using Domain;
using MongoDB.Driver;

namespace Infra.Database;

public sealed class ReadDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<Book> Books => _database.GetCollection<Book>(name: "Books");
    public IMongoCollection<Loan> Loans => _database.GetCollection<Loan>(name: "Loans");
    
    public ReadDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
}
