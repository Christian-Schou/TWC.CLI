using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Spectre.Console.Cli;
using Twc.Cli.Framework.Plugins;
using Twc.Cli.Sdk;
using Xunit;

namespace Twc.Cli.Framework.Tests.Plugins;

/// <summary>
/// Tests for plugin loading and registration.
/// </summary>
public sealed class PluginLoaderTests
{
    /// <summary>
    /// Ensures the loader calls into the plugin's entrypoints.
    /// </summary>
    [Fact]
    public void RegisterAll_should_invoke_plugin_service_and_command_entrypoints_in_order()
    {
        // Arrange
        var plugin = Substitute.For<ITwcCliPlugin>();
        plugin.Metadata.Returns(new PluginMetadata(
            new PluginId("test.plugin"),
            new PluginDisplayName("Test Plugin"),
            new SemanticVersion(1, 0, 0),
            VersionRange.Any));

        var callOrder = new List<string>();
        plugin
            .When(p => p.RegisterServices(Arg.Any<IServiceRegistry>()))
            .Do(_ => callOrder.Add("services"));
        plugin
            .When(p => p.RegisterCommands(Arg.Any<ICommandRegistry>()))
            .Do(_ => callOrder.Add("commands"));

        var catalog = new PluginCatalog().Add(plugin);
        var services = new ServiceCollection();
        var config = Substitute.For<IConfigurator>();

        // Act
        PluginLoader.RegisterAll(catalog, services, config, new SemanticVersion(0, 1, 0));

        // Assert
        plugin.Received(1).RegisterServices(Arg.Any<IServiceRegistry>());
        plugin.Received(1).RegisterCommands(Arg.Any<ICommandRegistry>());
        callOrder.ShouldBe(new[] { "services", "commands" });
    }
}
