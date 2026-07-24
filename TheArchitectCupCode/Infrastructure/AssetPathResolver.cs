using Godot;

namespace TheArchitectCup.Infrastructure;

internal static class AssetPathResolver
{
    internal static string ResolveExistingPath(params string[] candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        if (candidates.Length == 0)
            throw new ArgumentException("At least one asset path candidate is required.", nameof(candidates));

        foreach (string candidate in candidates)
        {
            if (ResourceLoader.Exists(candidate))
                return candidate;
        }

        return candidates[^1];
    }
}
