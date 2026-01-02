using Twc.Cli.Sdk;

namespace Twc.Cli.AppHost.Configuration;

/// <summary>
/// Collects configuration entries from plugins and merges them into a profile document.
/// </summary>
public static class PluginConfigSchemaCollector
{
    /// <summary>
    /// Applies default plugin config values to a profile document.
    /// </summary>
    public static void ApplyPluginDefaults<TCore>(ProfileDocument<TCore> doc, IEnumerable<ITwcCliPlugin> plugins)
        where TCore : class, new()
    {
        if (doc is null) throw new ArgumentNullException(nameof(doc));
        if (plugins is null) throw new ArgumentNullException(nameof(plugins));

        foreach (var plugin in plugins)
        {
            if (plugin is not IConfigSchemaContributor contributor)
                continue;

            var pluginId = plugin.Metadata.Id.ToString();
            if (!doc.PluginSettings.TryGetValue(pluginId, out var section))
            {
                section = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                doc.PluginSettings[pluginId] = section;
            }

            foreach (var entry in contributor.GetConfigEntries())
            {
                if (string.IsNullOrWhiteSpace(entry.Key))
                    continue;

                if (section.ContainsKey(entry.Key))
                    continue;

                section[entry.Key] = ToJsonCompatibleValue(entry.DefaultValue);
            }
        }
    }

    private static object? ToJsonCompatibleValue(ConfigValue value)
        => value switch
        {
            ConfigValue.String s => s.Value,
            ConfigValue.Boolean b => b.Value,
            ConfigValue.Integer i => i.Value,
            _ => null,
        };
}

