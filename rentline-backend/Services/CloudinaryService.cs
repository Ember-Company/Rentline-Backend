
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace rentline_backend.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloud;
    public CloudinaryService(IConfiguration cfg)
    {
        var account = new Account(cfg["Cloudinary:CloudName"], cfg["Cloudinary:ApiKey"], cfg["Cloudinary:ApiSecret"]);
        _cloud = new Cloudinary(account);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string? folder = "rentline")
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = folder
        };
        var res = await _cloud.UploadAsync(uploadParams);
        if (res.StatusCode != System.Net.HttpStatusCode.OK) throw new Exception("Cloudinary upload failed");
        return res.SecureUrl.ToString();
    }
}
