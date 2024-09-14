using PassportInfoExtractor.Data.Models;

namespace PassportInfoExtractor.Data.DataAccess;
public interface IPassportGroupData
{
    Task<List<PassportGroup>> GetGroups();
    Task<PassportGroup> GetGroup(string id);
    Task CreateGroup(PassportGroup group);
    Task UpdateGroup(PassportGroup group);
    Task DeleteGroup(string id);
}
