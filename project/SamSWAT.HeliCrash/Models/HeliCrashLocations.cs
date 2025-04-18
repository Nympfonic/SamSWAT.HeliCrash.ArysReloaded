using Newtonsoft.Json;
using System.Collections.Generic;

namespace SamSWAT.HeliCrash.ArysReloaded.Models;

[JsonObject]
[method: JsonConstructor]
public class HeliCrashLocations(
	[JsonProperty("Customs")] List<Location> customs,
	[JsonProperty("Woods")] List<Location> woods,
	[JsonProperty("Interchange")] List<Location> interchange,
	[JsonProperty("Lighthouse")] List<Location> lighthouse,
	[JsonProperty("Rezerv")] List<Location> reserve,
	[JsonProperty("Shoreline")] List<Location> shoreline,
	[JsonProperty("StreetsOfTarkov")] List<Location> streets,
	[JsonProperty("GroundZero")] List<Location> groundzero,
	[JsonProperty("Develop")] List<Location> develop)
{
	public List<Location> Customs { get; } = customs;
	public List<Location> Woods { get; } = woods;
	public List<Location> Interchange { get; } = interchange;
	public List<Location> Lighthouse { get; } = lighthouse;
	public List<Location> Rezerv { get; } = reserve;
	public List<Location> Shoreline { get; } = shoreline;
	public List<Location> StreetsOfTarkov { get; } = streets;
	public List<Location> GroundZero { get; } = groundzero;
	public List<Location> Develop { get; } = develop;
}