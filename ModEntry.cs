using BehaviorTree;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing;
using JumpKing.GameManager.MultiEnding;
using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;
using JumpKing.GameManager.MultiEnding.NormalEnding;
using JumpKing.GameManager.MultiEnding.OwlEnding;
using JumpKing.Mods;
using JumpKing.Util;
using JumpKing.Util.DrawBT;
using LessBabeNoises.Patching;
using System.Collections.Generic;
using System.Linq;

namespace LessBabeNoises
{
    [JumpKingMod("Zebra.LessBabeNoises")]
    public static class ModEntry
    {
        public static bool MuteMainBabe { get; private set; } = false;
        public static bool MuteNewBabe { get; private set; } = false;
        public static bool MuteGhostBabe { get; private set; } = false;

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            //Debugger.Launch();

            Harmony harmony = new Harmony("Zebra.LessBabeNoises.Harmony");
            new EndingKing(harmony);
            new NBPKingEntity(harmony);
            new HangingBabe(harmony);
            new OwlKingEntity(harmony);
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            // Babes get created before OnLevelStart is called, so we cant rely on their MakeBT method to remove babe sounds!
            // We will have to get their BehaviorTreeComp some other way.
            // OnLevelEnd is called before the ending plays, so we reset here

            MuteMainBabe = false;
            MuteNewBabe = false;
            MuteGhostBabe = false;

            if (Game1.instance.contentManager?.level?.Info.Tags is null)
            {
                return;
            }

            List<IEnding> endings = Traverse.Create(Game1.instance.m_game)
                                        .Field("m_game_loop")
                                        .Field("m_ending_manager")
                                        .Field("m_endings")
                                        .GetValue<List<IEnding>>();

            foreach (string tag in Game1.instance.contentManager.level.Info.Tags)
            {
                IEnding ending;
                if (tag == "MuteMainBabe")
                {
                    MuteMainBabe = true;
                    ending = endings.Find(e => e.GetType() == typeof(NormalEnding));
                    RemoveBabeNoises(ending);
                }
                else if (tag == "MuteNewBabe")
                {
                    MuteNewBabe = true;
                    ending = endings.Find(e => e.GetType() == typeof(NewBabePlusEnding));
                    RemoveBabeNoises(ending);
                }
                else if (tag == "MuteGhostBabe")
                {
                    MuteGhostBabe = true;
                    ending = endings.Find(e => e.GetType() == typeof(OwlEnding));
                    RemoveBabeNoises(ending);
                }
            }
        }

        /// <summary>
        /// Removes the noises made by the babe in the ending.
        /// </summary>
        /// <param name="ending">The ending the babe belongs to</param>
        public static void RemoveBabeNoises(IEnding ending)
        {
            /* Sounds, in order played, are:
             * Main Babe
             * 1 - babe.Jump
             * 2 - player.Land
             * 3 - babe.Kiss
             * 4 - babe.Pickup
             * 
             * New Babe
             * 1 - babe.Jump
             * 2 - babe.Kiss
             * 3 - babe.Mou
             * 4 - audio.Plink
             * 5 - babe.Pickup
             * 
             * Ghost Babe
             * 1 - babe.Kiss
             * 2 - babe.Jump
             * 3 - babe.Pickup
             */

            ISpriteEntity babe = Traverse.Create(ending).Field("m_babe").GetValue<ISpriteEntity>();
            BTmanager btManager = babe.GetComponent<BehaviorTreeComp>().GetRaw();
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
