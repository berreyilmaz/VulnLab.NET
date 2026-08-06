using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/path-traversal")]
public class PathTraversalLabController : Controller
{
    private readonly string _labFilesRoot;
    private readonly string _appDataRoot;

    public PathTraversalLabController(IWebHostEnvironment env)
    {
        _appDataRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, "App_Data"));
        _labFilesRoot = Path.GetFullPath(Path.Combine(_appDataRoot, "lab-files"));
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/PathTraversal.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? file)
    {
        file ??= string.Empty;
        ViewData["Mode"] = "Vulnerable";
        ViewData["File"] = file;

        if (string.IsNullOrWhiteSpace(file))
        {
            ViewData["Result"] = "Dosya adi bos olamaz.";
            return View("~/Views/Labs/PathTraversal.cshtml");
        }

        // KASITLI ZAFİYET: kullanici girdisi dogrudan path'e eklenir
        var targetPath = Path.GetFullPath(Path.Combine(_labFilesRoot, file));
        ViewData["ResolvedPath"] = targetPath;

        if (!System.IO.File.Exists(targetPath))
        {
            ViewData["Result"] = "Dosya bulunamadi.";
            return View("~/Views/Labs/PathTraversal.cshtml");
        }

        ViewData["Content"] = System.IO.File.ReadAllText(targetPath);
        ViewData["Result"] = "Dosya okundu (path kontrolu yok).";
        return View("~/Views/Labs/PathTraversal.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? file)
    {
        file ??= string.Empty;
        ViewData["Mode"] = "Secure";
        ViewData["File"] = file;

        if (string.IsNullOrWhiteSpace(file))
        {
            ViewData["Result"] = "Dosya adi bos olamaz.";
            return View("~/Views/Labs/PathTraversal.cshtml");
        }

        if (file.Contains("..", StringComparison.Ordinal) ||
            file.Contains('/', StringComparison.Ordinal) ||
            file.Contains('\\', StringComparison.Ordinal))
        {
            ViewData["Result"] = "Reddedildi: path traversal karakterleri izinli degil.";
            return View("~/Views/Labs/PathTraversal.cshtml");
        }

        var targetPath = Path.GetFullPath(Path.Combine(_labFilesRoot, file));
        ViewData["ResolvedPath"] = targetPath;

        if (!targetPath.StartsWith(_labFilesRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(targetPath, _labFilesRoot, StringComparison.OrdinalIgnoreCase))
        {
            ViewData["Result"] = "Reddedildi: dosya lab-files kok dizini disinda.";
            return View("~/Views/Labs/PathTraversal.cshtml");
        }

        if (!System.IO.File.Exists(targetPath))
        {
            ViewData["Result"] = "Dosya bulunamadi.";
            return View("~/Views/Labs/PathTraversal.cshtml");
        }

        ViewData["Content"] = System.IO.File.ReadAllText(targetPath);
        ViewData["Result"] = "Dosya guvenli sinirlar icinde okundu.";
        return View("~/Views/Labs/PathTraversal.cshtml");
    }
}
