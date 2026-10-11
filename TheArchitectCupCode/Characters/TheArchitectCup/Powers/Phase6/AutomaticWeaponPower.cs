using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class AutomaticWeaponPower : BasePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Attack ||
            !card.Keywords.Contains(CardKeyword.Unplayable) || CombatManager.Instance.IsOverOrEnding)
            return;

        AttackCommand attack;
        if (card.DynamicVars.ContainsKey("CalculatedDamage"))
            attack = DamageCmd.Attack(card.DynamicVars.CalculatedDamage);
        else if (card.DynamicVars.ContainsKey("Damage"))
            attack = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue);
        else if (card.DynamicVars.ContainsKey("OstyDamage"))
            attack = DamageCmd.Attack(card.DynamicVars.OstyDamage.BaseValue);
        else
        {
            Log.Warn($"{Id.Entry}: {card.Id.Entry} has no supported damage variable.");
            return;
        }

        Flash();
        // Normal attack resolution applies modifiers separately for each enemy.
        if (card.DynamicVars.ContainsKey("OstyDamage") && !card.DynamicVars.ContainsKey("Damage") &&
            card.Owner.Osty is { } osty)
            attack.FromOsty(osty, card, null);
        else
            attack.FromCard(card, null);
        await attack.TargetingAllOpponents(CombatState)
            .WithNoAttackerAnim().WithHitFx("vfx/vfx_attack_slash", "slash_attack.mp3")
            .Execute(choiceContext);
    }
}
