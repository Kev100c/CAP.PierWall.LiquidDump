#region Header
// PierLiquidDumpResearchData.cs
// Registers the research node that unlocks the CAP Pier Wall liquid dump.
//
// Reference note: Created with support from GPT (ChatGPT / GPT-5 Thinking).
#endregion

#region Usings

using Mafi;
using Mafi.Base;
using Mafi.Base.Prototypes.Machines;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;
using Mafi.Core.Research;

#endregion

namespace CAP.PierWall.LiquidDump;

/// <summary>
/// Registers the research node for the CAP Pier Wall liquid dump.
/// </summary>
internal sealed class PierLiquidDumpResearchData : IResearchNodesData
{

    #region Registration

    /// <summary>
    /// Registers the research node beside the CAP Pier Walls research node.
    /// </summary>
    public void RegisterData(ProtoRegistrator registrator)
    {
        ProtosDb prototypesDb = registrator.PrototypesDb;

        ResearchNodeProto pierWallsNode =
            prototypesDb.GetOrThrow<ResearchNodeProto>(
                PierLiquidDumpIds.ExternalResearch.UnlockPierWalls);

        OceanLiquidDumpProto machine =
            prototypesDb.GetOrThrow<OceanLiquidDumpProto>(
                PierLiquidDumpIds.Machines.PierLiquidDump);

        Vector2i researchPosition = pierWallsNode.GridPosition + new Vector2i(4, 0);

        registrator.ResearchNodeProtoBuilder
            .Start(
                "Pier wall liquid dump",
                PierLiquidDumpIds.Research.UnlockPierWallLiquidDump,
                costMonths: 32)
            .Description("Unlocks a liquid dump that must be built on Pier Wall support tiles.")
            .AddLayoutEntityToUnlock(PierLiquidDumpIds.Machines.PierLiquidDump)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierWaterDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierBrineDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierWasteWaterDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierSourWaterDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierAcidDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierToxicSlurryDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierSeawaterDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierFertilizerOrganicDumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierFertilizerChem1Dumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierFertilizerChem2Dumping)
            .AddRecipeToUnlock(PierLiquidDumpIds.Recipes.PierRedMudDumping)
            .AddIcon(machine)
            .AddParents(pierWallsNode)
            .SetGridPosition(researchPosition)
            .BuildAndAdd();
    }

    #endregion
}