using System.Text.Json;

namespace Twc.Cli.AppHost.Configuration;

/// <summary>
/// Stores and bootstraps profile JSON files in the user's home directory.
/// Filenames are namespaced by the app name: {app}.{profile}.json
/// </summary>
public sealed class ProfileStore
{
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public string ProfilesRoot { get; }
    public string AppName { get; }

    public ProfileStore(string appName, string? profilesRoot = null)
    {
        if (string.IsNullOrWhiteSpace(appName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(appName));

        AppName = appName.Trim();
        ProfilesRoot = string.IsNullOrWhiteSpace(profilesRoot)
            ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            : profilesRoot;
    }

    public void EnsureDefaultProfiles(Func<string, object> defaultProfileFactory)
    {
        Directory.CreateDirectory(ProfilesRoot);
        EnsureProfileExists("dev", () => defaultProfileFactory("dev"));
        EnsureProfileExists("test", () => defaultProfileFactory("test"));
        EnsureProfileExists("prod", () => defaultProfileFactory("prod"));
    }

    public IReadOnlyList<string> ListProfiles(Func<JsonElement, bool> isProfileJson)
    {
        if (!Directory.Exists(ProfilesRoot))
            return [];

        var results = new List<string>();
        var pattern = $"{AppName}.*.json";

        foreach (var file in Directory.EnumerateFiles(ProfilesRoot, pattern, SearchOption.TopDirectoryOnly))
        {
            try
            {
                using var stream = File.OpenRead(file);
                using var doc = JsonDocument.Parse(stream);

                if (!isProfileJson(doc.RootElement))
                    continue;

                var profileName = TryExtractProfileName(file);
                if (string.IsNullOrWhiteSpace(profileName))
                    continue;

                results.Add(profileName);
            }
            catch
            {
                // Ignore unreadable/invalid JSON files.
            }
        }

        return results.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public string Save(string profileName, object profile, bool overwrite)
    {
        if (string.IsNullOrWhiteSpace(profileName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(profileName));
        if (profile is null) throw new ArgumentNullException(nameof(profile));

        var path = GetProfilePath(profileName);
        Directory.CreateDirectory(ProfilesRoot);

        if (!overwrite && File.Exists(path))
            throw new InvalidOperationException($"Profile '{profileName}' already exists: {path}");

        var json = JsonSerializer.Serialize(profile, _jsonOptions);
        File.WriteAllText(path, json);
        return path;
    }

    public string GetProfilePath(string profileName)
        => Path.Combine(ProfilesRoot, $"{AppName}.{SanitizeProfileName(profileName)}.json");

    private void EnsureProfileExists(string profileName, Func<object> create)
    {
        var path = GetProfilePath(profileName);
        if (File.Exists(path))
            return;

        Save(profileName, create(), overwrite: false);
    }

    private string? TryExtractProfileName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        if (!fileName.StartsWith(AppName + ".", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return null;

        var withoutApp = fileName[(AppName.Length + 1)..];
        var profile = withoutApp[..^5];

        return string.IsNullOrWhiteSpace(profile) ? null : profile;
    }

    private static string SanitizeProfileName(string profileName)
    {
        profileName = profileName.Trim();
        foreach (var c in Path.GetInvalidFileNameChars())
            profileName = profileName.Replace(c, '-');

        return profileName;
    }
}

