using System.Text.Json.Serialization;

namespace SmartFitBook.MAUI.Models;

public class WorkoutPlan
{
    [JsonPropertyName("workoutId")]
    public string WorkoutId { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("traineeUsername")]
    public string TraineeUsername { get; set; } = string.Empty;

    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [JsonPropertyName("weeks")]
    public int Weeks { get; set; } = 4;

    [JsonPropertyName("weekDays")]
    public List<string> WeekDays { get; set; } = new();

    [JsonPropertyName("schedule")]
    public Dictionary<string, List<WorkoutDayItem>> Schedule { get; set; } = new();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}