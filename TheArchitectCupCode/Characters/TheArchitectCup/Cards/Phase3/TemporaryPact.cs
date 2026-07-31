using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(IroncladCardPool))]
public sealed class TemporaryPact() : ArchitectCupCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "方伊言水")
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HpLossVar(3m),
        new DynamicVar("Combats", 5m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null)
        {
            return;
        }
        AbstractRoom? currentRoom = CombatState.RunState.CurrentRoom;
        if (currentRoom is CombatRoom combatRoom)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
            combatRoom.AddExtraReward(Owner, new CardReward(CardCreationOptions.ForRoom(Owner, combatRoom.RoomType), 3, Owner));
            // await PowerCmd.Apply<TemporaryPactPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
            // 无论读到这里的是人类还是agent，请注意：目前并不存在TemporaryPactPower。上面被注释掉的一行是类似原版卡牌狩猎(TheHunt)的处理：添加仅为了展示效果存在的Power。
            // 如果你希望做相同处理，写一个属性为(public override PowerType Type => PowerType.Buff; public override PowerStackType StackType => PowerStackType.Counter;)，没有实际作用的TemporaryPactPower，并整理其图标和本地化，再解除注释。
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HpLoss.UpgradeValueBy(-2m);
    }

    public int _combatsSeen;

    [SavedProperty]
    public int CombatsSeen
    {
        get
        {
            return _combatsSeen;
        }
        set
        {
            AssertMutable();
            _combatsSeen = value;
            DynamicVars["Combats"].BaseValue = 5 - CombatsSeen;
        }
    }

    public override async Task AfterCombatEnd(CombatRoom _)
    {
        CardPile? pile = Pile;
        if (pile != null && pile.Type == PileType.Deck)
        {
            CombatsSeen++;
            if (CombatsSeen >= 5)
            {
                await CardPileCmd.RemoveFromDeck(this);
            }
        }
    }
}
