using System.Reflection;
using Spectre.Console.Cli;
using Twc.Cli.Sdk;

namespace Twc.Cli.Framework.Plugins;

internal sealed class SpectreCommandRegistry : ICommandRegistry
{
    private readonly CommandTypeCatalog _commandTypes;
    private readonly IConfigurator _config;

    public SpectreCommandRegistry(IConfigurator config, CommandTypeCatalog commandTypes)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _commandTypes = commandTypes ?? throw new ArgumentNullException(nameof(commandTypes));
    }

    public void AddCommand(string name, Type commandType, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (commandType is null) throw new ArgumentNullException(nameof(commandType));

        if (!typeof(ICommand).IsAssignableFrom(commandType))
            throw new ArgumentException($"Command type '{commandType}' must implement {nameof(ICommand)}.",
                nameof(commandType));

        _commandTypes.Add(commandType);

        // Use Spectre's generic AddCommand<TCommand>(string name) on the concrete configurator type.
        var addCommandGeneric = _config
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.Name == nameof(IConfigurator.AddCommand)
                        && m.IsGenericMethodDefinition
                        && m.GetGenericArguments().Length == 1)
            .Select(m => new { Method = m, Params = m.GetParameters() })
            .Where(x => x.Params.Length == 1 && x.Params[0].ParameterType == typeof(string))
            .Select(x => x.Method)
            .Single();

        var builder = (ICommandConfigurator?)addCommandGeneric
            .MakeGenericMethod(commandType)
            .Invoke(_config, [name]);

        if (builder is null)
            throw new InvalidOperationException($"Failed to register command '{name}' of type '{commandType}'.");

        if (!string.IsNullOrWhiteSpace(description))
            builder.WithDescription(description);
    }

    public void AddBranch(string name, Action<ICommandRegistry> configure, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        _config.AddBranch(name, branch =>
        {
            if (!string.IsNullOrWhiteSpace(description))
                branch.SetDescription(description);

            // branch is IConfigurator<CommandSettings>; treat it as IConfigurator for our abstraction.
            configure(new SpectreCommandRegistry((IConfigurator)branch, _commandTypes));
        });
    }
}