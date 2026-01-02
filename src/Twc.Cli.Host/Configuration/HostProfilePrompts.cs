using Spectre.Console;

namespace Twc.Cli.Host.Configuration;

/// <summary>
///     Interactive prompts for creating a <see cref="HostProfile" />.
/// </summary>
public static class HostProfilePrompts
{
    /// <summary>
    ///     Prompts the user for values to populate a <see cref="HostProfile" />.
    /// </summary>
    /// <param name="profileName">The name of the profile.</param>
    /// <returns>A <see cref="HostProfile" /> populated with the user's selections.</returns>
    public static HostProfile Prompt(string profileName)
    {
        var profile = new HostProfile { Name = profileName };

        profile.Environment = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select environment")
                .AddChoices(HostProfileTemplates.Environments));

        profile.ApiBaseUrl = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select API base URL")
                .AddChoices(HostProfileTemplates.ApiBaseUrls));

        profile.LogLevel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select log level")
                .AddChoices(HostProfileTemplates.LogLevels));

        return profile;
    }
}