using System.Text.Json.Serialization;

namespace SmartFitBook.MAUI.Models;

public class WorkoutDayItem
{
    [JsonPropertyName("exerciseId")]
    public string ExerciseId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gifUrl")]
    public string GifUrl { get; set; } = string.Empty;

    [JsonPropertyName("sets")]
    public int Sets { get; set; } = 3;

    [JsonPropertyName("reps")]
    public string Reps { get; set; } = "10-12";

    [JsonPropertyName("restSeconds")]
    public int RestSeconds { get; set; } = 60;

    [JsonPropertyName("trainerNote")]
    public string? TrainerNote { get; set; }

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = new();
    // اضافه کردن این خاصیت
    public string GifFileName => Path.GetFileName(GifUrl);
    // برای نمایش در UI
    public string SetsRepsDisplay => $"{Sets} set × {Reps} reps";
}