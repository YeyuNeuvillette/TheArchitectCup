using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Api;

namespace TheArchitectCup.Features.CardGeneration;

internal static class CrossCharacterCardFilterRegistry
{
    private static readonly SortedDictionary<string, Func<CrossCharacterCardFilterContext, CardModel, bool>> Filters =
        new(StringComparer.Ordinal);
    private static readonly HashSet<string> ReportedFailures = new(StringComparer.Ordinal);
    private static readonly object SyncRoot = new();

    internal static IDisposable Register(
        string registrationId,
        Func<CrossCharacterCardFilterContext, CardModel, bool> filter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registrationId);
        ArgumentNullException.ThrowIfNull(filter);

        lock (SyncRoot)
        {
            if (!Filters.TryAdd(registrationId, filter))
                throw new InvalidOperationException($"A cross-character card filter named '{registrationId}' is already registered.");
        }

        return new Registration(registrationId);
    }

    internal static bool Allows(CrossCharacterCardFilterContext context, CardModel candidate)
    {
        KeyValuePair<string, Func<CrossCharacterCardFilterContext, CardModel, bool>>[] snapshot;
        lock (SyncRoot)
            snapshot = Filters.ToArray();

        foreach ((string registrationId, var filter) in snapshot)
        {
            try
            {
                if (!filter(context, candidate))
                    return false;
            }
            catch (Exception exception)
            {
                lock (SyncRoot)
                {
                    if (ReportedFailures.Add(registrationId))
                        MainFile.Logger.Warn($"Cross-character card filter '{registrationId}' failed and was ignored: {exception.Message}");
                }
            }
        }

        return true;
    }

    private sealed class Registration(string registrationId) : IDisposable
    {
        private string? _registrationId = registrationId;

        public void Dispose()
        {
            string? id = Interlocked.Exchange(ref _registrationId, null);
            if (id == null)
                return;

            lock (SyncRoot)
            {
                Filters.Remove(id);
                ReportedFailures.Remove(id);
            }
        }
    }
}
