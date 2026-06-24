using System.Text.Json.Serialization;

namespace SmartFitBook.MAUI.Models;

public class Exercise
{
    [JsonPropertyName("exerciseId")]
    public string ExerciseId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gifUrl")]
    public string GifUrl { get; set; } = string.Empty;

    [JsonPropertyName("targetMuscles")]
    public List<string> TargetMuscles { get; set; } = new();

    [JsonPropertyName("bodyParts")]
    public List<string> BodyParts { get; set; } = new();

    [JsonPropertyName("equipments")]
    public List<string> Equipments { get; set; } = new();

    [JsonPropertyName("secondaryMuscles")]
    public List<string> SecondaryMuscles { get; set; } = new();

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = new();

    // گرفتن نام فایل GIF از URL
    public string GifFileName => Path.GetFileName(GifUrl);
}