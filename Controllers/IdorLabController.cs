using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/idor")]
public class IdorLabController : Controller
{
    private static readonly Dictionary<int, (string Owner, string Content)> Documents = new()
    {
        [1001] = ("alice", "Alice maas dokumu"),
        [1002] = ("bob", "Bob performans notu"),
        [1003] = ("charlie", "Charlie IK kaydi")
    };

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/Idor.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(int documentId)
    {
        const string currentUser = "bob";
        ViewData["Mode"] = "Vulnerable";
        ViewData["CurrentUser"] = currentUser;
        ViewData["RequestedId"] = documentId;

        if (Documents.TryGetValue(documentId, out var doc))
        {
            ViewData["Result"] = $"Belge bulundu: {doc.Content} (Owner: {doc.Owner})";
        }
        else
        {
            ViewData["Result"] = "Belge bulunamadi.";
        }

        return View("~/Views/Labs/Idor.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(int documentId)
    {
        const string currentUser = "bob";
        ViewData["Mode"] = "Secure";
        ViewData["CurrentUser"] = currentUser;
        ViewData["RequestedId"] = documentId;

        if (!Documents.TryGetValue(documentId, out var doc))
        {
            ViewData["Result"] = "Belge bulunamadi.";
            return View("~/Views/Labs/Idor.cshtml");
        }

        if (!string.Equals(doc.Owner, currentUser, StringComparison.OrdinalIgnoreCase))
        {
            ViewData["Result"] = "Erisim reddedildi: bu belge size ait degil.";
            return View("~/Views/Labs/Idor.cshtml");
        }

        ViewData["Result"] = $"Belge bulundu: {doc.Content} (Owner: {doc.Owner})";
        return View("~/Views/Labs/Idor.cshtml");
    }
}
