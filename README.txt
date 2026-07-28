CAP Pier Wall Liquid Dump
=========================

A mod for "Captain of Industry" that adds a liquid dump that must be built on
CAP Pier Wall support tiles.

Version: 1.0.1
Tested up to: 0.8.6a
Requires: PierWallMod


Features
--------

- Adds a pier-wall liquid dump for dumping liquids into the ocean from a pier
  wall.
- Requires CAP Pier Wall support under the marked 2 x 4 support area.
- Works with compatible CAP Pier Wall variants:
  - Pier Wall (short)
  - Pier Wall (long)
  - Pier Wall (corner)
  - Pier Wall (cross)
  - Pier Wall (tee)
- Supports placement across multiple pier wall segments, so the support area can
  span segment seams.
- Prevents supported pier wall tiles from being removed while a pier-wall liquid
  dump depends on them.
- Adds a research node to unlock the pier-wall liquid dump after the CAP Pier
  Walls research.
- Uses a custom model, custom icon, liquid-colored particle effects, and the
  vanilla waste-dump sound.
- Uses no electricity.


Supported Dumping Recipes
-------------------------

- Water
- Brine
- Waste water
- Sour water
- Acid
- Toxic slurry
- Seawater
- Organic fertilizer
- Chemical fertilizer I
- Chemical fertilizer II
- Red mud


Installation
------------

Extract the mod ZIP to:

%APPDATA%\Captain of Industry\Mods\CAP-PierWall-LiquidDump

Make sure the required dependency PierWallMod is installed and enabled.

The mod folder should contain:

- manifest.json
- config.json
- CAP.PierWall.LiquidDump.dll
- AssetBundles/
- README.txt
- thumbnail.png


How to Build
------------

- Create a coastline with a maximum height of 3.
- Place the pier wall along the coastline.
- Dig down to at least -2 on the water side of the pier wall.
- Place the pump


Compatibility with Save Games
-----------------------------

This mod can be added to an existing save game.

Removing the mod from an existing save file is not supported, as the save file
may contain pier-wall liquid dump entities registered by the mod.


Configuration
-------------

This mod currently has no custom configuration options.


Build from Source
-----------------

The project targets .NET Framework 4.8 and references the Captain of Industry
managed assemblies.

To build locally:

1. Set COI_ROOT to your Captain of Industry installation directory.
2. Open CAP.PierWall.LiquidDump.sln or CAP.PierWall.LiquidDump.slnx.
3. Build the project.

When deployment is enabled, the build process copies the mod files to:

%APPDATA%\Captain of Industry\Mods\CAP-PierWall-LiquidDump


Note on AI Support
------------------

Parts of this mod were developed with the support of ChatGPT / GPT-5.5 Thinking
from OpenAI.


License
-------

This mod is licensed under the Captain of Industry Open License (COI-Open) 1.0.

See the LICENSE file for details.
