using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/open-redirect")]
public class OpenRedirectLabController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/OpenRedirect.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? returnUrl)
    {
        returnUrl ??= string.Empty;
        ViewData["Mode"] = "Vulnerable";
        ViewData["ReturnUrl"] = returnUrl;

        // KASITLI ZAFİYET: kullanici girdisi dogrudan redirect
        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            return Redirect(returnUrl);
        }

        ViewData["Message"] = "returnUrl bos; yonlendirme yapilmadi.";
        return View("~/Views/Labs/OpenRedirect.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? returnUrl)
    {
        returnUrl ??= string.Empty;
        ViewData["Mode"] = "Secure";
        ViewData["ReturnUrl"] = returnUrl;

        // GUVENLI: yalnizca local URL
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        ViewData["Message"] = "Reddedildi: returnUrl local degil. Ornek gecerli deger: /labs/xss";
        return View("~/Views/Labs/OpenRedirect.cshtml");
    }
}