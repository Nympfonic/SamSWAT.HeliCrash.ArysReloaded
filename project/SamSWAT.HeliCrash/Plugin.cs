using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using SamSWAT.HeliCrash.ArysReloaded.Models;
using SamSWAT.HeliCrash.ArysReloaded.Utils;
using System.IO;
using System.Reflection;

namespace SamSWAT.HeliCrash.ArysReloaded;

[BepInPlugin("com.SamSWAT.HeliCrash.ArysReloaded", "SamSWAT's HeliCrash: Arys Reloaded", "2.3.0")]
[BepInDependency("com.SPT.core", "3.11.0")]
public class Plugin : BaseUnityPlugin
{
	internal static ConfigEntry<bool> LoggingEnabled { get; private set; }
	internal static ConfigEntry<bool> SpawnAllCrashSites { get; private set; }
	internal static ConfigEntry<int> HeliCrashChance { get; private set; }
	internal static ConfigEntry<int> CrashHasLootChance { get; private set; }
	
	internal static string Directory { get; private set; }
	internal static HeliCrashLocations HeliCrashLocations { get; private set; }
	
	private void Awake()
	{
		Utils.Logger.Initialize(Logger);
		Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		
		LocalizationService.LoadMappings();
		
		string crashSitesJsonPath = Path.Combine(Directory, "HeliCrashLocations.json");
		HeliCrashLocations = LoadJson<HeliCrashLocations>(crashSitesJsonPath);
		
		SetupDebugConfigBindings();
		SetupMainConfigBindings();
		
		new InitHeliCrashOnRaidStartPatch().Enable();
	}
	
	private void SetupDebugConfigBindings()
	{
		LoggingEnabled = Config.Bind(
			LocalizationService.GetString("debugSettings"),
			LocalizationService.GetString("enableLogging"),
			false);
		
		SpawnAllCrashSites = Config.Bind(
			LocalizationService.GetString("debugSettings"),
			LocalizationService.GetString("spawnAllCrashSites"),
			false,
			LocalizationService.GetString("spawnAllCrashSites_desc"));
	}
	
	private void SetupMainConfigBindings()
	{
		HeliCrashChance = Config.Bind(
			LocalizationService.GetString("mainSettings"),
			LocalizationService.GetString("crashSiteSpawnChance"),
			10,
			new ConfigDescription(
				LocalizationService.GetString("crashSiteSpawnChance_desc"),
				new AcceptableValueRange<int>(0, 100)));
		
		CrashHasLootChance = Config.Bind(
			LocalizationService.GetString("mainSettings"),
			LocalizationService.GetString("crashHasLootChance"),
			100,
			new ConfigDescription(
				LocalizationService.GetString("crashHasLootChance_desc"),
				new AcceptableValueRange<int>(0, 100)));
	}
	
	internal static T LoadJson<T>(string path)
	{
		string json = File.ReadAllText(path);
		return JsonConvert.DeserializeObject<T>(json);
	}
}