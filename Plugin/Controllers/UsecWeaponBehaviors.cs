using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SkillsExtended;
using SkillsExtended.Helpers;
using SkillsExtended.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Skills_Extended.Controllers
{
    public class UsecWeaponBehaviors : MonoBehaviour
    {
        private class OrigWeaponValues
        {
            public float ergo;
            public float weaponUp;
            public float weaponBack;
        }

        private SkillManager _skillManager;

        private GameWorld _gameWorld { get => Singleton<GameWorld>.Instance; }

        private Player _player { get => _gameWorld.MainPlayer; }

        private int _shotCount = 0;
        public Dictionary<string, int> weaponInstanceIds = new Dictionary<string, int>();

        //Index 0: ERGO, index 1: UP, index 2: BACK
        private Dictionary<string, OrigWeaponValues> _originalWeaponValues = new Dictionary<string, OrigWeaponValues>();

        private void Awake()
        {
            new IsShootingWeaponPatch().Enable();
        }

        private void Update()
        {
            // Set skill manager instance
            if (_skillManager == null && Plugin.Session?.Profile?.Skills != null)
            {
                _skillManager = Plugin.Session.Profile.Skills;

                Plugin.Log.LogDebug("Usec Weapon Comp Initialized");
            }

            if (_gameWorld?.MainPlayer != null)
            {
                if (_player.Side == EPlayerSide.Usec)
                {
                    _skillManager = _player.Skills;
                }
                else
                {
                    // We dont continue this behavior if the player is not USEC
                    return;
                }
            }

            if (_gameWorld == null && _shotCount != 0)
            {
                _shotCount = 0;
            }

            if (_skillManager == null)
            {
                return;
            }

            StaticManager.Instance.StartCoroutine(UpdateWeapon());
        }

        public void ApplyXp()
        {
            // Dont apply XP if we arent usec
            if (Plugin.Session.Profile.Side != EPlayerSide.Usec) { return; }

            _skillManager.UsecArsystems.Current += (0.15f * GetShotXpFactor());

            Plugin.Log.LogDebug("XP Gained.");
        }

        private float GetShotXpFactor()
        {
            if (_shotCount <= 60)
            {
                _shotCount++;
                return 1f;
            }
            else if (_shotCount > 60 && _shotCount <= 90)
            {
                _shotCount++;
                return 0.5f;
            }
            else if (_shotCount > 90)
            {
                _shotCount++;
                return 0.25f;
            }
            return 0f;
        }

        private IEnumerator UpdateWeapon()
        {
            #if RELEASE
            // Dont continue if we arent USEC
            if (Plugin.Session.Profile.Side != EPlayerSide.Usec) { yield break; }
            #endif

            // Only change weapons from the USEC list
            var items = Plugin.Session.Profile.Inventory.AllPlayerItems.Where(x => Constants.USEC_WEAPON_LIST.Contains(x.TemplateId));

            foreach ( var item in items)
            {
                if (item is Weapon weap)
                {
                    // Store the weapons original values
                    if (!_originalWeaponValues.ContainsKey(item.TemplateId))
                    {
                        var origVals = new OrigWeaponValues();

                        origVals.ergo = weap.Template.Ergonomics;
                        origVals.weaponUp = weap.Template.RecoilForceUp;
                        origVals.weaponBack = weap.Template.RecoilForceBack;

                        Plugin.Log.LogDebug($"Orig ergo: {weap.Template.Ergonomics}, up {weap.Template.RecoilForceUp}, back {weap.Template.RecoilForceBack}");

                        _originalWeaponValues.Add(item.TemplateId, origVals);
                    }

                    //Skip instances of the weapon that are already adjusted at this level.
                    if (weaponInstanceIds.ContainsKey(item.Id))
                    {
                        if (weaponInstanceIds[item.Id] == _skillManager.UsecArsystems.Level)
                        {
                            continue;
                        }
                        else
                        {
                            weaponInstanceIds.Remove(item.Id);
                        }
                    }

                    var level = _skillManager.UsecArsystems.Level;

                    var ergoBonus = _skillManager.UsecArsystems.IsEliteLevel ? level * Constants.USEC_ERGO_MOD + Constants.USEC_ERGO_MOD_ELITE : level * Constants.USEC_ERGO_MOD;
                    var recoilReduction = _skillManager.UsecArsystems.IsEliteLevel ? level * Constants.USEC_RECOIL_REDUCTION + Constants.USEC_RECOIL_REDUCTION_ELITE : level * Constants.USEC_RECOIL_REDUCTION;

                    weap.Template.Ergonomics = _originalWeaponValues[item.TemplateId].ergo * (1 + ergoBonus);
                    weap.Template.RecoilForceUp = _originalWeaponValues[item.TemplateId].weaponUp * (1 - recoilReduction); 
                    weap.Template.RecoilForceBack = _originalWeaponValues[item.TemplateId].weaponBack * (1 - recoilReduction);

                    Plugin.Log.LogDebug($"New ergo: {weap.Template.Ergonomics}, up {weap.Template.RecoilForceUp}, back {weap.Template.RecoilForceBack}");

                    weaponInstanceIds.Add(item.Id, level);
                }
            }

            yield break;
        }
    }
}