using System.Text.Json;

namespace SmartTaskerMini.Core.Application;

public static class ConfigurationManager
{
    private static readonly string ConfigFilePath = "appsettings.json";
    private static AppSettings? _settings;

    public static AppSettings Settings
    {
        get
        {
            if (_settings == null)
                LoadSettings();
            return _settings!;
        }
    }

    private static void LoadSettings()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            else
            {
                _settings = new AppSettings();
                SaveSettings();
            }
        }
        catch
        {
            _settings = new AppSettings();
        }
    }

    public static void SaveSettings()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(ConfigFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public static void SetStorageType(string storageType)
    {
        Settings.StorageType = storageType;
        SaveSettings();
    }
}

public class AppSettings
{
    public string StorageType { get; set; } = "SQL";
    public string JsonFilePath { get; set; } = "data/tasks.json";
    public string XmlFilePath { get; set; } = "data/tasks.xml";
}