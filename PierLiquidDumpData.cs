#region Header
// PierLiquidDumpData.cs
// Prototype and recipe registration for the CAP Pier Wall liquid dump.
//
// Reference note: Created with support from GPT (ChatGPT / GPT-5 Thinking).
#endregion

#region Usings

using System.Linq;
using Mafi;
using Mafi.Base;
using Mafi.Base.Prototypes.Machines;
using Mafi.Collections.ImmutableCollections;
using Mafi.Core;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Animations;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Factory.Machines;
using Mafi.Core.Factory.Recipes;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using Mafi.Core.Terrain;
using Mafi.Localization;
using Mafi.Numerics;

#endregion

namespace CAP.PierWall.LiquidDump;

/// <summary>
/// Registers the pier-wall liquid dump machine and its dumping recipes.
/// </summary>
internal sealed class PierLiquidDumpData : IModData {

	#region Registration

	/// <summary>
	/// Registers all prototypes owned by this data class.
	/// </summary>
	public void RegisterData(ProtoRegistrator registrator) {
		OceanLiquidDumpProto machine = RegisterMachine(registrator);
		RegisterRecipes(registrator, machine);
	}

	#endregion

	#region Machine Registration

	/// <summary>
	/// Creates and registers the custom pier-wall liquid dump.
	/// </summary>
	private static OceanLiquidDumpProto RegisterMachine(ProtoRegistrator registrator) {
		ProtosDb prototypesDb = registrator.PrototypesDb;

		Proto.Str strings = Proto.CreateStrFormatDesc1(
			PierLiquidDumpIds.Machines.PierLiquidDump,
			"Pier wall liquid dump",
			"Dumps liquids into the ocean from a CAP Pier Wall. Requires CAP Pier Wall support under the marked support area. Works at the maximum height of {0} from the ocean level.",
			new LocStrFormatted(11.ToString()),
			"{0} is an integer specifying max height such as '5'");

		EntityLayout layout = CreateLayout(registrator);
		EntityCosts costs = Costs.Machines.WasteWaterPump.MapToEntityCosts(registrator);
		Electricity consumedPowerPerTick = Electricity.Zero;
		ImmutableArray<AnimationParams> animationParams = ImmutableArray<AnimationParams>.Empty;

		// Ocean area behind the pier wall: 2 tiles wide and 4 tiles long.
		RectangleTerrainArea2iRelative reservedOceanArea = new(
			new RelTile2i(0, 0),
			new RelTile2i(2, 4));

		HeightTilesI minGroundHeight = new(1);
		HeightTilesI maxGroundHeight = new(30);
		HeightTilesF? minDepthOverride = new HeightTilesF(-2);
		ThicknessTilesI? maxHeightOverride = new ThicknessTilesI(0);

		RelTile3f prefabOffset = new(
			(-0.25).ToFix32(),
			(-0.125).ToFix32(),
			0);

		ImmutableArray<ToolbarEntryData> categories = ImmutableArray.Create(
			registrator.GetCategory(Ids.ToolbarCategories.Waste_Fluid),
			registrator.GetCategory(Ids.ToolbarCategories.Oil_Basic),
			registrator.GetCategory(Ids.ToolbarCategories.Transports_Fluid, doesNotUnlock: true));

		ImmutableArray<ParticlesParams> particlesParams = ImmutableArray.Create(
			ParticlesParams.Loop(
				"WasteParticles",
				useUtilizationOnAlpha: false,
				null,
				(RecipeProto recipe) => recipe.AllInputs.First.Product.Graphics.Color));

		Option<string> machineSoundPrefabPath = "Assets/Base/Machines/Water/WasteDump/WasteDump_Sound.prefab";

		MachineProto.Gfx graphics = new(
			"Assets/Base/Machines/Water/WasteDump.prefab",
			categories,
			prefabOffset,
			default(Option<string>),
			particlesParams,
			default(ImmutableArray<EmissionParams>),
			machineSoundPrefabPath,
			useInstancedRendering: false,
			useSemiInstancedRendering: true);

		return prototypesDb.Add(new OceanLiquidDumpProto(
			PierLiquidDumpIds.Machines.PierLiquidDump,
			strings,
			layout,
			costs,
			consumedPowerPerTick,
			animationParams,
			reservedOceanArea,
			minGroundHeight,
			maxGroundHeight,
			graphics,
			buffersMultiplier: null,
			useAllRecipesAtStartOrAfterUnlock: false,
			computingConsumed: default(Computing),
			emissionWhenRunning: null,
			isWasteDisposal: true,
			disableLogisticsByDefault: false,
			boostCost: null,
			minDepthOverride,
			maxHeightOverride));
	}

	#endregion

	#region Layout

	/// <summary>
	/// Creates a compact liquid-dump layout with 2 x 4 ocean cells, 2 x 4 pier-wall support cells, and a 2 x 4 land-side block.
	/// </summary>
	private static EntityLayout CreateLayout(ProtoRegistrator registrator) {
		return registrator.LayoutParser.ParseLayoutOrThrow(
			new EntityLayoutParams(
				(LayoutTile tile) => tile.Constraint == LayoutTileConstraint.None
					|| tile.Constraint.HasAnyConstraints(LayoutTileConstraint.Ocean),
				new CustomLayoutToken[] {
					new(
						"~~~",
						(EntityLayoutParams p, int h) => new LayoutTokenSpec(
							heightFrom: -12,
							heightToExcl: -10,
							constraint: LayoutTileConstraint.Ocean)),
					new(
						"(S)",
						(EntityLayoutParams p, int h) => new LayoutTokenSpec(
							heightFrom: 1,
							heightToExcl: 2,
							constraint: LayoutTileConstraint.None))
				}),
			// Ocean side: 2 x 4 ocean area behind the wall.
			"~~~~~~         ",
			"~~~~~~         ",
			"~~~~~~         ",
			"~~~~~~         ",
			// Pier-wall support area: 2 x 4 cells. These cells sit above the wall collision volume.
			"(S)(S)         ",
			"(S)(S)         ",
			"(S)(S)         ",
			"(S)(S)         ",
			// Land-side machine block: 2 x 4 x 2 with two fluid input ports.
			"[2][2]         ",
			"[2][2]         ",
			"[2][2]A@<      ",
			"[2][2]B@<      ");
	}

	#endregion

	#region Recipes

	/// <summary>
	/// Registers the dumping recipes for the custom liquid dump.
	/// </summary>
	private static void RegisterRecipes(ProtoRegistrator registrator, OceanLiquidDumpProto machine) {
		Duration duration = 3.Seconds();

		registrator.RecipeProtoBuilder.Start("Water dumping", PierLiquidDumpIds.Recipes.PierWaterDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.Water)
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Brine dumping", PierLiquidDumpIds.Recipes.PierBrineDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.Brine)
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Waste water dumping", PierLiquidDumpIds.Recipes.PierWasteWaterDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.WasteWater)
			.AddOutput(10, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Sour water dumping", PierLiquidDumpIds.Recipes.PierSourWaterDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.SourWater)
			.AddOutput(20, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Acid dumping", PierLiquidDumpIds.Recipes.PierAcidDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.Acid)
			.AddOutput(20, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Toxic slurry dumping", PierLiquidDumpIds.Recipes.PierToxicSlurryDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.ToxicSlurry)
			.AddOutput(25, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Seawater dumping", PierLiquidDumpIds.Recipes.PierSeawaterDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.Seawater)
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Fertilizer dumping", PierLiquidDumpIds.Recipes.PierFertilizerOrganicDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.FertilizerOrganic)
			.AddOutput(2, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Fertilizer dumping", PierLiquidDumpIds.Recipes.PierFertilizerChem1Dumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.FertilizerChemical)
			.AddOutput(5, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Fertilizer dumping", PierLiquidDumpIds.Recipes.PierFertilizerChem2Dumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.FertilizerChemical2)
			.AddOutput(10, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();

		registrator.RecipeProtoBuilder.Start("Red mud dumping", PierLiquidDumpIds.Recipes.PierRedMudDumping, machine)
			.SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
			.AddInput(10, Ids.Products.RedMud)
			.AddOutput(10, Ids.Products.PollutedWater, "VIRTUAL")
			.SetDuration(duration)
			.EnablePartialExecution(1.Percent())
			.BuildAndAdd();
	}

	#endregion
}
