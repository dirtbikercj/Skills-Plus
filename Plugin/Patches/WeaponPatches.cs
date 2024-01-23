using Aki.Reflection.Patching;
using EFT;
using System.Reflection;
using SkillsExtended.Helpers;
using EFT.UI;
using EFT.InventoryLogic;

namespace SkillsExtended.Patches
{
    internal class IsShootingWeaponPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            typeof(Player.FirearmController).GetMethod("InitiateShot", BindingFlags.NonPublic | BindingFlags.Instance);

        [PatchPostfix]
        public static void Postfix(Player.FirearmController __instance, GInterface273 weapon)
        {
            if (Constants.USEC_WEAPON_LIST.Contains(weapon.Item.TemplateId))
            {
                Plugin.UsecARSystems.ApplyXp();
            }
        }
    }

    internal class ItemAttributeDisplayPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            typeof(ItemSpecificationPanel).GetMethod("method_16", BindingFlags.NonPublic | BindingFlags.Instance);

        [PatchPrefix]
        public static void Prefix(ref Item compareItem)
        {
            if (compareItem is Weapon weap)
            {
                // Change display params for USEC weapons only.
                if (Constants.USEC_WEAPON_LIST.Contains(compareItem.TemplateId))
                {
                    var skills = Plugin.Session.Profile.Skills;

                    var level = skills.UsecArsystems.Level;

                    var ergoBonus = skills.UsecArsystems.IsEliteLevel ? level * Constants.USEC_ERGO_MOD + Constants.USEC_ERGO_MOD_ELITE : level * Constants.USEC_ERGO_MOD;
                    var recoilReduction = skills.UsecArsystems.IsEliteLevel ? level * Constants.USEC_RECOIL_REDUCTION + Constants.USEC_RECOIL_REDUCTION_ELITE : level * Constants.USEC_RECOIL_REDUCTION;

                    weap.Template.Ergonomics *= (1 + ergoBonus);
                    weap.Template.RecoilForceUp *= (1 - recoilReduction);
                    weap.Template.RecoilForceBack *= (1 - recoilReduction);
                }

                // Change display params for BEAR weapons only.
                if (Constants.BEAR_WEAPON_LIST.Contains(compareItem.TemplateId))
                {
                    // TODO
                }
            }            
        }
    }
}
