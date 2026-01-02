namespace Twc.Cli.Framework.Plugins;

/// <summary>
///     Captures command types registered into Spectre during startup so the host can add them to DI.
/// </summary>
public sealed class CommandTypeCatalog
{
    private readonly HashSet<Type> _commandTypes = [];

    /// <summary>
    ///     All registered command types.
    /// </summary>
    public IReadOnlyCollection<Type> CommandTypes => _commandTypes;

    internal void Add(Type commandType)
    {
        if (commandType is null) throw new ArgumentNullException(nameof(commandType));
        _commandTypes.Add(commandType);
    }
}