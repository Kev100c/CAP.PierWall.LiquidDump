#region Header
// PierLiquidDumpIds.cs
// Central ID definitions for CAP Pier Wall Liquid Dump.
//
// Reference note: Created with support from GPT (ChatGPT / GPT-5 Thinking).
#endregion

#region Usings

using Mafi.Base;
using MachineID = Mafi.Core.Factory.Machines.MachineProto.ID;
using RecipeID = Mafi.Core.Factory.Recipes.RecipeProto.ID;

#endregion

namespace CAP.PierWall.LiquidDump;

/// <summary>
/// Holds all prototype IDs created by the mod.
/// </summary>
public static partial class PierLiquidDumpIds {

	#region Machine IDs

	/// <summary>
	/// Machine IDs created by this mod.
	/// </summary>
	public static partial class Machines {

		/// <summary>
		/// Custom liquid dump that requires CAP Pier Wall support in its support area.
		/// </summary>
		public static readonly MachineID PierLiquidDump = Ids.Machines.CreateId("CAP_PierWall_LiquidDump");
	}

	#endregion

	#region Recipe IDs

	/// <summary>
	/// Recipe IDs created by this mod.
	/// </summary>
	public static partial class Recipes {

		public static readonly RecipeID PierWaterDumping = Ids.Recipes.CreateId("CAP_PierWall_WaterDumping");
		public static readonly RecipeID PierBrineDumping = Ids.Recipes.CreateId("CAP_PierWall_BrineDumping");
		public static readonly RecipeID PierWasteWaterDumping = Ids.Recipes.CreateId("CAP_PierWall_WasteWaterDumping");
		public static readonly RecipeID PierSourWaterDumping = Ids.Recipes.CreateId("CAP_PierWall_SourWaterDumping");
		public static readonly RecipeID PierAcidDumping = Ids.Recipes.CreateId("CAP_PierWall_AcidDumping");
		public static readonly RecipeID PierToxicSlurryDumping = Ids.Recipes.CreateId("CAP_PierWall_ToxicSlurryDumping");
		public static readonly RecipeID PierSeawaterDumping = Ids.Recipes.CreateId("CAP_PierWall_SeawaterDumping");
		public static readonly RecipeID PierFertilizerOrganicDumping = Ids.Recipes.CreateId("CAP_PierWall_FertilizerOrganicDumping");
		public static readonly RecipeID PierFertilizerChem1Dumping = Ids.Recipes.CreateId("CAP_PierWall_FertilizerChem1Dumping");
		public static readonly RecipeID PierFertilizerChem2Dumping = Ids.Recipes.CreateId("CAP_PierWall_FertilizerChem2Dumping");
		public static readonly RecipeID PierRedMudDumping = Ids.Recipes.CreateId("CAP_PierWall_RedMudDumping");
	}

	#endregion
}
