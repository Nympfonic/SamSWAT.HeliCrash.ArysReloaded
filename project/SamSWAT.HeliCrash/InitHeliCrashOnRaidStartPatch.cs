using EFT;
using EFT.Airdrop;
using HarmonyLib;
using SamSWAT.HeliCrash.ArysReloaded.Utils;
using SPT.Reflection.Patching;
using System.Linq;
using System.Reflection;

namespace SamSWAT.HeliCrash.ArysReloaded
{
    public class InitHeliCrashOnRaidStartPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }
        
        [PatchPostfix]
        public static void PatchPostfix(GameWorld __instance)
        {
            string location = __instance.MainPlayer.Location;
            bool crashAvailable = location.ToLower() == "sandbox" || LocationScene.GetAll<AirdropPoint>().Any();
            bool shouldSpawnCrash = Plugin.SpawnAllCrashSites.Value || BlessRNG.RngBool(Plugin.HeliCrashChance.Value);
            
            if (crashAvailable && shouldSpawnCrash)
            {
	            __instance.gameObject.AddComponent<HeliCrashManager>();
            }
        }
    }
}
