namespace Twc.Cli.Sdk;

/// <summary>
///     Marks an assembly as containing one or more TWC CLI plugins.
///     The host can use this for lightweight plugin discovery without arbitrary type scanning.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
#pragma warning disable IDE0051 // Reflection-based usage.
public sealed class TwcCliPluginAttribute : Attribute
{
    /// <summary>
    ///     Creates a new attribute.
    /// </summary>
    public TwcCliPluginAttribute(Type pluginType)
    {
        PluginType = pluginType;
    }

    /// <summary>
    ///     The plugin type implementing <see cref="ITwcCliPlugin" />.
    /// </summary>
    public Type PluginType { get; }
}
#pragma warning restore IDE0051