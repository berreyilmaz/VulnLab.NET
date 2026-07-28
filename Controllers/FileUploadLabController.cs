using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/file-upload")]
public class FileUploadLabController : Controller
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".pdf"
    };

    private const long MaxFileSizeBytes = 2 * 1024 * 1024;

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/FileUpload.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(IFormFile? uploadFile)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["Result"] = uploadFile is null
            ? "Dosya secilmedi."
            : $"Dosya kabul edildi: {uploadFile.FileName} ({uploadFile.Length} bytes).";
        return View("~/Views/Labs/FileUpload.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(IFormFile? uploadFile)
    {
        ViewData["Mode"] = "Secure";

        if (uploadFile is null)
        {
            ViewData["Result"] = "Dosya secilmedi.";
            return View("~/Views/Labs/FileUpload.cshtml");
        }

        var extension = Path.GetExtension(uploadFile.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            ViewData["Result"] = $"Reddedildi: {extension} uzantisi izinli degil.";
            return View("~/Views/Labs/FileUpload.cshtml");
        }

        if (uploadFile.Length <= 0 || uploadFile.Length > MaxFileSizeBytes)
        {
            ViewData["Result"] = "Reddedildi: dosya boyutu gecersiz veya limit disi.";
            return View("~/Views/Labs/FileUpload.cshtml");
        }

        ViewData["Result"] = $"Dosya guvenli kontrollerden gecti: {uploadFile.FileName} ({uploadFile.Length} bytes).";
        return View("~/Views/Labs/FileUpload.cshtml");
    }
}
