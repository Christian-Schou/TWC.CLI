using System.Text.Json;

namespace Twc.Cli.Host.Configuration;

/// <summary>
///     Stores and bootstraps profile JSON files in the user's home directory.
/// </summary>
public sealed class ProfileStore
{
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    ///     Creates a profile store.
    /// </summary>
    /// <param name="profilesRoot">
    ///     Directory where profiles should be stored.
    ///     If null, defaults to the user's home directory.
    /// </param>
    /// <param name="appName">
    ///     Application name used as a filename prefix.
    ///     If null or whitespace, defaults to "twc".
    /// </param>
    public ProfileStore(string? profilesRoot = null, string appName = "twc")
    {
        ProfilesRoot = string.IsNullOrWhiteSpace(profilesRoot)
            ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            : profilesRoot;

        AppName = string.IsNullOrWhiteSpace(appName) ? "twc" : appName.Trim();
    }

    /// <summary>
    ///     Gets the root directory where profiles are stored.
    /// </summary>
    public string ProfilesRoot { get; }

    /// <summary>
    ///     Application name used as a filename prefix (e.g., twc.dev.json).
    /// </summary>
    public string AppName { get; }

    /// <summary>
    ///     Ensures the default profiles (dev/test/prod) exist.
    /// </summary>
    public void EnsureDefaultProfiles()
    {
        Directory.CreateDirectory(ProfilesRoot);

        EnsureProfileExists("dev", p =>
        {
            p.Name = "dev";
            p.Environment = "dev";
            p.ApiBaseUrl = "https://api.example.local";
        });

        EnsureProfileExists("test", p =>
        {
            p.Name = "test";
            p.Environment = "test";
            p.ApiBaseUrl = "https://api.test.example.com";
        });

        EnsureProfileExists("prod", p =>
        {
            p.Name = "prod";
            p.Environment = "prod";
            p.ApiBaseUrl = "https://api.example.com";
        });
    }

    /// <summary>
    ///     Gets available profile names (based on *.json files in the store).
    /// </summary>
    public IReadOnlyList<string> ListProfiles()
    {
        if (!Directory.Exists(ProfilesRoot))
            return [];

        var results = new List<string>();
        var pattern = $"{AppName}.*.json";

        foreach (var file in Directory.EnumerateFiles(ProfilesRoot, pattern, SearchOption.TopDirectoryOnly))
            try
            {
                using var stream = File.OpenRead(file);
                using var doc = JsonDocument.Parse(stream);

                if (!IsHostProfileJson(doc.RootElement))
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

        return results
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool IsHostProfileJson(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
            return false;

        // Minimal shape validation. We treat { Name, Environment, ApiBaseUrl, LogLevel } as a profile.
        return root.TryGetProperty("Name", out _)
               && root.TryGetProperty("Environment", out _)
               && root.TryGetProperty("ApiBaseUrl", out _)
               && root.TryGetProperty("LogLevel", out _);
    }

    /// <summary>
    ///     Writes a profile to disk.
    /// </summary>
    public string Save(string profileName, HostProfile profile, bool overwrite)
    {
        if (string.IsNullOrWhiteSpace(profileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(profileName));
        if (profile is null) throw new ArgumentNullException(nameof(profile));

        var path = GetProfilePath(profileName);
        Directory.CreateDirectory(ProfilesRoot);

        if (!overwrite && File.Exists(path))
            throw new InvalidOperationException($"Profile '{profileName}' already exists: {path}");

        var json = JsonSerializer.Serialize(profile, _jsonOptions);
        File.WriteAllText(path, json);
        return path;
    }

    /// <summary>
    ///     Gets the file path for a named profile.
    /// </summary>
    public string GetProfilePath(string profileName)
    {
        return Path.Combine(ProfilesRoot, $"{AppName}.{SanitizeProfileName(profileName)}.json");
    }

    private string? TryExtractProfileName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        // Expect: {AppName}.{profile}.json
        if (!fileName.StartsWith(AppName + ".", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return null;

        var withoutApp = fileName[(AppName.Length + 1)..];
        var profile = withoutApp[..^5]; // trim .json

        return string.IsNullOrWhiteSpace(profile) ? null : profile;
    }

    private void EnsureProfileExists(string profileName, Action<HostProfile> configure)
    {
        var path = GetProfilePath(profileName);
        if (File.Exists(path))
            return;

        var profile = new HostProfile();
        configure(profile);

        Save(profileName, profile, false);
    }

    private static string SanitizeProfileName(string profileName)
    {
        profileName = profileName.Trim();
        foreach (var c in Path.GetInvalidFileNameChars())
            profileName = profileName.Replace(c, '-');

        return profileName;
    }
}