using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace PassportInfoExtractor.Services;

public class AzureBlobService
{
    private readonly string _storageAccountName;
    private readonly string _storageAccountKey;
    private readonly string _storageConnectionString;

    public AzureBlobService(IConfiguration config)
    {
        _storageAccountName = config["AzureStorage:AccountName"];
        _storageAccountKey = config["AzureStorage:AccountKey"];

        _storageConnectionString = config.GetConnectionString("Storage");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string uniqueFileName)
    {
        var connectionString = _storageConnectionString;
        var containerName = "24-umrahtrip-passports";
        var blobName = uniqueFileName;

        BlobClient blobClient = new BlobClient(connectionString, containerName, blobName);

        // Upload the file
        await blobClient.UploadAsync(fileStream, overwrite: true);

        // Generate SAS URL
        BlobSasBuilder sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey)).ToString();
        string sasUrl = $"{blobClient.Uri}?{sasToken}";

        return sasUrl;
    }
}
