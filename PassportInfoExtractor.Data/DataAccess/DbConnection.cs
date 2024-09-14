using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace PassportInfoExtractor.Data.DataAccess;
public class DbConnection : IDbConnection
{
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private const string _connectionId = "MongoDB";

    public string DbName { get; private set; }
    public string PassportCollectionName { get; private set; } = "passports";
    public string GroupCollectionName { get; private set; } = "groups";
    public MongoClient Client { get; private set; }
    public IMongoCollection<Passport> PassportCollection { get; private set; }
    public IMongoCollection<PassportGroup> GroupCollection { get; private set; }

    public DbConnection(IConfiguration config)
    {
        _config = config;
        Client = new MongoClient(_config.GetConnectionString(_connectionId));
        DbName = _config["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        PassportCollection = _db.GetCollection<Passport>(PassportCollectionName);
        GroupCollection = _db.GetCollection<PassportGroup>(GroupCollectionName);
    }
}
