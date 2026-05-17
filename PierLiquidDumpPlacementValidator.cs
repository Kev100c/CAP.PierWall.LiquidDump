#region Header
// PierLiquidDumpPlacementValidator.cs
// Validates that the CAP Pier Wall liquid dump is supported by CAP Pier Wall cells.
//
// Reference note: Created with support from GPT (ChatGPT / GPT-5 Thinking).
#endregion

#region Usings

using Mafi;
using Mafi.Core;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Static;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Entities.Validators;
using Mafi.Localization;

#endregion

namespace CAP.PierWall.LiquidDump;

/// <summary>
/// Addition validator for the custom liquid dump support area.
/// </summary>
/// <remarks>
/// Creates the validator with access to the current world entities.
/// </remarks>
[GlobalDependency(RegistrationMode.AsEverything, false, false)]
public sealed class PierLiquidDumpPlacementValidator(EntitiesManager entitiesManager) : IEntityAdditionValidator<LayoutEntityAddRequest> {

	#region Fields

	private readonly EntitiesManager m_entitiesManager = entitiesManager;

    #endregion
    #region Construction

    #endregion

    #region IEntityAdditionValidator

    /// <summary>
    /// Uses the default validation priority. This keeps the validator compatible with COI's EntityValidatorPriority type.
    /// </summary>
    public EntityValidatorPriority Priority => default;

	/// <summary>
	/// Checks whether the liquid dump is fully supported by compatible pier-wall cells.
	/// </summary>
	public EntityValidationResult CanAdd(LayoutEntityAddRequest request) {
		if (!PierLiquidDumpRules.IsPierLiquidDumpProto(request.Proto)) {
			return EntityValidationResult.Success;
		}

		return HasRequiredPierWallSupport(request)
			? EntityValidationResult.Success
			: EntityValidationResult.CreateError("Requires CAP Pier Wall support under the marked support area.".AsLoc());
	}

	#endregion

	#region Support Checks

	/// <summary>
	/// Checks every support cell. Cells may be supported by different adjacent Pier Wall entities.
	/// </summary>
	private bool HasRequiredPierWallSupport(LayoutEntityAddRequest request) {
		foreach (RelTile2i supportTile in PierLiquidDumpRules.RequiredPierWallSupportTiles) {
			Tile2i worldTile = request.Layout.Transform(supportTile, request.Transform);

			if (!TryFindCompatiblePierWallAt(worldTile)) {
				if (request.RecordTileErrorsAndMetadata) {
					request.GetAdditionalErrorTilesStorage().Add(worldTile);
				}

				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Searches all static entities for a compatible Pier Wall occupying the requested support cell.
	/// </summary>
	private bool TryFindCompatiblePierWallAt(Tile2i worldTile) {
		foreach (IStaticEntity entity in m_entitiesManager.GetAllEntitiesOfType<IStaticEntity>()) {
			if (!PierLiquidDumpRules.IsCompatiblePierWall(entity)) {
				continue;
			}

			if (PierLiquidDumpRules.EntityOccupiesTile(entity, worldTile)) {
				return true;
			}
		}

		return false;
	}

	#endregion
}
