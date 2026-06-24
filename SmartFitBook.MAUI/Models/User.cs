namespace SmartFitBook.MAUI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Trainee"; // Trainer or Trainee
    public string? TrainerUsername { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}