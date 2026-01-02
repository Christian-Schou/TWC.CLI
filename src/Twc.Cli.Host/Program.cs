using Twc.Cli.AppHost;
using Twc.Cli.Host;
using Twc.Cli.Host.Configuration;
using Twc.Cli.Sdk;

var info = new HostAppInfo(
    HostInfo.ApplicationName,
    "twc.json",
    "TWC_");

return HostApp.Run(args, info, new HostAppOptions
{
    HostVersion = new SemanticVersion(0, 1, 0),

    // v1: referenced NuGet packages
    EnablePlugins = true,

    // Enable built-in config wizard. This is the only host-specific bit:
    // how to create/prompt for your core profile model.
    ConfigWizard = new ConfigWizardOptions
    {
        CoreType = typeof(HostProfile),
        CreateDefaultCore = (Func<string, HostProfile>)(name => new HostProfile { Name = name }),
        PromptForCore = (Func<string, HostProfile>)(name => HostProfilePrompts.Prompt(name))
    }
});