using MongoDB.Driver;

namespace PassportInfoExtractor.Data.DataAccess;
public interface IDbConnection
{
    string DbName { get; }
    MongoClient Client { get; }
    IMongoCollection<Passport> PassportCollection { get; }
    string PassportCollectionName { get; }
    IMongoCollection<PassportGroup> GroupCollection { get; }
    string GroupCollectionName { get; }
}
