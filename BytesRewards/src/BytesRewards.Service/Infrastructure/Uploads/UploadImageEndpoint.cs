using FastEndpoints;
using Microsoft.AspNetCore.Hosting;

namespace BytesRewards.Service.Infrastructure.Uploads;

/// <summary>
/// POST /uploads/image
/// Accepts multipart/form-data with field name "file".
/// Saves to {ContentRootPath}/wwwroot/uploads/ and returns { url: "/uploads/{filename}" }.
/// Allowed types: JPG, PNG GIF — max 2 MB.
/// </summary>
public sealed class UploadImageEndpoint(IWebHostEnvironment env)
    : EndpointWithoutRequest<UploadImageResponse>
{
    private static readonly HashSet<string> AllowedTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/jpg",
            "image/png", "image/gif"
        };

    private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

    public override void Configure()
    {
        Post("/uploads/image");
        Roles("employee", "manager", "admin");
        AllowFileUploads();
        Options(o => o.WithTags("11 - Uploads"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Access form file directly from HttpContext
        var formFile = HttpContext.Request.Form.Files.GetFile("file");

        if (formFile is null || formFile.Length == 0)
            throw new Exception("No file uploaded. Use field name 'file'.");

        if (!AllowedTypes.Contains(formFile.ContentType))
            throw new Exception("Only image files are allowed (JPG, PNG, GIF).");

        if (formFile.Length > MaxBytes)
            throw new Exception("Image must be under 2 MB.");

        // Use ContentRootPath — works in all environments (local, Aspire, Docker)
        var uploadsDir = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsDir);

        var ext      = Path.GetExtension(formFile.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await formFile.CopyToAsync(stream, ct);

        Response = new UploadImageResponse { Url = $"/uploads/{fileName}" };
    }
}

public sealed class UploadImageResponse
{
    public string Url { get; set; } = string.Empty;
}
