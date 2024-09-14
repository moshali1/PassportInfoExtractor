
namespace PassportInfoExtractor.Services;

public interface IDocumentIntelligenceService
{
    Task<Passport> AnalyzePassportAsync(Stream documentStream, string fileName);
    Task<Passport> AnalyzePassportFromUrlAsync(string url);
}