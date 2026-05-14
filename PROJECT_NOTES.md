# CAP Pier Wall Liquid Dump - project notes

## Intent

This project is an adapted version of the COI `ExampleMod` layout. It adds a custom `OceanLiquidDumpProto` based machine which requires CAP Pier Wall support cells.

## Important behavior

- No global `IgnoreForCollisions` patch is used.
- The support layer uses the custom `(S)` token at height 1..2, so it should sit above the current CAP Pier Wall collision range if the wall uses `heightToExcl: 1`.
- Every support cell is checked individually with `OccupancyManager.TryGetOccupyingEntityAt`.
- Adjacent support cells may be provided by different Pier Wall entities, allowing the dump to be placed across seams between two wall segments.

## Tuning points

Edit `PierLiquidDumpRules.cs`:

- `PierWallIdRequiredParts`: change this if your Pier Wall prototype ID does not contain both `CAP` and `PierWall`.
- `SupportCheckZOffsets`: change if the support check probes the wrong height.
- `RequiredPierWallSupportTiles`: must match the `(S)` area in `PierLiquidDumpData.CreateLayout`.

Edit `PierLiquidDumpData.cs`:

- `CreateLayout`: changes occupied/support/port tiles.
- `reservedOceanArea`: changes the required ocean area.
- `graphics`: currently reuses the vanilla Waste Dump prefab.

## Known compile-risk points

The code is written against the decompiled COI 0.8.4 API details supplied in chat. If your local API differs, the most likely files to need tiny signature fixes are:

- `PierLiquidDumpPlacementValidator.cs`
- `PierLiquidDumpRemovalValidator.cs`

Send the exact compiler error list to adjust the source.


## v003 layout adjustment

The support footprint was changed from 4 x 2 to 2 x 4 and the land-side machine block was aligned with the ocean/support column. This follows the intended liquid-dump geometry: 2 x 4 ocean area, 2 x 4 wall-support area, and 2 x 4 land-side block.
