using Azure;
using Azure.AI.DocumentIntelligence;

namespace PassportInfoExtractor.Services;

public class DocumentIntelligenceService : IDocumentIntelligenceService
{
    private readonly string _endpoint;
    private readonly string _key;
    private readonly DocumentIntelligenceClient _client;

    // https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/concept-id-document?view=doc-intel-4.0.0
    private const string MODEL_ID = "prebuilt-idDocument";

    public DocumentIntelligenceService(IConfiguration configuration)
    {
        _endpoint = configuration["AzureDocumentIntelligence:Endpoint"];
        _key = configuration["AzureDocumentIntelligence:Key"];
        AzureKeyCredential credential = new AzureKeyCredential(_key);
        _client = new DocumentIntelligenceClient(new Uri(_endpoint), credential);
    }

    public async Task<Passport> AnalyzePassportAsync(Stream documentStream, string fileName)
    {
        try
        {
            Console.WriteLine($"Analyzing file: {fileName}");

            // Convert the stream to a Base64 string within the service method
            var base64FileContent = await ConvertStreamToBase64Async(documentStream);

            var content = new AnalyzeDocumentContent() { Base64Source = BinaryData.FromString(base64FileContent) };

            Console.WriteLine("Sending request to Azure Document Intelligence...");
            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, MODEL_ID, content);

            Console.WriteLine("Request completed. Extracting results...");
            AnalyzeResult result = operation.Value;

            return ExtractPassportInfo(result);
        }
        catch (RequestFailedException ex) when (ex.Status == 400 && ex.ErrorCode == "InvalidContent")
        {
            Console.WriteLine($"File format or content is invalid: {ex.Message}");
            throw new InvalidOperationException("The file format is unsupported or the file is corrupted.", ex);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Azure request failed: {ex.Status}, {ex.ErrorCode}, {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }


    private async Task<string> ConvertStreamToBase64Async(Stream stream)
    {
        // Ensure the stream is at the beginning
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using (var ms = new MemoryStream())
        {
            await stream.CopyToAsync(ms);
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    public async Task<Passport> AnalyzePassportFromUrlAsync(string url)
    {
        try
        {
            var content = new AnalyzeDocumentContent() { UrlSource = new Uri(url) };

            Console.WriteLine("Sending request to Azure Document Intelligence...");
            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, MODEL_ID, content);

            Console.WriteLine("Request completed. Extracting results...");
            AnalyzeResult result = operation.Value;

            return ExtractPassportInfo(result);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Azure request failed: {ex.Status}, {ex.ErrorCode}, {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    private Passport ExtractPassportInfo(AnalyzeResult result)
    {
        var passport = new Passport();

        if (result.Documents.Count > 0)
        {
            AnalyzedDocument document = result.Documents[0];
            foreach (var item in document.Fields)
            {
                ExtractDocumentField(item.Key, item.Value, passport);
            }
        }

        // After extracting FirstName and MiddleName
        if (string.IsNullOrWhiteSpace(passport.MiddleName) && !string.IsNullOrWhiteSpace(passport.FirstName))
        {
            var nameParts = passport.FirstName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length > 1)
            {
                passport.FirstName = nameParts[0];
                passport.MiddleName = string.Join(" ", nameParts.Skip(1));
                passport.MiddleNameConfidence = 0;
                passport.IsMiddleNameDerived = true;
            }
        }
        else
        {
            passport.IsMiddleNameDerived = false;
        }


        // Remove all whitespace (including newlines) for comparison
        string cleanedAuthority = new string(passport.IssuingAuthority?.Where(c => !char.IsWhiteSpace(c)).ToArray());

        if (cleanedAuthority == "UnitedStatesDepartmentofState")
        {
            passport.IssuingAuthority = "United States Department of State";
        }

        return passport;
    }

    private void ExtractDocumentField(string key, DocumentField docField, Passport passport)
    {
        string valueStr = ExtractValueFromDocumentField(docField);
        double confidence = docField.Confidence ?? 0.0;

        switch (key.ToLower())
        {
            case "documentnumber":
                passport.DocumentNumber = valueStr;
                passport.DocumentNumberConfidence = confidence;
                break;
            case "firstname":
                passport.FirstName = valueStr;
                passport.FirstNameConfidence = confidence;
                break;
            case "middlename":
                passport.MiddleName = valueStr;
                passport.MiddleNameConfidence = confidence;
                break;
            case "lastname":
                passport.LastName = valueStr;
                passport.LastNameConfidence = confidence;
                break;
            case "aliases":
                passport.Aliases = valueStr.Split(',').Select(a => a.Trim()).ToList();
                passport.AliasesConfidence = confidence;
                break;
            case "dateofbirth":
                if (docField.ValueDate.HasValue)
                {
                    passport.DateOfBirth = docField.ValueDate.Value.DateTime.ToUniversalTime();
                    passport.DateOfBirthConfidence = confidence;
                }
                break;
            case "dateofexpiration":
                if (docField.ValueDate.HasValue)
                {
                    passport.DateOfExpiration = docField.ValueDate.Value.DateTime.ToUniversalTime();
                    passport.DateOfExpirationConfidence = confidence;
                }
                break;
            case "dateofissue":
                if (docField.ValueDate.HasValue)
                {
                    passport.DateOfIssue = docField.ValueDate.Value.DateTime.ToUniversalTime();
                    passport.DateOfIssueConfidence = confidence;
                }
                break;
            case "sex":
                passport.Sex = valueStr;
                passport.SexConfidence = confidence;
                break;
            case "countryregion":
                passport.CountryRegion = valueStr;
                passport.CountryRegionConfidence = confidence;
                break;
            case "documenttype":
                passport.DocumentType = valueStr;
                passport.DocumentTypeConfidence = confidence;
                break;
            case "nationality":
                passport.Nationality = valueStr;
                passport.NationalityConfidence = confidence;
                break;
            case "placeofbirth":
                passport.PlaceOfBirth = valueStr;
                passport.PlaceOfBirthConfidence = confidence;
                break;
            case "placeofissue":
                passport.PlaceOfIssue = valueStr;
                passport.PlaceOfIssueConfidence = confidence;
                break;
            case "issuingauthority":
                passport.IssuingAuthority = valueStr;
                passport.IssuingAuthorityConfidence = confidence;
                break;
            case "personalnumber":
                passport.PersonalNumber = valueStr;
                passport.PersonalNumberConfidence = confidence;
                break;
            case "machinereadablezone":
                passport.MachineReadableZone = valueStr;
                passport.MachineReadableZoneConfidence = confidence;
                break;
            case "address":
                passport.Address = valueStr;
                passport.AddressConfidence = confidence;
                break;
            default:
                Console.WriteLine($"Unhandled field: {key}");
                break;
        }

        // Store the confidence in the Accuracy dictionary
        passport.Accuracy[key] = confidence;
    }

    private string ExtractValueFromDocumentField(DocumentField docField)
    {
        if (docField == null)
        {
            return string.Empty;
        }
        if (docField.Type == DocumentFieldType.String)
        {
            return docField.ValueString ?? string.Empty;
        }
        else if (docField.Type == DocumentFieldType.Date)
        {
            return docField.ValueDate?.ToUniversalTime().ToString("yyyy-MM-dd") ?? string.Empty;
        }
        else if (docField.Type == DocumentFieldType.Time)
        {
            return docField.ValueTime?.ToString("HH:mm:ss") ?? string.Empty;
        }
        else if (docField.Type == DocumentFieldType.Address)
        {
            var address = docField.ValueAddress;
            if (address != null)
            {
                var addressParts = new[]
                {
                address.HouseNumber,
                address.Road,
                address.City,
                address.State,
                address.PostalCode,
                address.CountryRegion
            };
                return string.Join(", ", addressParts.Where(part => !string.IsNullOrWhiteSpace(part)));
            }
            return string.Empty;
        }
        else if (docField.Type == DocumentFieldType.CountryRegion)
        {
            return docField.ValueCountryRegion ?? string.Empty;
        }
        else if (docField.Type == DocumentFieldType.SelectionMark)
        {
            return docField.ValueSelectionMark?.ToString() ?? string.Empty;
        }
        else if (docField.Type == DocumentFieldType.Dictionary)
        {
            return docField.ValueDictionary != null
                ? string.Join(", ", docField.ValueDictionary.Select(kv => $"{kv.Key}: {ExtractValueFromDocumentField(kv.Value)}"))
                : string.Empty;
        }
        else if (docField.Type == DocumentFieldType.List)
        {
            return docField.ValueList != null
                ? string.Join(", ", docField.ValueList.Select(ExtractValueFromDocumentField))
                : string.Empty;
        }
        else
        {
            return docField.Content ?? string.Empty;
        }
    }
}