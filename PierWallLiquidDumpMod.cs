#region Header
// PierWallLiquidDumpMod.cs
// Mod entry point for CAP Pier Wall Liquid Dump.
//
// Reference note: Created with support from GPT (ChatGPT / GPT-5 Thinking).
#endregion

#region Usings

using Mafi;
using Mafi.Base;
using Mafi.Collections;
using Mafi.Core;
using Mafi.Core.Mods;

#endregion

namespace CAP.PierWall.LiquidDump;

/// <summary>
/// Registers the CAP Pier Wall liquid dump prototypes and recipes.
/// </summary>
public sealed class PierWallLiquidDumpMod : DataOnlyMod {

	#region Construction

	/// <summary>
	/// Creates the mod instance when Captain of Industry loads the mod assembly.
	/// </summary>
	public PierWallLiquidDumpMod(ModManifest manifest) : base(manifest) {
		Log.Info("CAP.PierWall.LiquidDump: constructed");
	}

	#endregion

	#region Prototype Registration

	/// <summary>
	/// Registers custom prototypes for the mod.
	/// </summary>
	public override void RegisterPrototypes(ProtoRegistrator registrator) {
		Log.Info("CAP.PierWall.LiquidDump: registering prototypes");

		registrator.RegisterData<PierLiquidDumpData>();
	}

	#endregion

	#region Configuration Migration

	/// <summary>
	/// Handles JSON configuration migrations between mod versions.
	/// </summary>
	public override void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues) {
		// No custom configuration fields are used in version 0.0.1.
	}

	#endregion
}
