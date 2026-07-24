using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Api;
using TheArchitectCup.Characters.TheArchitectCup.Cards;
using TheArchitectCup.Characters.TheArchitectCup.Cards.Phase2;
using TheArchitectCup.Settings;

namespace TheArchitectCup.Content;

internal sealed record ArchitectCupCardDefinition(
    string Id,
    Type ModelType,
    int Phase,
    string Author,
    string EnglishTitle,
    string? SettingId,
    CardMultiplayerConstraint MultiplayerConstraint,
    bool ShowInCompendium,
    Func<CardSettingsData, bool>? ReadEnabled,
    Action<CardSettingsData, bool>? WriteEnabled)
{
    internal bool Configurable => SettingId != null && ReadEnabled != null && WriteEnabled != null;

    internal ArchitectCupCardInfo ToPublicInfo() => new(
        Id,
        ModelType,
        Phase,
        Author,
        Configurable,
        MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly,
        ShowInCompendium);
}

internal static class ArchitectCupCardCatalog
{
    internal static IReadOnlyList<ArchitectCupCardDefinition> All { get; } =
    [
        Configurable<CursedHandkerchief>(ArchitectCupCardIds.CursedHandkerchief, 1, "罡璧", "Cursed Handkerchief", "cursed_handkerchief",
            static data => data.CursedHandkerchiefEnabled,
            static (data, value) => data.CursedHandkerchiefEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<ThreeLeggedRace>(ArchitectCupCardIds.ThreeLeggedRace, 1, "六方最密堆积", "Three-Legged Race", "three_legged_race",
            static data => data.ThreeLeggedRaceEnabled,
            static (data, value) => data.ThreeLeggedRaceEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<PingPong>(ArchitectCupCardIds.PingPong, 1, "小肥", "Ping Pong", "ping_pong",
            static data => data.PingPongEnabled,
            static (data, value) => data.PingPongEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<GenerousDonation>(ArchitectCupCardIds.GenerousDonation, 1, "SkyF", "Generous Donation", "generous_donation",
            static data => data.GenerousDonationEnabled,
            static (data, value) => data.GenerousDonationEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<FindSomeoneToGetYou>(ArchitectCupCardIds.FindSomeoneToGetYou, 1, "花盆上屹立的不明食草兽", "Find Someone to Do It", "find_someone_to_get_you",
            static data => data.FindSomeoneToGetYouEnabled,
            static (data, value) => data.FindSomeoneToGetYouEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Generated<Human>(ArchitectCupCardIds.Human, 1, "花盆上屹立的不明食草兽", "Human"),
        Configurable<EarthShattering>(ArchitectCupCardIds.EarthShattering, 1, "absi2011", "Earth-Shattering", "earth_shattering",
            static data => data.EarthShatteringEnabled,
            static (data, value) => data.EarthShatteringEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<BurnTheMountain>(ArchitectCupCardIds.BurnTheMountain, 1, "BlueGhost", "Burn the Mountain", "burn_the_mountain",
            static data => data.BurnTheMountainEnabled,
            static (data, value) => data.BurnTheMountainEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<BeeBroOnTheRun>(ArchitectCupCardIds.BeeBroOnTheRun, 1, "阿迪", "Bee Bro on the Run", "bee_bro_on_the_run",
            static data => data.BeeBroOnTheRunEnabled,
            static (data, value) => data.BeeBroOnTheRunEnabled = value,
            CardMultiplayerConstraint.MultiplayerOnly),
        Configurable<Coupling>(ArchitectCupCardIds.Coupling, 2, "助手", "Coupling", "coupling",
            static data => data.CouplingEnabled,
            static (data, value) => data.CouplingEnabled = value),
        Configurable<PressForward>(ArchitectCupCardIds.PressForward, 2, "糯米团子", "Press Forward", "press_forward",
            static data => data.PressForwardEnabled,
            static (data, value) => data.PressForwardEnabled = value),
        Configurable<VakuuTeachesUToPlay>(ArchitectCupCardIds.VakuuTeachesUToPlay, 2, "塔莉娜", "Vakuu Teaches You to Play", "vakuu_teaches_u_to_play",
            static data => data.VakuuTeachesUToPlayEnabled,
            static (data, value) => data.VakuuTeachesUToPlayEnabled = value),
        Configurable<LikeShadow>(ArchitectCupCardIds.LikeShadow, 2, "Kijin Seija 正邪", "Like a Shadow", "like_shadow",
            static data => data.LikeShadowEnabled,
            static (data, value) => data.LikeShadowEnabled = value),
        Configurable<Agitation>(ArchitectCupCardIds.Agitation, 2, "Alome", "Agitation", "agitation",
            static data => data.AgitationEnabled,
            static (data, value) => data.AgitationEnabled = value),
    ];

    internal static IEnumerable<ArchitectCupCardDefinition> ConfigurableCards =>
        All.Where(static definition => definition.Configurable);

    internal static bool TryGet(string cardId, out ArchitectCupCardDefinition definition)
    {
        definition = All.FirstOrDefault(candidate =>
            string.Equals(candidate.Id, cardId, StringComparison.Ordinal))!;
        return definition != null;
    }

    private static ArchitectCupCardDefinition Configurable<TCard>(
        string id,
        int phase,
        string author,
        string englishTitle,
        string settingId,
        Func<CardSettingsData, bool> readEnabled,
        Action<CardSettingsData, bool> writeEnabled,
        CardMultiplayerConstraint multiplayerConstraint = CardMultiplayerConstraint.None)
        where TCard : CardModel =>
        new(
            id,
            typeof(TCard),
            phase,
            author,
            englishTitle,
            settingId,
            multiplayerConstraint,
            true,
            readEnabled,
            writeEnabled);

    private static ArchitectCupCardDefinition Generated<TCard>(
        string id,
        int phase,
        string author,
        string englishTitle)
        where TCard : CardModel =>
        new(
            id,
            typeof(TCard),
            phase,
            author,
            englishTitle,
            null,
            CardMultiplayerConstraint.None,
            false,
            null,
            null);

}
