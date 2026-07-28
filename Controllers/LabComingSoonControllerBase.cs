using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

public abstract class LabComingSoonControllerBase : Controller
{
    protected IActionResult RenderComingSoon(string labTitle)
    {
        ViewData["LabTitle"] = labTitle;
        return View("~/Views/Shared/LabComingSoon.cshtml");
    }
}
