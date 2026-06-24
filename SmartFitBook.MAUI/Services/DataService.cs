using System.Text.Json;
using SmartFitBook.MAUI.Models;

namespace SmartFitBook.MAUI.Services;

public class DataService
{
    private List<Exercise>? _allExercises;
    private List<string>? _bodyParts;
    private List<string>? _equipments;
    private List<string>? _muscles;

    private async Task<string> ReadFileAsync(string fileName)
    {
        try
        {
            // روش 1: خواندن از MauiAsset
            var assetPath = Path.Combine("data", fileName);
            using var stream = await FileSystem.OpenAppPackageFileAsync(assetPath);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        catch
        {
            // روش 2: خواندن از AppDataDirectory (برای دیباگ)
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "data", fileName);
            if (File.Exists(appDataPath))
            {
                return await File.ReadAllTextAsync(appDataPath);
            }

            // روش 3: کپی از پروژه به AppData
            var sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", fileName);
            if (File.Exists(sourcePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(appDataPath)!);
                File.Copy(sourcePath, appDataPath, true);
                return await File.ReadAllTextAsync(appDataPath);
            }

            throw new FileNotFoundException($"فایل {fileName} یافت نشد");
        }
    }

    public async Task<List<Exercise>> GetAllExercisesAsync()
    {
        if (_allExercises != null) return _allExercises;

        try
        {
            var json = await ReadFileAsync("exercises.json");
            _allExercises = JsonSerializer.Deserialize<List<Exercise>>(json) ?? new();
            System.Diagnostics.Debug.WriteLine($"Loaded {_allExercises.Count} exercises");
            return _allExercises;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return new List<Exercise>();
        }
    }

    public async Task<List<string>> GetBodyPartsAsync()
    {
        if (_bodyParts != null) return _bodyParts;

        try
        {
            var json = await ReadFileAsync("bodyparts.json");
            var items = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            _bodyParts = items?.Select(x => x["name"]).ToList() ?? new();
            return _bodyParts;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task<List<string>> GetEquipmentsAsync()
    {
        if (_equipments != null) return _equipments;

        try
        {
            var json = await ReadFileAsync("equipments.json");
            var items = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            _equipments = items?.Select(x => x["name"]).ToList() ?? new();
            return _equipments;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task<List<string>> GetMusclesAsync()
    {
        if (_muscles != null) return _muscles;

        try
        {
            var json = await ReadFileAsync("muscles.json");
            var items = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            _muscles = items?.Select(x => x["name"]).ToList() ?? new();
            return _muscles;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return new List<string>();
        }
    }
    public async Task<List<string>> GetRealMusclesAsync()
    {
        var all = await GetAllExercisesAsync();
        var muscles = all
            .SelectMany(e => e.TargetMuscles)
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        System.Diagnostics.Debug.WriteLine($"Real muscles from exercises: {string.Join(", ", muscles.Take(20))}");
        return muscles;
    }
    public async Task<List<Exercise>> GetExercisesByMuscleAsync(string muscle, bool includeSecondary = false)
    {
        var all = await GetAllExercisesAsync();
        return all.Where(e => e.TargetMuscles.Contains(muscle, StringComparer.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<Exercise>> GetExercisesByEquipmentAsync(string equipment)
    {
        var all = await GetAllExercisesAsync();
        return all.Where(e => e.Equipments.Contains(equipment, StringComparer.OrdinalIgnoreCase)).ToList();
    }

    public async Task<Exercise?> GetExerciseByIdAsync(string exerciseId)
    {
        var all = await GetAllExercisesAsync();
        return all.FirstOrDefault(e => e.ExerciseId == exerciseId);
    }
}