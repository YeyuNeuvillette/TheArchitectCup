namespace TheArchitectCup.Api;

/// <summary>Read-only metadata for a card provided by The Architect Cup.</summary>
public sealed record ArchitectCupCardInfo(
    string Id,
    Type ModelType,
    int Phase,
    string Author,
    bool Configurable,
    bool MultiplayerOnly,
    bool ShowInCompendium);
