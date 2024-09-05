using EntityComponent.BT;
using HarmonyLib;
using System;
using System.Reflection;

namespace LessBabeNoises.Patching
{
    public class HangingBabe
    {
        public HangingBabe(Harmony harmony)
        {
            Type hangingBabe = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.HangingBabe");

            MethodInfo hangingBabeMakeBT = hangingBabe.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod removeBabeNoises = new HarmonyMethod(typeof(HangingBabe).GetMethod(nameof(RemoveBabeNoises)));

            harmony.Patch(
                hangingBabeMakeBT,
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
