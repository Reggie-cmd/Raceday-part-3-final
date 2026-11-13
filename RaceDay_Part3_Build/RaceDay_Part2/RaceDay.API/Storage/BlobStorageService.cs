using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Storage;

public sealed class BlobStorageService(IConfiguration configuration)
{
    public async Task<string> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0) throw new InvalidOperationException("The selected file is empty.");
        if (file.Length > 5 * 1024 * 1024) throw new InvalidOperationException("Images must be 5 MB or smaller.");
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase)) throw new InvalidOperationException("Only JPG, PNG and WEBP images are accepted.");
        var connection = configuration.GetConnectionString("AzureBlobStorage");
        if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Azure Blob Storage is not configured. Set ConnectionStrings:AzureBlobStorage.");
        var service = new BlobServiceClient(connection);
        var container = service.GetBlobContainerClient(configuration["AzureBlobStorage:Container"] ?? "raceday-images");
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blob = container.GetBlobClient($"{folder}/{Guid.NewGuid():N}{extension}");
        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: cancellationToken);
        return blob.Uri.ToString();
    }
}
