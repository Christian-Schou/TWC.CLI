using System.Reflection;
using Twc.Cli.Sdk;

namespace Twc.Cli.Framework.Plugins;

/// <summary>
///     Explicit plugin catalog (no runtime scanning in v1).
/// </summary>
public sealed class PluginCatalog
{
    private readonly List<ITwcCliPlugin> _plugins = [];

    /// <summary>
    ///     Currently registered plugins.
    /// </summary>
    public IReadOnlyList<ITwcCliPlugin> Plugins => _plugins;

    /// <summary>
    ///     Adds a plugin instance to the catalog.
    /// </summary>
    public PluginCatalog Add(ITwcCliPlugin plugin)
    {
        if (plugin is null) throw new ArgumentNullException(nameof(plugin));
        _plugins.Add(plugin);
        return this;
    }

    /// <summary>
    ///     Convenience method to enumerate plugins.
    /// </summary>
    public IEnumerable<ITwcCliPlugin> EnumeratePlugins()
    {
        return _plugins;
    }

    /// <summary>
    ///     Adds all plugins declared via <see cref="TwcCliPluginAttribute" /> on the given assemblies.
    /// </summary>
    public PluginCatalog AddFromAssemblies(params Assembly[] assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));

        foreach (var asm in assemblies)
        foreach (var type in asm.GetCustomAttributes<TwcCliPluginAttribute>().Select(a => a.PluginType))
        {
            if (!typeof(ITwcCliPlugin).IsAssignableFrom(type))
                continue;

            if (Activator.CreateInstance(type) is ITwcCliPlugin plugin)
                Add(plugin);
        }

        return this;
    }
}