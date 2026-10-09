using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
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
        if (card.Owner.Creature == Owner && card.Type == CardType.Attack && card.Keywords.Contains(CardKeyword.Unplayable))
        {
            decimal damage = default;
            if (card.DynamicVars.ContainsKey("CalculatedDamage"))
            {
                damage = card.DynamicVars.CalculatedDamage.Calculate(null);
            }
            else if (card.DynamicVars.ContainsKey("Damage"))
            {
                damage = card.DynamicVars.Damage.BaseValue;
            }
            else if (card.DynamicVars.ContainsKey("OstyDamage"))
            {
                damage = card.DynamicVars.OstyDamage.BaseValue;
            }
            else
            {
                Log.Warn(Id.Entry + " attack card " + card.Id.Entry + " that did not have an appropriate damage var!");
            }
            Player? player = Owner.Player;
            if(player == null) return;
            damage = Hook.ModifyDamage(player.RunState, Owner.CombatState, null, Owner, damage, ValueProp.Move, card, null, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);
            VfxCmd.PlayOnCreatureCenters(CombatState.HittableEnemies, "vfx/vfx_attack_slash");
            SfxCmd.Play("slash_attack.mp3");
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, damage, ValueProp.Unpowered, Owner);
        }
    }
}
