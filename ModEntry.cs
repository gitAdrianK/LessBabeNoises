using EntityComponent.BT;
using HarmonyLib;
using JumpKing;
using JumpKing.GameManager.MultiEnding;
using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;
using JumpKing.GameManager.MultiEnding.NormalEnding;
using JumpKing.GameManager.MultiEnding.OwlEnding;
using JumpKing.Mods;
using JumpKing.Util.DrawBT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace LessBabeNoises
{
    [JumpKingMod("Zebra.LessBabeNoises")]
    public static class ModEntry
    {
        private static bool muteMainBabe = false;
        private static bool muteNewBabe = false;
        private static bool muteGhostBabe = false;

        private static Type mainKing;
        private static Type newKing;
        private static Type newHangingBabe;
        private static Type ghostKing;

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            Debugger.Launch();

            Harmony harmony = new Harmony("Zebra.LessBabeNoises.Harmony");
            HarmonyMethod removeBabeNoises = new HarmonyMethod(typeof(ModEntry).GetMethod(nameof(RemoveBabeNoises)));

            mainKing = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.NormalEnding.EndingKing");
            MethodInfo mainKingMakeBT = mainKing.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            harmony.Patch(
                mainKingMakeBT,
                postfix: removeBabeNoises);

            newKing = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPKingEntity");
            MethodInfo newKingMakeBT = newKing.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            harmony.Patch(
                newKingMakeBT,
                postfix: removeBabeNoises);
            newHangingBabe = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.HangingBabe");
            MethodInfo newHangingBabeMakeBT = newHangingBabe.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            harmony.Patch(
                newHangingBabeMakeBT,
                postfix: removeBabeNoises);

            ghostKing = AccessTools.TypeByName("JumpKing.GameManager.MultiEnding.OwlEnding.OwlKingEntity");
            MethodInfo ghostKingMakeBT = ghostKing.GetMethod("MakeBT", BindingFlags.NonPublic | BindingFlags.Instance);
            harmony.Patch(
                ghostKingMakeBT,
                postfix: removeBabeNoises);
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            // Babes get created before OnLevelStart is called, so we cant rely on their MakeBT method to remove babe sounds!

            if (Game1.instance.contentManager?.level?.Info.Tags is null)
            {
                return;
            }

            object endingManager = AccessTools.Field("JumpKing.GameManager.MultiEnding.EndingManager:_instance");
            List<IEnding> endings = Traverse.Create(endingManager).Field("m_endings").GetValue<List<IEnding>>();
            foreach (string tag in Game1.instance.contentManager.level.Info.Tags)
            {
                IEnding ending;
                if (tag == "MuteMainBabe")
                {
                    muteMainBabe = true;
                    ending = endings.Find(e => e.GetType() == typeof(NormalEnding));
                }
                else if (tag == "MuteNewBabe")
                {
                    muteNewBabe = true;
                    ending = endings.Find(e => e.GetType() == typeof(NewBabePlusEnding));
                }
                else if (tag == "MuteGhostBabe")
                {
                    muteGhostBabe = true;
                    ending = endings.Find(e => e.GetType() == typeof(OwlEnding));
                }
                else
                {
                    continue;
                }

                ISpriteEntity babe = Traverse.Create(ending).Field("m_babe").GetValue<ISpriteEntity>();
                BehaviorTreeComp btc = babe.GetComponent<BehaviorTreeComp>();
            }
        }

        /// <summary>
        /// Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        public static void OnLevelEnd()
        {
            muteMainBabe = false;
            muteNewBabe = false;
            muteGhostBabe = false;
        }

        public static void RemoveBabeNoises(object __instance, ref BehaviorTreeComp __result)
        {
            if (__instance.GetType() == mainKing && !muteMainBabe)
            {
                return;
            }
            if (__instance.GetType() == newKing && !muteNewBabe)
            {
                return;
            }
            if (__instance.GetType() == newHangingBabe && !muteNewBabe)
            {
                return;
            }
            if (__instance.GetType() == ghostKing && !muteGhostBabe)
            {
                return;
            }

        }
    }
}
