namespace VulnLab.NET.Models;

public class LabDefinition
{
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public string Difficulty { get; set; } = "Beginner";
    public string? Route { get; set; }
}
