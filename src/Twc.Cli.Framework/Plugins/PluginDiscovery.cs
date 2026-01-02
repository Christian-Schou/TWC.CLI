using System.Reflection;

namespace Twc.Cli.Framework.Plugins;

/// <summary>
///     Helpers for discovering plugin assemblies.
/// </summary>
public static class PluginDiscovery
{
    /// <summary>
    ///     Returns candidate plugin assemblies from the current <see cref="AppDomain" />.
    ///     v1 discovery strategy: only considers assemblies already loaded into the current process
    ///     (e.g., referenced NuGet packages).
    /// </summary>
    public static Assembly[] GetLoadedAssemblies()
    {
        return GetLoadedAssemblies(static _ => true);
    }

    /// <summary>
    ///     Returns candidate plugin assemblies from the current <see cref="AppDomain" />,
    ///     filtered by a caller-provided predicate.
    /// </summary>
    public static Assembly[] GetLoadedAssemblies(Func<Assembly, bool> predicate)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));

        return SafeGetAssemblies()
            .Where(IsCandidatePluginAssembly)
            .Where(predicate)
            .ToArray();
    }

    private static IEnumerable<Assembly> SafeGetAssemblies()
    {
        try
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
        catch
        {
            // Extremely defensive: this shouldn't happen in normal .NET execution,
            // but we treat discovery as best-effort.
            return Array.Empty<Assembly>();
        }
    }

    private static bool IsCandidatePluginAssembly(Assembly assembly)
    {
        // Skip dynamic assemblies.
        if (assembly.IsDynamic)
            return false;

        // Some runtime-generated assemblies may throw when accessing Location.
        // We use Location access as a cheap filter that also avoids exceptions later.
        try
        {
            _ = assembly.Location;
        }
        catch
        {
            return false;
        }

        return true;
    }
}