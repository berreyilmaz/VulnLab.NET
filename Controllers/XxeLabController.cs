using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace VulnLab.NET.Controllers;

[Route("labs/xxe")]
public class XxeLabController : Controller
{
    private const string SafeDefaultXml = "<note><to>alice</to><message>Hello</message></note>";

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewData["DefaultXml"] = SafeDefaultXml;
        return View("~/Views/Labs/Xxe.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? xmlInput)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["InputXml"] = xmlInput ?? string.Empty;
        ViewData["DefaultXml"] = SafeDefaultXml;
        ViewData["Result"] = ParseXml(xmlInput, secureMode: false);
        return View("~/Views/Labs/Xxe.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? xmlInput)
    {
        ViewData["Mode"] = "Secure";
        ViewData["InputXml"] = xmlInput ?? string.Empty;
        ViewData["DefaultXml"] = SafeDefaultXml;
        ViewData["Result"] = ParseXml(xmlInput, secureMode: true);
        return View("~/Views/Labs/Xxe.cshtml");
    }

    private static string ParseXml(string? xmlInput, bool secureMode)
    {
        if (string.IsNullOrWhiteSpace(xmlInput))
        {
            return "XML bos olamaz.";
        }

        var hasDtd = xmlInput.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase);
        var hasEntity = xmlInput.Contains("<!ENTITY", StringComparison.OrdinalIgnoreCase)
                        || xmlInput.Contains("&xxe;", StringComparison.OrdinalIgnoreCase);

        if (secureMode && (hasDtd || hasEntity))
        {
            return "Reddedildi (secure): DTD/external entity icerigi tespit edildi.";
        }

        try
        {
            using var stringReader = new StringReader(xmlInput);
            XmlReaderSettings settings;

            if (secureMode)
            {
                settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };
            }
            else
            {
                settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    XmlResolver = new XmlUrlResolver()
                };
            }

            using var xmlReader = XmlReader.Create(stringReader, settings);
            var document = XDocument.Load(xmlReader, LoadOptions.None);
            var normalized = document.ToString(SaveOptions.DisableFormatting);

            if (!secureMode && (hasDtd || hasEntity))
            {
                return "Vulnerable parse tamamlandi: DTD/external entity icerigi kabul edildi (simulated risk).";
            }

            return $"XML parse basarili: {normalized}";
        }
        catch (Exception ex)
        {
            return $"Parse hatasi: {ex.Message}";
        }
    }
}
