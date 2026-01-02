namespace Twc.Cli.Sdk;

/// <summary>
///     Minimal command registration abstraction (host owns Spectre integration).
/// </summary>
public interface ICommandRegistry
{
    /// <summary>
    ///     Adds a leaf command.
    /// </summary>
    void AddCommand(string name, Type commandType, string? description = null);

    /// <summary>
    ///     Adds a branch command group.
    /// </summary>
    void AddBranch(string name, Action<ICommandRegistry> configure, string? description = null);
}