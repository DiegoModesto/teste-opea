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
        BsonClassMap.RegisterClassMap<Book>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        });
        
        BsonClassMap.RegisterClassMap<Loan>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        });
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
}
