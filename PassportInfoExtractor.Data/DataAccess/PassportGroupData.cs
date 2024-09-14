using MongoDB.Driver;

namespace PassportInfoExtractor.Data.DataAccess;
public class PassportGroupData : IPassportGroupData
{
    private readonly IMongoCollection<PassportGroup> _groups;

    public PassportGroupData(IDbConnection db) =>
        _groups = db.GroupCollection;

    public async Task<List<PassportGroup>> GetGroups() =>
        await _groups.Find(_ => true).ToListAsync();

    public async Task<PassportGroup> GetGroup(string id) =>
        await _groups.Find(g => g.Id == id).FirstOrDefaultAsync();

    public async Task CreateGroup(PassportGroup group) =>
        await _groups.InsertOneAsync(group);

    public async Task UpdateGroup(PassportGroup group) =>
        await _groups.ReplaceOneAsync(g => g.Id == group.Id, group);

    public async Task DeleteGroup(string id) =>
        await _groups.DeleteOneAsync(g => g.Id == id);
}
