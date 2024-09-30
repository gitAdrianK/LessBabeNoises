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
            /* Sounds are in order:
                1 - audio.Plink
                2 - babe.Pickup
                3 - babe.Surprised
                
                (Unused)
                1 - player.Jump

                1 - player.Splat
                2 - babe.Scream
                3 - babe.Kiss
             */

            if (!ModEntry.MuteGhostBabe)
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
