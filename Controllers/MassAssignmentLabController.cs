using Microsoft.AspNetCore.Mvc;
using VulnLab.NET.Models;

namespace VulnLab.NET.Controllers;

[Route("labs/mass-assignment")]
public class MassAssignmentLabController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/MassAssignment.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(ProfileUpdateRequest model)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["Username"] = model.Username ?? "-";
        ViewData["Email"] = model.Email ?? "-";
        ViewData["IsAdmin"] = model.IsAdmin;
        ViewData["Result"] = model.IsAdmin
            ? "Profil guncellendi. UYARI: IsAdmin=true baglandi, yetki yukseldi!"
            : "Profil guncellendi. IsAdmin=false.";
        return View("~/Views/Labs/MassAssignment.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(ProfileUpdateSecureRequest model)
    {
        // GUVENLI: IsAdmin bind edilmez; sunucu tarafinda sabit tutulur
        const bool isAdmin = false;

        ViewData["Mode"] = "Secure";
        ViewData["Username"] = model.Username ?? "-";
        ViewData["Email"] = model.Email ?? "-";
        ViewData["IsAdmin"] = isAdmin;
        ViewData["Result"] = "Profil guncellendi. IsAdmin istemciden kabul edilmedi (sunucu sabiti: false).";
        return View("~/Views/Labs/MassAssignment.cshtml");
    }
}
