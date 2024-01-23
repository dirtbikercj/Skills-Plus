﻿using Aki.Reflection.Patching;
using EFT;
using EFT.UI;
using EFT.UI.Screens;
using HarmonyLib;
using SkillsExtended.Controllers;
using SkillsExtended.Helpers;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using static EFT.SkillManager;

namespace SkillsExtended.Patches
{
    internal class SkillsPatches
    {
        internal class SkillManagerConstructorPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod() =>
                typeof(SkillManager).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(EPlayerSide) }, null);

            [PatchPostfix]
            public static void Postfix(SkillManager __instance, ref GClass1635[] ___DisplayList, ref GClass1635[] ___Skills, 
                ref GClass1635 ___UsecArsystems, ref GClass1635 ___BearAksystems)
            {
                int insertIndex = 12;

                ___UsecArsystems = new GClass1635(__instance, ESkillId.UsecArsystems, ESkillClass.Special, Array.Empty<GClass1647>(), Array.Empty<GClass1640>());
                ___BearAksystems = new GClass1635(__instance, ESkillId.BearAksystems, ESkillClass.Special, Array.Empty<GClass1647>(), Array.Empty<GClass1640>());

                var newDisplayList = new GClass1635[___DisplayList.Length + 2];

                Array.Copy(___DisplayList, newDisplayList, insertIndex);

                newDisplayList[12] = ___UsecArsystems;
                newDisplayList[12 + 1] = ___BearAksystems;

                Array.Copy(___DisplayList, insertIndex, newDisplayList, insertIndex + 2, ___DisplayList.Length - insertIndex);

                ___DisplayList = newDisplayList;

                Array.Resize(ref ___Skills, ___Skills.Length + 2);

                ___Skills[___Skills.Length - 1] = ___UsecArsystems;
                ___Skills[___Skills.Length - 2] = ___BearAksystems;

                AccessTools.Field(Utils.GetSkillType(), "Locked").SetValue(__instance.UsecArsystems, false);
                AccessTools.Field(Utils.GetSkillType(), "Locked").SetValue(__instance.BearAksystems, false);
            }
        }

        internal class EnableSkillsPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod() =>
                typeof(SkillManager).GetMethod("method_3", BindingFlags.NonPublic | BindingFlags.Instance);

            [PatchPostfix]
            public static void Postfix(SkillManager __instance)
            {
                try
                { 
                    AccessTools.Field(Utils.GetSkillType(), "Locked").SetValue(__instance.FirstAid, false);
                    AccessTools.Field(Utils.GetSkillType(), "Locked").SetValue(__instance.FieldMedicine, false);
                }
                catch(Exception e)
                {
                    Plugin.Log.LogDebug(e);
                }   
            }
        }

        internal class SimpleToolTipPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod() =>
                typeof(SimpleTooltip).GetMethod("Show");

            [PatchPostfix]
            public static void Postfix(SimpleTooltip __instance, ref string text)
            {
                string firstAid = @"\bFirstAidDescriptionPattern\b";
                string fieldMedicine = @"\bFieldMedicineDescriptionPattern\b";
                string usecARSystems = @"\bUsecArsystemsDescription\b";
                string bearAKSystems = @"\bBearAksystemsDescription\b";

                if (Regex.IsMatch(text, firstAid))
                {
                    var speedBonus = MedicalBehavior.playerSkillManager.FirstAid.Level * 0.007f;
                    var hpBonus = MedicalBehavior.playerSkillManager.FirstAid.Level * 5f;

                    if (MedicalBehavior.playerSkillManager.FirstAid.IsEliteLevel)
                    {
                        speedBonus += 0.15f;
                        hpBonus = MedicalBehavior.playerSkillManager.FirstAid.Level * 10f;
                    }

                    __instance.SetText($"First aid skills make use of first aid kits quicker and more effective." +
                        $"\n\n Increases the speed of healing items by 0.7% per level. \n\n Elite bonus: 15% \n\n Increases the HP resource of medical items by 5 per level. \n\n Elite bonus: 10 per level." +
                        $"\n\n Current speed bonus: <color=red>{speedBonus * 100}%</color> \n\n Current bonus HP: <color=red>{hpBonus}</color>");
                }

                if (Regex.IsMatch(text, fieldMedicine))
                {
                    var speedBonus = MedicalBehavior.playerSkillManager.FieldMedicine.Level * 0.007f;

                    if (MedicalBehavior.playerSkillManager.FirstAid.IsEliteLevel)
                    {
                        speedBonus += 0.15f;
                    }

                    __instance.SetText($"Field Medicine increases your skill at applying wound dressings. \n\n Increases the speed of splints, bandages, and heavy bleed items 0.7% per level. \n\n Elite bonus: 15% " +
                        $"\n\n Current speed bonus: <color=red>{speedBonus * 100}%</color>");
                }

                if (Regex.IsMatch(text, usecARSystems))
                {
                    var usecSystems = Plugin.Session.Profile.Skills.UsecArsystems;

                    var ergoBonus = usecSystems.IsEliteLevel ? usecSystems.Level * Constants.USEC_ERGO_MOD + Constants.USEC_ERGO_MOD_ELITE : usecSystems.Level * Constants.USEC_ERGO_MOD;
                    var recoilReduction = usecSystems.IsEliteLevel ? usecSystems.Level * Constants.USEC_RECOIL_REDUCTION + Constants.USEC_RECOIL_REDUCTION_ELITE : usecSystems.Level * Constants.USEC_RECOIL_REDUCTION;

                    __instance.SetText($"As a USEC PMC, you excel in the use of NATO assault rifles and carbines. \n\n" +
                        $"Inceases ergonomics by {Constants.USEC_ERGO_MOD * 100}% per level on NATO assault rifles and carbines. \n {Constants.USEC_RECOIL_REDUCTION_ELITE * 100}% Elite bonus \n\n" +
                        $"Reduces vertical and horizontal recoil by {Constants.USEC_RECOIL_REDUCTION * 100}% per level. \n {Constants.USEC_RECOIL_REDUCTION_ELITE * 100}% Elite bonus \n\n" + 
                        $"Current ergonomics bonus: <color=red>{ergoBonus * 100}%</color>\n" +
                        $"Current recoil bonuses: <color=red>{recoilReduction * 100}%</color>");
                }

                if (Regex.IsMatch(text, bearAKSystems))
                {
                    //TODO: Replace placeholder text

                    __instance.SetText($"As a Bear PMC, you excel in the use of eastern block assult rifles and carbines.");
                }
            }

            internal class SkillPanelNamePatch : ModulePatch
            {
                protected override MethodBase GetTargetMethod() =>
                    typeof(SkillPanel).GetMethod("Show", BindingFlags.Public | BindingFlags.Instance);

                [PatchPostfix]
                public static void Postfix(SkillPanel __instance, GClass1635 skill)
                {
                    if (skill.Id == ESkillId.UsecArsystems)
                    {
                        TextMeshProUGUI name = (TextMeshProUGUI)AccessTools.Field(typeof(SkillPanel), "_name").GetValue(__instance);
                        name.text = "USEC rifle and carbine proficiency";
                    }

                    if (skill.Id == ESkillId.BearAksystems)
                    {
                        TextMeshProUGUI name = (TextMeshProUGUI)AccessTools.Field(typeof(SkillPanel), "_name").GetValue(__instance);
                        name.text = "BEAR rifle and carbine proficiency";
                    }
                }
            }

            internal class OnScreenChangePatch : ModulePatch
            {
                protected override MethodBase GetTargetMethod() =>
                    typeof(MenuTaskBar).GetMethod("OnScreenChanged");

                [PatchPrefix]
                public static void Prefix(EEftScreenType eftScreenType)
                {
                    if (eftScreenType == EEftScreenType.Inventory)
                    {
                        Plugin.MedicalScript.fieldMedicineInstanceIDs.Clear();
                        Plugin.MedicalScript.firstAidInstanceIDs.Clear();
                        Plugin.UsecARSystems.weaponInstanceIds.Clear(); 
                        Utils.GetServerConfig();
                    }
                }
            }
        }
    }
}
