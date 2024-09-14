using MongoDB.Driver;
using PassportInfoExtractor.Data.Models;
using System.Data;

namespace PassportInfoExtractor.Data.DataAccess;
public class PassportData : IPassportData
{
    private readonly IMongoCollection<Passport> _passports;

    public PassportData(IDbConnection db) =>
        _passports = db.PassportCollection;

    public async Task<List<Passport>> GetPassports() =>
        await _passports.Find(_ => true).ToListAsync();

    public async Task<Passport> GetPassport(string id) =>
        await _passports.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task CreatePassport(Passport passport) =>
        await _passports.InsertOneAsync(passport);

    public async Task UpdatePassport(Passport passport) =>
        await _passports.ReplaceOneAsync(p => p.Id == passport.Id, passport);

    public async Task DeletePassport(string id) =>
        await _passports.DeleteOneAsync(p => p.Id == id);
}
