using Aki.Reflection.Patching;
using EFT;
using System.Reflection;
using SkillsExtended.Helpers;

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
}
