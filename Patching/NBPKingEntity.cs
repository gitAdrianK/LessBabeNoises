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
            /* Sounds are in order:
                1 - player.EndingParasol
                2 - babe.Jump
                3 - babe.Surprised
                4 - player.Jump
                5 - babe.Scream
             */

            if (!ModEntry.MuteNewBabe)
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
            foreach (IBTnode node in sequencorNodes)
            {
                if (node.GetType() == typeof(PlaySFX))
                {
                    continue;
                }
                remainingNodes.Add(node);
            }
            traverseSequencor.SetValue(remainingNodes.ToArray());
        }
    }
}
