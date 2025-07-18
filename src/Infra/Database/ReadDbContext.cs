using Application.Abstractions.Data;
using Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infra.Database;

public sealed class ReadDbContext : IReadDbContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<Book> Books => _database.GetCollection<Book>(name: "Books");
    public IMongoCollection<Loan> Loans => _database.GetCollection<Loan>(name: "Loans");
    
    public ReadDbContext(string connectionString, string databaseName)
    {
        ConfigureGuidSerialization();
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
    
    private static void ConfigureGuidSerialization()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Guid)))
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
    }
}
