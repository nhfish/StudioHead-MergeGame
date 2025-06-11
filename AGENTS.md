Studio Merge — Game Design Agent Notes (June 2025)
===================================================

Purpose
-------
This document describes the architecture, core systems, and design rules for **Studio Merge** (sometimes referred to as "Studio Head").
It helps agents such as Codex understand project conventions and current gameplay logic so contributions stay consistent.

Game Concept
------------
Mobile portrait (9×16) incremental merge game set inside a bustling film studio. Players merge **Department Items** to craft higher tier equipment, contract **Writer**, **Director**, and **Actor** cards, and assign everything to soundstages to produce movies. Completed films generate Money, Fans, and Fame. The aspirational goal is to earn a coveted 5‑star rating across all genre combinations.

Core Gameplay Loops
-------------------
- Drag-and-merge department items on the 6×9 grid (10-tier color palette).
- Tap department crates to spawn new items in free cells.
- Fill required and optional slots on soundstages with merged items.
- Select up to three talent cards (Writer, Director, Actor) from the Headshot Binder UI.
- Manage inventory space across the Merge Grid and Warehouse.
- Start a timed production. Optionally play the Dailies puzzle to boost rewards.
- Choose a distribution method: Flat payout or Revenue Over Time (RCUs).
- Earn Money, Fans, and Fame.
- Spend earnings on crates, soundstage upgrades, talent contracts, and overflow slots.
- Progress Studio Fame Rank to unlock more Talent, Soundstages, Marketing, and RCU capacity.
- Collect unique genre combinations and build Franchises via sequels.
- Long‑term goal: achieve 5‑star movies across all 14 genres.

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
- Merge grid size is currently 6×9 and locked to portrait orientation.
- One tile or crate occupies a cell at a time (no stacking).
- All drags are clamped to the screen and snap to the nearest cell on release.
- Merge actions may consume soft currency in the future ("Crew Energy").
- `MergeTile` uses a 10‑tier color palette: Gray → White → Yellow → Green → Dark Blue → Purple → Orange → Red → Light Blue → Gold.

Talent & Production
-------------------
- `TalentInventory` holds `TalentCard` instances created from `TalentBaseData` assets. Cards track remaining uses and locking.
- Talent has four rarities with limited uses:
  - **A-List** – 2 uses
  - **B-List** – 3 uses
  - **C-List** – 4 uses
  - **D-List** – 5 uses
- Up to three talents (Writer, Director, Actor) can be assigned per movie. Matching genres grant a synergy bonus. Reusing the exact combo diminishes that bonus.
- There are 14 genres: Action, Drama, Comedy, Fantasy, Horror, Mystery, Romance, Thriller, Sci-Fi, Western, Documentary, Animation, Musical, Biography.
- Available talent refreshes every three hours and higher rarities unlock with Studio Fame Rank.
- `MovieRecipePanel` builds a `MovieRecipe` using submitted items and selected talent.
- `ProductionManager` processes a recipe over time and notifies `DailiesManager` when milestones allow a Dailies attempt.
- The optional **Dailies Puzzle** (`DailiesBoardManager`) is a 4×4 merge mini-game that awards bonus score and multiplier.
- When production completes, `DistributionQueueManager` presents the `DistributionPanel` to choose payout type. Rewards funnel through `RewardManager`, archived via `FilmArchiveManager`, and tracked by `FranchiseManager` for sequels. Choosing a flat payout breaks Franchise Momentum.
Soundstage & Distribution
-------------------------
- Soundstage upgrades add optional slots rather than new required ones.
- Required departments: Camera, Sound, Production.
- Leaving optional slots empty applies a penalty to rewards and stars.
- Production times mirror real film lengths: 8, 22, 44, 70, and 180 minutes.
- Multiple soundstages exist: one free at start, three unlockable via soft currency, and one milestone unlock.
- Upgrading soundstages becomes a key end-game driver.
- After production players choose a distribution method:
  - **Flat Payout** – one-time reward.
  - **Revenue Over Time (RCUs)** – accrues over 24 real hours and is required for Awards and Franchise Momentum.
- RCU capacity scales with Marketing and Soundstage upgrades.
- Selecting Flat Payout cancels Franchise Momentum.

Movie Rating System
-------------------
- Five-star movies require three max-tier talents, all soundstage slots filled with the highest tier items, and a theatrical release via RCUs.
- A small RNG factor means five stars are never guaranteed.
- Lower star ratings scale rewards down proportionally.
- Awards from high ratings grant temporary passive boosts.

Studio Progression
------------------
- Studio Fame Rank acts as the primary meta progression.
- Fame gates talent rarity, soundstage unlocks, marketing upgrades, and RCU capacity.
- Higher ranks generate a small passive money drip.
- The studio map becomes busier as Fame Rank and active productions increase.
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
- Expand `EconomyManager` with passive income, full IAP support, and ad rewards via the **Screening Room**.
- Implement the **Headshot Binder** for talent selection.
- Add a Trend system to influence genre rewards.
- Develop a Film Festival mode for specialized competitions.
- Surface Franchise Momentum tracking in the UI.
- Finish leaderboards and studio fame ranking systems.
- Expand talent acquisition via a Contract Office screen and implement rarity progression.
- Add dynamic events and holiday themed boosts.
Notes for Codex / Contributors
-----------------------------
- All `MergeTile`, `DepartmentCrate`, and grid logic must enforce **one tile per grid cell**.
- Talent cards live in UI screens and never occupy grid cells.
- The merge grid is portrait locked and cannot scroll off screen.
- Soundstage optional slots incur a penalty when empty but do not block production.
- Franchise Momentum is sensitive; avoid unintended resets.
- Studio Fame Rank is a global gate for most systems and unlocks new content.
- When adding features, keep them within these core loops.
Repository Structure (partial)
------------------------------
- `Assets/_Game/Scripts/Grid/` – grid logic (`GridManager`, `DepartmentCrateSpawner`, `MergeTile`, etc.)
- `Assets/_Game/Scripts/Data/` – ScriptableObject definitions (`DepartmentItemData`, `MovieRecipeData`, etc.)
- `Assets/_Game/Scripts/Managers/` – gameplay managers (`EconomyManager`, `ProductionManager`, `DistributionQueueManager`, `InventoryOverflowManager`, `FranchiseManager`, `FilmArchiveManager`, `UniversalCrateSystem`)
- `Assets/_Game/Scripts/UI/` – UI panels and views (`CurrencyPanel`, `OverflowPanel`, `MovieRecipePanel`, `TalentListPanel`, etc.)
- See `README.md` and `DESIGNLOG.md` for further details and history.
