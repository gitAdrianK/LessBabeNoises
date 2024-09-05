using EntityComponent.BT;
using HarmonyLib;
using System;
using System.Reflection;

namespace LessBabeNoises.Patching
{
    public class EndingKing
    {
        public EndingKing(Harmony harmony)
        {
            Type mainKing = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.NormalEnding.EndingKing");

            MethodInfo mainKingMakeBT = mainKing.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod removeBabeNoises = new HarmonyMethod(typeof(EndingKing).GetMethod(nameof(RemoveBabeNoises)));

            harmony.Patch(
                mainKingMakeBT,
                postfix: removeBabeNoises);
        }

        public static void RemoveBabeNoises(BehaviorTreeComp __result)
        {
            if (!ModEntry.MuteMainBabe)
            {
                return;
            }
            // TODO
        }
    }
}
