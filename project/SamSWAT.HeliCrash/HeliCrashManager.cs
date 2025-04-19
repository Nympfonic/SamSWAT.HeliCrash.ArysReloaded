using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SamSWAT.HeliCrash.ArysReloaded.Models;
using SamSWAT.HeliCrash.ArysReloaded.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace SamSWAT.HeliCrash.ArysReloaded;

public class HeliCrashManager : MonoBehaviourSingleton<HeliCrashManager>
{
	private async void Start()
	{
		try
		{
			await Initialize(Singleton<GameWorld>.Instance.MainPlayer.Location);
		}
		catch (Exception ex)
		{
			Utils.Logger.LogError($"Failed to initialize heli crash site: {ex.Message}\n{ex.StackTrace}");
		}
	}
	
	private async Task Initialize(string location)
	{
		List<Location> heliLocations = GetCrashSiteLocations(location);
		if (heliLocations == null)
		{
			Utils.Logger.LogError("Invalid map or crash location data, aborting heli crash initialization!");
			return;
		}

		string heliBundlePath = Path.Combine(Plugin.Directory, "sikorsky_uh60_blackhawk.bundle");
		GameObject heliPrefab = await LoadPrefabAsync(heliBundlePath);

		if (Plugin.SpawnAllCrashSites.Value)
		{
			int count = heliLocations.Count;
			var heliObjects = new GameObject[count];
			
			for (var i = 0; i < count; i++)
			{
				heliObjects[i] = await CreateCrashSite(heliLocations[i], heliPrefab);
			}
			
			// Enable crash site objects in batches to avoid stutters and unnecessary draw calls
			StartCoroutine(BatchEnable(heliObjects));
		}
		else
		{
			Location chosenLocation = heliLocations.SelectRandom();
			bool spawnWithLoot = !chosenLocation.Unreachable && BlessRNG.RngBool(Plugin.CrashHasLootChance.Value);
			
			GameObject heli = await CreateCrashSite(
				chosenLocation,
				heliPrefab,
				spawnWithLoot);
			heli.SetActive(true);
		}
	}
	
	private static async Task<GameObject> CreateCrashSite(Location location, GameObject heliPrefab, bool withLoot = false)
	{
		GameObject choppa = Instantiate(
			heliPrefab,
			location.Position, 
			Quaternion.Euler(location.Rotation));
		
		// CarveNavMesh(choppa);
		
		var container = choppa.GetComponentInChildren<EFT.Interactive.LootableContainer>();
		if (withLoot)
		{
			await CreateLoot(container);
		}
		else
		{
			// Disable the container game object
			container.transform.parent.gameObject.SetActive(false);
		}
		
		if (Plugin.LoggingEnabled.Value)
		{
			var logMessage = $"Heli crash site spawned at {location.Position.ToString()}";
			Utils.Logger.LogWarning(logMessage);
		}
		
		return choppa;
	}
	
	// NavMeshObstacle doesn't work on Colliders other than Box or Capsule Colliders
	// TODO: Change main collider from MeshCollider to BoxCollider
	// private static void CarveNavMesh(GameObject choppa)
	// {
	// 	var navMeshObstacle = choppa.transform.GetChild(0).GetChild(5) .gameObject.AddComponent<NavMeshObstacle>();
	// 	var collider = navMeshObstacle.GetComponent<Collider>();
	// 	
	// 	navMeshObstacle.center = collider.bounds.center;
	// 	navMeshObstacle.size = collider.bounds.size;
	// 	
	// 	// Toggle to force an update on carving the nav mesh
	// 	navMeshObstacle.carving = false;
	// 	navMeshObstacle.carving = true;
	// }
	
	private static async Task CreateLoot(EFT.Interactive.LootableContainer container)
	{
		var lootController = new LootController(container);
		Item containerItem = await lootController.CreateContainer();
		await lootController.AddLoot(containerItem);
	}
	
	private static IEnumerator BatchEnable(GameObject[] objectsToEnable, int batchSize = 10)
	{
		var count = 0;
		int arrayLength = objectsToEnable.Length;
		
		for (var i = 0; i < arrayLength; i++)
		{
			objectsToEnable[i].SetActive(true);
			
			if (i >= arrayLength - 1)
			{
				break;
			}
			
			count++;
			
			if (count >= batchSize)
			{
				count = 0;
				yield return null;
			}
		}
	}
	
	private static List<Location> GetCrashSiteLocations(string map)
	{
		List<Location> locations;
		
		switch (map.ToLower())
		{
			case "bigmap":
				locations = Plugin.HeliCrashLocations.Customs;
				break;
			case "interchange":
				locations = Plugin.HeliCrashLocations.Interchange;
				break;
			case "rezervbase":
				locations = Plugin.HeliCrashLocations.Rezerv;
				break;
			case "shoreline":
				locations = Plugin.HeliCrashLocations.Shoreline;
				break;
			case "woods":
				locations = Plugin.HeliCrashLocations.Woods;
				break;
			case "lighthouse":
				locations = Plugin.HeliCrashLocations.Lighthouse;
				break;
			case "tarkovstreets":
				locations = Plugin.HeliCrashLocations.StreetsOfTarkov;
				break;
			case "sandbox":
				locations = Plugin.HeliCrashLocations.GroundZero;
				break;
			case "develop":
				locations = Plugin.HeliCrashLocations.Develop;
				break;
			default:
				return null;
		}
		
		return locations;
	}
	
	private static async Task<GameObject> LoadPrefabAsync(string bundlePath)
	{
		AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
		while (!bundleLoadRequest.isDone)
		{
			await Task.Yield();
		}
		
		AssetBundle bundle = bundleLoadRequest.assetBundle;
		if (bundle == null)
		{
			Utils.Logger.LogError("Failed to load UH-60 Blackhawk bundle");
			return null;
		}
		
		AssetBundleRequest assetLoadRequest = bundle.LoadAllAssetsAsync<GameObject>();
		while (!assetLoadRequest.isDone)
		{
			await Task.Yield();
		}
		
		var requestedGo = (GameObject)assetLoadRequest.allAssets[0];
		if (requestedGo == null)
		{
			Utils.Logger.LogError("Failed to load UH-60 Blackhawk asset");
			return null;
		}
		
		requestedGo.SetActive(false);
		bundle.Unload(false);
		
		return requestedGo;
	}
}