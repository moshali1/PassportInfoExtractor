using PassportInfoExtractor.Data.Models;

namespace PassportInfoExtractor.Services;
public interface IDatabaseService
{
    Task<string> CreateGroup(PassportGroup group);
    Task<List<PassportGroup>> GetAllGroups();
    Task<List<Passport>> GetAllPassports();
    Task<PassportGroup> GetGroup(string id);
    Task<Passport> GetPassport(string id);
    Task<string> SavePassport(Passport passport);
}