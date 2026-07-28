using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/csrf")]
public class CsrfLabController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/Csrf.cshtml");
    }

    [HttpPost("vulnerable-transfer")]
    public IActionResult VulnerableTransfer(string? toAccount, decimal amount)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["ToAccount"] = toAccount ?? "-";
        ViewData["Amount"] = amount;
        ViewData["Message"] = "Transfer istegi token dogrulamasi olmadan kabul edildi.";
        return View("~/Views/Labs/Csrf.cshtml");
    }

    [HttpPost("secure-transfer")]
    [ValidateAntiForgeryToken]
    public IActionResult SecureTransfer(string? toAccount, decimal amount)
    {
        ViewData["Mode"] = "Secure";
        ViewData["ToAccount"] = toAccount ?? "-";
        ViewData["Amount"] = amount;
        ViewData["Message"] = "Transfer istegi anti-forgery token ile dogrulandi.";
        return View("~/Views/Labs/Csrf.cshtml");
    }
}
