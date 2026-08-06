namespace VulnLab.NET.Models;

public class ProfileUpdateRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
}

public class ProfileUpdateSecureRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
}
