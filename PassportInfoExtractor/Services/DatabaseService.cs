using PassportInfoExtractor.Data.DataAccess;
using PassportInfoExtractor.Data.Models;

namespace PassportInfoExtractor.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IPassportData _passportData;
    private readonly IPassportGroupData _groupData;

    public DatabaseService(IPassportData passportData, IPassportGroupData groupData)
    {
        _passportData = passportData;
        _groupData = groupData;
    }

    public async Task<string> SavePassport(Passport passport)
    {
        await _passportData.CreatePassport(passport);
        return passport.Id;
    }

    public async Task<Passport> GetPassport(string id)
    {
        return await _passportData.GetPassport(id);
    }

    public async Task<List<Passport>> GetAllPassports()
    {
        return await _passportData.GetPassports();
    }

    public async Task<string> CreateGroup(PassportGroup group)
    {
        await _groupData.CreateGroup(group);
        return group.Id;
    }

    public async Task<PassportGroup> GetGroup(string id)
    {
        return await _groupData.GetGroup(id);
    }

    public async Task<List<PassportGroup>> GetAllGroups()
    {
        return await _groupData.GetGroups();
    }
}