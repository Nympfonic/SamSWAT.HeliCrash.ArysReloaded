using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using SamSWAT.HeliCrash.ArysReloaded.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SamSWAT.HeliCrash.ArysReloaded;

internal class LootController
{
	private readonly LootableContainer _container;
	private readonly ItemFactoryClass _itemFactory;
	private readonly PoolManagerClass _poolManager;
	private readonly ProfileEndpointFactoryAbstractClass _profileEndpointFactory;
	
	public LootController(LootableContainer container)
	{
		_container = container;
		
		_itemFactory = Singleton<ItemFactoryClass>.Instance
			?? throw new NullReferenceException("LootController._itemFactory is null");
		
		_poolManager = Singleton<PoolManagerClass>.Instance
			?? throw new NullReferenceException("LootController._poolManager is null");
		
		_profileEndpointFactory = (ProfileEndpointFactoryAbstractClass)
			Singleton<ClientApplication<ISession>>.Instance.GetClientBackEndSession();
	}
	
	public async Task<Item> CreateContainer(string lootTemplateId = null)
	{
		Result<AirdropLootResponse> lootResponseResult = await _profileEndpointFactory.LoadLootContainerData(lootTemplateId);
		AirdropLootResponse lootResponse = lootResponseResult.Value;
		
		if (Plugin.LoggingEnabled.Value && lootResponse?.data == null)
		{
			throw new NullReferenceException("Heli crash site loot response is null");
		}
		
		Item containerItem = _itemFactory.FlatItemsToTree(lootResponse.data)
			.Items[lootResponse.data[0]._id];
		LootItem.CreateLootContainer(
			_container,
			containerItem,
			LocalizationService.GetString("containerName"),
			Singleton<GameWorld>.Instance);
		
		return containerItem;
	}
	
	public async Task AddLoot(Item containerItem)
	{
		try
		{
			ResourceKey[] resources;
			if (containerItem is ContainerData container)
			{
				resources = container.GetAllItemsFromCollection()
					.SelectMany(item => item.Template.AllResources)
					.ToArray();
			}
			else
			{
				resources = containerItem.Template.AllResources.ToArray();
			}
			
			await _poolManager.LoadBundlesAndCreatePools(
				PoolManagerClass.PoolsCategory.Raid,
				PoolManagerClass.AssemblyType.Local,
				resources,
				JobPriorityClass.Immediate);
		}
		catch (Exception ex)
		{
			Logger.LogError($"Failed to add loot to container! {ex.Message}\n{ex.StackTrace}");
		}
	}
}