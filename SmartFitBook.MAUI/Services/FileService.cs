namespace SmartFitBook.MAUI.Services;

public class FileService
{
    // ذخیره فایل در حافظه گوشی
    public async Task<string> SaveFileAsync(string fileName, string content)
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, fileName);
        await File.WriteAllTextAsync(path, content);
        return path;
    }

    // خواندن فایل از حافظه
    public async Task<string> ReadFileAsync(string filePath)
    {
        if (File.Exists(filePath))
            return await File.ReadAllTextAsync(filePath);
        return string.Empty;
    }

    // انتخاب فایل از حافظه گوشی (برای ایمپورت)
    public async Task<string?> PickJsonFileAsync()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select workout file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } }
                })
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error picking file: {ex.Message}");
        }
        return null;
    }

    // اشتراک‌گذاری فایل
    public async Task ShareFileAsync(string filePath, string title)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = title,
            File = new ShareFile(filePath)
        });
    }
}