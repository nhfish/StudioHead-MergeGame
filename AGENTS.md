Purpose
-------
This document describes the architecture, core systems, and design rules for **Studio Head** (also referred to as "Studio Merge Game").
It helps agents such as Codex understand project conventions and current gameplay logic so contributions stay consistent.

Game Concept
------------
Mobile portrait (9×16) merge and management game. Players operate a film studio by merging department items on a grid, selecting talent and soundstages, and releasing movies for profit and fame.

Core Gameplay Loops
-------------------
- Drag-and-merge department items on a grid (10 tiers).
- Fill required and optional slots on soundstages with merged items.
- Select up to three talent cards (writer, director, actor) from the Headshot Binder UI.
- Start a timed production. Optionally play the Dailies puzzle to boost rewards.
- Choose a distribution method when the movie finishes.
- Earn Money, Gems, Tickets, and Fans.
- Spend earnings on crates, soundstage upgrades, talent contracts, and overflow slots.
- Long‑term goals: complete the film archive, achieve 5‑star movies, raise studio fame, and explore sequels.

Architecture & Conventions
--------------------------
- **MonoBehaviours & ScriptableObjects** drive gameplay. Core data types such as `DepartmentItemData`, `CrateData`, and `MovieRecipeData` are ScriptableObjects.
- **Singletons** use `FindFirstObjectByType<T>()` for lookups (e.g. `EconomyManager`, `ProductionManager`, `DistributionQueueManager`).
- **UI** uses Unity UI + TextMeshPro. Key prefabs include `CurrencyPanel`, `OverflowPanel`, `MovieRecipePanel`, and `TalentListPanel`.
- **Grid** managed by `GridManager` with registration via `RegisterTile` / `UnregisterTile`. Only one `IGridOccupant` per cell.
- **Drag Behaviour** clamps world position to the screen and always snaps to the nearest grid cell on release.

Grid & Merge System
-------------------
- `GridManager` defines spacing, cell lookup, and visual tile generation.
- `MergeTile` implements `IGridOccupant`. Tiles can be dragged, merged, and snap to grid.
- `DepartmentCrateSpawner` is also an occupant. On tap it spawns a `MergeTile` in a free cell. Crates disappear when empty.
- `UniversalCrateSystem` periodically drops department crates using weighted tier chances.
- `InventoryOverflowManager` stores items when no grid cell is free. The slide‑up `OverflowPanel` lets players recover or discard items.

Talent & Production
-------------------
- `TalentInventory` holds `TalentCard` instances created from `TalentBaseData` assets. Cards track uses and locking.
- `MovieRecipePanel` builds a `MovieRecipe` using submitted items and selected talent.
- `ProductionManager` processes a recipe over time and notifies `DailiesManager` when milestones allow a Dailies attempt.
- The optional **Dailies Puzzle** (`DailiesBoardManager`) is a 4×4 merge mini‑game that awards bonus score and multiplier.
- When production completes, `DistributionQueueManager` presents the `DistributionPanel` to choose payout type. Rewards funnel through `RewardManager` and archived via `FilmArchiveManager`. `FranchiseManager` tracks sequel counts for recurring titles.

Economy & Shop
--------------
- `EconomyManager` tracks `Money`, `Gems`, and `Tickets`. `CurrencyPanel` displays amounts and listens for changes.
- `DepartmentCrateManager` handles crate purchases, era upgrades, and integrates with `UniversalCrateSystem`.
- `ShopManager` sells crates or department upgrades using the multi‑currency system.

Other Systems
-------------
- `MainMenuUI` with `SettingsPanel` serves as initial navigation.
- `ContinuousMapNavigation` (see `Docs/ContinuousMapNavigation.md`) provides camera panning between map anchors.
- `FilmArchiveManager` persists completed recipes via `PlayerPrefs`.
- `UniversalCrateSystem` and `DepartmentCrateManager` share the same crate prefab.

Coding Standards
----------------
- Use `FindFirstObjectByType<T>()` for singleton access rather than custom static lookups.
- Always call `RegisterTile` / `UnregisterTile` when moving or destroying grid occupants.
- Clamp drags to screen edges; release always snaps to the closest valid grid cell.
- Load prefabs using `Resources.Load` for items and tiles (current approach).
- Text elements should use TextMeshPro.

Planned / Future Work
---------------------
- Evaluate a tweening library (e.g. DOTween) for smoother snap, drag, and UI animations.
- Expand `EconomyManager` with passive income, IAP hooks, and ad rewards via the ScreeningRoom.
- Finish leaderboards and studio fame ranking systems.
- Expand talent acquisition via a Contract Office screen and implement rarity progression.
- More dynamic events (seasonal boosts, genre trends) and franchise momentum for sequels.

Repository Structure (partial)
------------------------------
- `Assets/_Game/Scripts/Grid/` – grid logic (`GridManager`, `DepartmentCrateSpawner`, `MergeTile`, etc.)
- `Assets/_Game/Scripts/Data/` – ScriptableObject definitions (`DepartmentItemData`, `MovieRecipeData`, etc.)
- `Assets/_Game/Scripts/Managers/` – gameplay managers (`EconomyManager`, `ProductionManager`, `DistributionQueueManager`, `InventoryOverflowManager`, `FranchiseManager`, `FilmArchiveManager`, `UniversalCrateSystem`)
- `Assets/_Game/Scripts/UI/` – UI panels and views (`CurrencyPanel`, `OverflowPanel`, `MovieRecipePanel`, `TalentListPanel`, etc.)
- See `README.md` and `DESIGNLOG.md` for further details and history.
