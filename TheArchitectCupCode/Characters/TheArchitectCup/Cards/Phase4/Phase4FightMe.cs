using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class Phase4FightMe() : ArchitectCupCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "登高"),
        new HoverTip(new LocString("static_hover_tips", "CHAMPION_PHASE4.title"), new LocString("static_hover_tips", "CHAMPION_PHASE4.description"))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13m, ValueProp.Move),
        new DynamicVar("EnemyDamage", 13m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        MonsterModel? monster = cardPlay.Target.Monster;
        bool shouldForceAttack = monster is not null && !monster.IntendsToAttack;
        MoveState? replacedMove = shouldForceAttack ? monster!.NextMove : null;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (!shouldForceAttack || monster is null || replacedMove is null || cardPlay.Target.IsDead)
            return;

        int enemyDamage = (int)DynamicVars["EnemyDamage"].BaseValue;
        MoveState forcedAttack = new(
            "THE_ARCHITECT_CUP_FIGHT_ME_MOVE",
            _ => DamageCmd.Attack(enemyDamage)
                .FromMonster(monster)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(null),
            new SingleAttackIntent(enemyDamage))
        {
            FollowUpStateId = replacedMove.Id,
            MustPerformOnceBeforeTransitioning = true
        };

        monster.SetMoveImmediate(forcedAttack, forceTransition: true);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
