using BehaviorTree;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing.Util;
using System;
using System.Collections.Generic;
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
            BTmanager btManager = __result.GetRaw();
            Traverse traverseNodes = Traverse.Create(btManager)
                                            .Field("m_root_node")
                                            .Field("m_children");
            IBTnode[] nodes = traverseNodes.GetValue<IBTnode[]>();
            List<IBTnode> remainingNodes = new List<IBTnode>();
            foreach (IBTnode node in nodes)
            {
                if (node.GetType() == typeof(PlaySFX))
                {
                    continue;
                }
                remainingNodes.Add(node);
            }
            traverseNodes.SetValue(remainingNodes.ToArray());
        }
    }
}
