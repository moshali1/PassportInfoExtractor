using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PassportInfoExtractor.Data.Models;

public class PassportGroup
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string GroupName { get; set; }
    public List<string> PassportIds { get; set; } = new();
}
