#region Header
// PierLiquidDumpRemovalValidator.cs
// Prevents removing a CAP Pier Wall while it supports a CAP Pier Wall liquid dump.
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
/// Removal validator that keeps supported pier liquid dumps from becoming unsupported when a wall is removed.
/// </summary>
[GlobalDependency(RegistrationMode.AsEverything, false, false)]
public sealed class PierLiquidDumpRemovalValidator : IEntityRemovalValidator<IStaticEntity> {

	#region Fields

	private readonly EntitiesManager m_entitiesManager;

	#endregion

	#region Construction

	/// <summary>
	/// Creates the validator with access to the current world entities.
	/// </summary>
	public PierLiquidDumpRemovalValidator(EntitiesManager entitiesManager) {
		m_entitiesManager = entitiesManager;
	}

	#endregion

	#region IEntityRemovalValidator

	/// <summary>
	/// Uses the default validation priority. This keeps the validator compatible with COI's EntityValidatorPriority type.
	/// </summary>
	public EntityValidatorPriority Priority => default(EntityValidatorPriority);

	/// <summary>
	/// Blocks removal of a compatible pier wall when a custom liquid dump depends on at least one of its cells.
	/// </summary>
	public EntityValidationResult CanRemove(IStaticEntity entity, EntityRemoveReason reason) {
		if (!PierLiquidDumpRules.IsCompatiblePierWall(entity)) {
			return EntityValidationResult.Success;
		}

		if (SupportsAnyPierLiquidDump(entity)) {
			return EntityValidationResult.CreateError("Remove the pier wall liquid dump first.".AsLoc());
		}

		return EntityValidationResult.Success;
	}

    #endregion

    #region Support Checks

    /// <summary>
    /// Checks whether any existing custom pier liquid dump is supported by the supplied wall entity.
    /// </summary>
    private bool SupportsAnyPierLiquidDump(IStaticEntity wallEntity)
    {
        foreach (LayoutEntity entity in m_entitiesManager.GetAllEntitiesOfType<LayoutEntity>())
        {
            if (entity.Id == wallEntity.Id)
            {
                continue;
            }

            if (!PierLiquidDumpRules.IsPierLiquidDumpProto(entity.Prototype))
            {
                continue;
            }

            foreach (RelTile2i supportTile in PierLiquidDumpRules.RequiredPierWallSupportTiles)
            {
                Tile2i worldTile = entity.Prototype.Layout.Transform(supportTile, entity.Transform);

                if (PierLiquidDumpRules.EntityOccupiesTile(wallEntity, worldTile))
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion
}
