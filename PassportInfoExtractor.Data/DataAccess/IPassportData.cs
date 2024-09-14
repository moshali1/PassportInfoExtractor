namespace PassportInfoExtractor.Data.DataAccess;
public interface IPassportData
{
    Task<List<Passport>> GetPassports();
    Task<Passport> GetPassport(string id);
    Task CreatePassport(Passport passport);
    Task UpdatePassport(Passport passport);
    Task DeletePassport(string id);
}
