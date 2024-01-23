﻿using System;
using EFT.UI;
using BepInEx;
using UnityEngine;
using BepInEx.Logging;
using Aki.Reflection.Utils;
using SkillsExtended.Controllers;
using DrakiaXYZ.VersionChecker;
using SkillsExtended.Helpers;
using static SkillsExtended.Patches.SkillsPatches;
using System.Collections.Generic;
using Skills_Extended.Controllers;

namespace SkillsExtended
{
    [BepInPlugin("com.dirtbikercj.SkillsExtended", "Skills Extended", "0.2.3")]

    public class Plugin : BaseUnityPlugin
    {
        public const int TarkovVersion = 26535;

        public static ISession Session;
        public static Profile SEProfile;

        internal static GameObject Hook;
        internal static MedicalBehavior MedicalScript;
        internal static UsecARSystemsBehavior UsecARSystems;
        internal static BearAkSystemsBehavior BearAkSystems;

        internal static ManualLogSource Log;

        private bool _warned = false;

        void Awake()
        {
            if (!VersionChecker.CheckEftVersion(Logger, Info, Config))
            {
                throw new Exception("Invalid EFT Version");
            }

            new EnableSkillsPatch().Enable();
            new SimpleToolTipPatch().Enable();
            new SkillManagerConstructorPatch().Enable();

            Log = Logger;
            Log.LogInfo("Loading Skills Extended");
            Hook = new GameObject("Event Object");
           
            MedicalScript = Hook.AddComponent<MedicalBehavior>();
            UsecARSystems = Hook.AddComponent<UsecARSystemsBehavior>();
            BearAkSystems = Hook.AddComponent<BearAkSystemsBehavior>();

            DontDestroyOnLoad(Hook);
            
            List<SkillProgress> progressList = new List<SkillProgress>
            {
                new SkillProgress { SkillId = "456", Progress = 0.75f },
                new SkillProgress { SkillId = "101", Progress = 0.50f },
            };

            Profile profileImpl = new Profile
            {
                ProfileId = "TESTID",
                Skills = progressList
            };

#if DEBUG
            ConsoleCommands.RegisterCommands();
#endif
        }

        void Update()
        {

            #if BETA
            if (!_warned && PreloaderUI.Instance != null)
            {
                PreloaderUI.Instance.ShowErrorScreen("Skills Extended", "Skills Extended: This is a BETA build. Report all bugs in the thread, or on the website.");
                Log.LogDebug("User was warned.");
                _warned = true;
            }
            #endif

            if (Session == null && ClientAppUtils.GetMainApp().GetClientBackEndSession() != null)
            {
                Session = ClientAppUtils.GetMainApp().GetClientBackEndSession();

                Log.LogDebug("Session set");
            }
        }
    }
}