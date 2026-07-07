using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public sealed class PingPongPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public bool IsUpgraded { get; set; }

    private readonly Color _vfxTint = new Color("83eb85");

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner)
            return;

        if (cardPlay.Card.EnergyCost.GetResolved() != 0)
            return;

        await PlayBouncingFlaskVfx();
    }

    private async Task PlayBouncingFlaskVfx()
    {
        Flash();

        Creature? owner = Owner;
        if (owner == null)
            return;

        ICombatState? combatState = owner.CombatState;
        if (combatState == null)
            return;

        Creature? enemy = Owner.Player?.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
        if (enemy == null)
            return;

        await CreatureCmd.TriggerAnim(owner, "Cast", Owner.Player?.Character.CastAnimDelay ?? 0f);

        NCombatRoom? combatRoom = NCombatRoom.Instance;
        if (combatRoom == null)
            return;

        NCreature? ownerNode = combatRoom.GetCreatureNode(owner);
        if (ownerNode == null)
            return;

        Vector2 lastPos = ownerNode.VfxSpawnPosition;
        NCreature? targetNode = combatRoom.GetCreatureNode(enemy);

        if (targetNode != null)
        {
            int repeatCount = IsUpgraded ? 4 : 3;

            for (int i = 0; i < repeatCount; i++)
            {
                NItemThrowVfx? throwVfx = NItemThrowVfx.Create(lastPos, targetNode.GetBottomOfHitbox(), ModelDb.Potion<PoisonPotion>().Image);
                if (throwVfx != null)
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(throwVfx);
                lastPos = targetNode.VfxSpawnPosition;
                await Cmd.Wait(0.5f);

                NSplashVfx? splashVfx = NSplashVfx.Create(targetNode.VfxSpawnPosition, _vfxTint);
                if (splashVfx != null)
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(splashVfx);

                NLiquidOverlayVfx? liquidVfx = NLiquidOverlayVfx.Create(enemy, _vfxTint);
                if (liquidVfx != null)
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(liquidVfx);

                NGaseousImpactVfx? impactVfx = NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, _vfxTint);
                if (impactVfx != null)
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(impactVfx);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}