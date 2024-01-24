﻿using EFT;
using EFT.UI;
using System;
using Comfort.Common;

namespace SkillsExtended.Helpers
{
    internal static class ConsoleCommands
    {
        public static void RegisterCommands()
        {
            ConsoleScreen.Processor.RegisterCommand("increaseFirstAid", new Action(DoIncreaseFirstAidLevel));
            ConsoleScreen.Processor.RegisterCommand("decreaseFirstAid", new Action(DoDecreaseFirstAidLevel));

            ConsoleScreen.Processor.RegisterCommand("increaseUsecAR", new Action(DoIncreaseUsecARLevel));
            ConsoleScreen.Processor.RegisterCommand("decreaseUsecAR", new Action(DoDecreaseUsecARLevel));

            ConsoleScreen.Processor.RegisterCommand("increaseBearAK", new Action(DoIncreaseBearAKLevel));
            ConsoleScreen.Processor.RegisterCommand("decreaseBearAK", new Action(DoDecreaseBearAKLevel));

            ConsoleScreen.Processor.RegisterCommand("increaseFirstAidScav", new Action(DoIncreaseFirstAidLevelScav));
            ConsoleScreen.Processor.RegisterCommand("decreaseFirstAidScav", new Action(DoDecreaseFirstAidLevelScav));

            ConsoleScreen.Processor.RegisterCommand("damage", new Action(DoDamage));
            ConsoleScreen.Processor.RegisterCommand("die", new Action(DoDie));
            ConsoleScreen.Processor.RegisterCommand("fracture", new Action(DoFracture));
        }

        #region SKILLS

        public static void DoIncreaseFirstAidLevel()
        {
            var firstAid = Plugin.Session.Profile.Skills.FirstAid;

            if (firstAid == null) { return; }

            firstAid.SetLevel(firstAid.Level + 1);
        }

        public static void DoDecreaseFirstAidLevel()
        {
            var firstAid = Plugin.Session.Profile.Skills.FirstAid;

            if (firstAid == null) { return; }

            firstAid.SetLevel(firstAid.Level - 1);
        }

        public static void DoIncreaseUsecARLevel()
        {
            var usec = Plugin.Session.Profile.Skills.UsecArsystems;

            if (usec == null) { return; }

            usec.SetLevel(usec.Level + 1);
        }

        public static void DoDecreaseUsecARLevel()
        {
            var usec = Plugin.Session.Profile.Skills.UsecArsystems;

            if (usec == null) { return; }

            usec.SetLevel(usec.Level - 1);
        }

        public static void DoIncreaseBearAKLevel()
        {
            var bear = Plugin.Session.Profile.Skills.BearAksystems;

            if (bear == null) { return; }

            bear.SetLevel(bear.Level + 1);
        }

        public static void DoDecreaseBearAKLevel()
        {
            var bear = Plugin.Session.Profile.Skills.BearAksystems;

            if (bear == null) { return; }

            bear.SetLevel(bear.Level - 1);
        }

        public static void DoIncreaseFirstAidLevelScav()
        {
            var firstAid = Plugin.Session.ProfileOfPet.Skills.FirstAid;

            if (firstAid == null) { return; }

            firstAid.SetLevel(firstAid.Level + 1);
        }

        public static void DoDecreaseFirstAidLevelScav()
        {
            var firstAid = Plugin.Session.ProfileOfPet.Skills.FirstAid;

            if (firstAid == null) { return; }

            firstAid.SetLevel(firstAid.Level - 1);
        }

        #endregion

        #region HEALTH

        public static void DoDamage()
        {
            var player = Singleton<GameWorld>.Instance.MainPlayer;
            DamageInfo Blunt = new DamageInfo();

            if (player == null) { return; }

            player.ActiveHealthController.ApplyDamage(EBodyPart.LeftArm, 50, Blunt);
        }

        public static void DoDie()
        {
            var player = Singleton<GameWorld>.Instance.MainPlayer;
            DamageInfo Blunt = new DamageInfo();

            if (player == null) { return; }

            player.ActiveHealthController.ApplyDamage(EBodyPart.Head, int.MaxValue, Blunt);
        }

        public static void DoFracture()
        {
            var player = Singleton<GameWorld>.Instance.MainPlayer;

            if (player == null) { return; }

            player.ActiveHealthController.DoFracture(EBodyPart.LeftArm);
        }

        #endregion
    }
}
