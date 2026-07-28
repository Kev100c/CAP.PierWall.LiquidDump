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

        Proto.Str strings = Proto.CreateStr(
            PierLiquidDumpIds.Machines.PierLiquidDump,
            "Pier wall liquid dump",
            LocalizationManager.CreateAlreadyLocalizedStr(
                $"{PierLiquidDumpIds.Machines.PierLiquidDump.Value}__desc",
                "Dumps liquids into the ocean from a Pier Wall. Requires Pier Wall support under the marked support area."));

        EntityLayout layout = CreateLayout(registrator);
		EntityCosts costs = ((EntityCostsTpl)Costs.Build.CP2(20)).MapToEntityCosts(registrator);
        Electricity consumedPowerPerTick = Electricity.Zero;
		ImmutableArray<AnimationParams> animationParams = ImmutableArray<AnimationParams>.Empty;

		// Ocean area behind the pier wall: 2 tiles wide and 4 tiles long.
		RectangleTerrainArea2iRelative reservedOceanArea = new(
			new RelTile2i(-5, -2),		//Offset
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
			registrator.GetCategory(Ids.ToolbarCategories.Waste_Fluid, doesNotUnlock: true),
			registrator.GetCategory(Ids.ToolbarCategories.Oil_Basic, doesNotUnlock: true),
			registrator.GetCategory(Ids.ToolbarCategories.Transports_Fluid, doesNotUnlock: true));

		ImmutableArray<ParticlesParams> particlesParams = ImmutableArray.Create(
			ParticlesParams.Loop(
				"WasteParticles_A",
				useUtilizationOnAlpha: false,
                recipesSelector: null,
                (RecipeProto recipe) => recipe.AllInputs.First.Product.Graphics.Color),
            ParticlesParams.Loop(
                "WasteParticles_B",
                useUtilizationOnAlpha: false,
                recipesSelector: null,
                (RecipeProto recipe) => recipe.AllInputs.First.Product.Graphics.Color));

        // Reusing the waste dump sound prefab for the pier liquid dump, as it fits thematically and there are no custom sounds needed.
        Option<string> machineSoundPrefabPath =
            "Assets/Base/Machines/Water/WasteDump/WasteDump_Sound.prefab";

        // Custom icon for the pier liquid dump, as the default waste dump icon may not clearly represent the new machine's function and context.
        Option<string> customIconPath =
			"Assets/CAP/PierWall/LiquidDump/Icons/CAP_PierWall_LiquidDump_icon.png";

        // Note: Instanced rendering is disabled for this machine due to the use of particle effects and potential visual complexity, which may not benefit from instancing and could require unique rendering per instance.
        MachineProto.Gfx graphics = new(
            "Assets/CAP/PierWall/LiquidDump/CAP_PierWall_LiquidDump.prefab",
            categories,
            prefabOffset,
            customIconPath,
            particlesParams,
            default(ImmutableArray<EmissionParams>),
            machineSoundPrefabPath,
            useInstancedRendering: false,
            useSemiInstancedRendering: false);

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
							heightFrom: -11,
							heightToExcl: -10,
							constraint: LayoutTileConstraint.Ocean)),
					new(
						"(S)",
						(EntityLayoutParams p, int h) => new LayoutTokenSpec(
							heightFrom: -11,
							heightToExcl: -10,
							constraint: LayoutTileConstraint.None))
				}),
            "~~~~~~(S)(S)[2][2]         ",
            "~~~~~~(S)(S)[2][2]A@<      ",
            "~~~~~~(S)(S)[2][2]B@<      ",
            "~~~~~~(S)(S)[2][2]         ");
	}

	#endregion

	#region Recipes

	/// <summary>
	/// Registers the dumping recipes for the custom liquid dump.
	/// </summary>
	private static void RegisterRecipes(ProtoRegistrator registrator, OceanLiquidDumpProto machine) {
		Duration duration = 3.Seconds();
		const int DumpThroughputMultiplier = 2; // Multiplier to increase the input amounts for a more impactful dumping experience, while still allowing partial execution to be useful for smaller amounts.


        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierWaterDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.Water)
            .BuildAndAdd()
            .BindTo(
        machine,
        duration,
        minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierBrineDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.Brine)
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierWasteWaterDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.WasteWater)
            .AddOutput(10 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierSourWaterDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.SourWater)
            .AddOutput(20 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierAcidDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.Acid)
            .AddOutput(20 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierToxicSlurryDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.ToxicSlurry)
            .AddOutput(25 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierSeawaterDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.Seawater)
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierFertilizerOrganicDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.FertilizerOrganic)
            .AddOutput(2 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierFertilizerChem1Dumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.FertilizerChemical)
            .AddOutput(5 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierFertilizerChem2Dumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.FertilizerChemical2)
            .AddOutput(10 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());

        registrator.RecipeProtoBuilder.Start(PierLiquidDumpIds.Recipes.PierRedMudDumping)
            .SetProductsDestroyReason(DestroyReason.DumpedOnTerrain)
            .AddInput(10 * DumpThroughputMultiplier, Ids.Products.RedMud)
            .AddOutput(10 * DumpThroughputMultiplier, Ids.Products.PollutedWater, "VIRTUAL")
            .BuildAndAdd()
            .BindTo(
                machine,
                duration,
                minPartialUtilization: 1.Percent());
    }

	#endregion
}
