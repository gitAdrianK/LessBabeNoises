using BehaviorTree;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing.Util;
using System;
using System.Collections.Generic;
using System.Linq;
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
            BTmanager btManager = __result.GetRaw();
            IBTnode[] managerNodes = Traverse.Create(btManager)
                                            .Field("m_root_node")
                                            .Field("m_children")
                                            .GetValue<IBTnode[]>();
            List<IBTnode> managerNodesList = managerNodes.ToList();
            BTsequencor btSequencor = (BTsequencor)managerNodesList.Find(node => node.GetType() == typeof(BTsequencor));
            Traverse traverseSequencor = Traverse.Create(btSequencor)
                .Field("m_children");
            IBTnode[] sequencorNodes = traverseSequencor.GetValue<IBTnode[]>();
            List<IBTnode> remainingNodes = new List<IBTnode>();
            int count = 0;
            foreach (IBTnode node in sequencorNodes)
            {
                if (node.GetType() == typeof(PlaySFX))
                {
                    count++;
                    if (count == 5
                        || count == 6
                        || count == 8)
                    {
                        continue;
                    }
                }
                remainingNodes.Add(node);
            }
            traverseSequencor.SetValue(remainingNodes.ToArray());
            BTsimultaneous btSimultaneos = (BTsimultaneous)remainingNodes.FindLast(node => node.GetType() == typeof(BTsimultaneous));
            BTsequencor btSequencor2 = (BTsequencor)Traverse.Create(btSimultaneos)
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .ToList()
                .Find(node => node.GetType() == typeof(BTsequencor));
            Traverse traverseSequencor2 = Traverse.Create(btSequencor2)
                                                .Field("m_children");
            IBTnode[] sequencorNodes2 = traverseSequencor2.GetValue<IBTnode[]>();
            List<IBTnode> remainingNodes2 = new List<IBTnode>();
            foreach (IBTnode node in sequencorNodes2)
            {
                if (node.GetType() == typeof(PlaySFX))
                {
                    continue;
                }
                remainingNodes2.Add(node);
            }
            traverseSequencor2.SetValue(remainingNodes2.ToArray());
        }
    }
}
