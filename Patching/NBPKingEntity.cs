using EntityComponent.BT;
using HarmonyLib;
using System;
using System.Reflection;

namespace LessBabeNoises.Patching
{
    public class NBPKingEntity
    {
        public NBPKingEntity(Harmony harmony)
        {
            Type newKing = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPKingEntity");

            MethodInfo newKingMakeBT = newKing.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod removeBabeNoises = new HarmonyMethod(typeof(NBPKingEntity).GetMethod(nameof(RemoveBabeNoises)));

            harmony.Patch(
                newKingMakeBT,
                postfix: removeBabeNoises);
        }

        public static void RemoveBabeNoises(BehaviorTreeComp __result)
        {
            if (!ModEntry.MuteNewBabe)
            {
                return;
            }
            // TODO
        }
    }
}
