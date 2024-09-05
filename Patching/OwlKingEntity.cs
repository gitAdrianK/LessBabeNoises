using EntityComponent.BT;
using HarmonyLib;
using System;
using System.Reflection;

namespace LessBabeNoises.Patching
{
    public class OwlKingEntity
    {
        public OwlKingEntity(Harmony harmony)
        {
            Type ghostKing = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.OwlEnding.OwlKingEntity");

            MethodInfo ghostKingMakeBT = ghostKing.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod removeBabeNoises = new HarmonyMethod(typeof(OwlKingEntity).GetMethod(nameof(RemoveBabeNoises)));

            harmony.Patch(
                ghostKingMakeBT,
                postfix: removeBabeNoises);
        }

        public static void RemoveBabeNoises(BehaviorTreeComp __result)
        {
            if (!ModEntry.MuteGhostBabe)
            {
                return;
            }
            // TODO
        }
    }
}
