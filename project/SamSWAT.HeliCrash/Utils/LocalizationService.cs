using System;
using System.Collections.Generic;
using System.IO;

namespace SamSWAT.HeliCrash.ArysReloaded.Utils;

internal static class LocalizationService
{
	private static Dictionary<string, string> s_mappings;
	
	public static void LoadMappings()
	{
		string path = Path.Combine(Plugin.Directory, "LocalizationMappings.jsonc");
		s_mappings = Plugin.LoadJson<Dictionary<string, string>>(path);
	}

	public static string GetString(string key)
	{
		if (s_mappings == null)
		{
			throw new InvalidOperationException(
				"[SamSWAT.HeliCrash.ArysReloaded] Localization mappings not yet loaded! Load it first with LocalizationManager.LoadMappings()");
		}
		
		return s_mappings[key];
	}
}