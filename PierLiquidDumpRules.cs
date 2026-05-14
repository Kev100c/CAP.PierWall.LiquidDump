#region Header
// PierLiquidDumpRules.cs
// Shared filters and support-area definitions for the CAP Pier Wall liquid dump.
//
// Reference note: Created with support from GPT (ChatGPT / GPT-5 Thinking).
#endregion

#region Usings

using System;
using Mafi.Core;
using Mafi.Core.Entities.Static;
using Mafi.Core.Entities.Static.Layout;

#endregion

namespace CAP.PierWall.LiquidDump;

/// <summary>
/// Contains deterministic rules for identifying the custom liquid dump and compatible CAP Pier Wall support.
/// </summary>
internal static class PierLiquidDumpRules {

	#region Constants

	/// <summary>
	/// ID parts required on a compatible pier wall prototype.
	/// Adjust these strings if the existing CAP Pier Wall mod uses different prototype IDs.
	/// </summary>
	private static readonly string[] PierWallIdRequiredParts = {
		"CAP",
		"PierWall"
	};

	/// <summary>
	/// Support cells that must each contain a compatible CAP Pier Wall tile.
	/// The cells match the (S) support layer in PierLiquidDumpData.CreateLayout.
	/// The support footprint is 2 x 4, matching the intended liquid-dump wall contact area.
	/// Each support cell may be provided by a different Pier Wall entity, allowing placement across segment seams.
	/// </summary>
	public static readonly RelTile2i[] RequiredPierWallSupportTiles = {
		new(0, 4),
		new(1, 4),
		new(0, 5),
		new(1, 5),
		new(0, 6),
		new(1, 6),
		new(0, 7),
		new(1, 7)
	};

	#endregion

	#region Prototype Checks

	/// <summary>
	/// Returns whether the supplied prototype is the custom CAP Pier Wall liquid dump.
	/// </summary>
	public static bool IsPierLiquidDumpProto(ILayoutEntityProto proto) {
		return proto.Id.ToString().Equals(
			PierLiquidDumpIds.Machines.PierLiquidDump.Value,
			StringComparison.Ordinal);
	}

	/// <summary>
	/// Returns whether the supplied entity is a compatible CAP Pier Wall support entity.
	/// </summary>
	public static bool IsCompatiblePierWall(IStaticEntity entity) {
		string id = entity.Prototype.Id.ToString();

		foreach (string requiredPart in PierWallIdRequiredParts) {
			if (id.IndexOf(requiredPart, StringComparison.OrdinalIgnoreCase) < 0) {
				return false;
			}
		}

		return true;
	}

	#endregion

	#region Occupancy Checks

	/// <summary>
	/// Returns whether an entity occupies the supplied world tile on the XY plane.
	/// The support rule intentionally checks cells instead of requiring one specific Pier Wall entity.
	/// </summary>
	public static bool EntityOccupiesTile(IStaticEntity entity, Tile2i worldTile) {
		foreach (OccupiedTileRelative occupiedTile in entity.OccupiedTiles) {
			Tile2i occupiedWorldTile = entity.CenterTile.Xy + occupiedTile.RelCoord;

			if (occupiedWorldTile == worldTile) {
				return true;
			}
		}

		return false;
	}

	#endregion
}
