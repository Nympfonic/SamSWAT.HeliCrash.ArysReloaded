using Aki.Reflection.Patching;
using EFT;
using EFT.Airdrop;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace SamSWAT.HeliCrash.ArysReloaded
{
    public class HeliCrashPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        public static void PatchPostfix(GameWorld __instance)
        {
            var gameWorld = __instance;
            var crashAvailable = gameWorld.MainPlayer.Location.ToLower() == "sandbox" || LocationScene.GetAll<AirdropPoint>().Any();
            var location = gameWorld.MainPlayer.Location;
            
            if (!crashAvailable || !BlessRNG.RngBool(Plugin.HeliCrashChance.Value)) return;
            
            var heliCrash = gameWorld.gameObject.AddComponent<HeliCrash>();
            heliCrash.Init(location);
        }
    }
}
