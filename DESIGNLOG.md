# Design Log

This file tracks high level design choices and rationale for **Studio Head**, a top-down merge and idle prototype. Use this document to record major decisions so the team can reference past discussions and keep future work aligned.

## Core Goals

- **Merge Mechanics** – Allow players to drag items onto one another to create upgraded versions. The grid-based board manages space and prevents invalid placements.
- **Modular Data** – Items are defined via `ScriptableObject` assets so designers can quickly add new props, costumes, or talent without touching code.
- **Mobile Friendly** – Input handling supports both touch and mouse to keep the project cross-platform. The UI scales using Unity's Canvas Scaler.

## Recent Decisions

- **Economy System** – Added an `EconomyManager` that tracks multiple currencies (`Money`, `Gems`, `Tickets`). The `CurrencyPanel` listens for changes and updates the UI automatically.
- **Color-Coded Tiers** – Merge items use background colors to represent tier level for quick readability. Tier 1 is gray and scales up to gold for tier 10.

- **DepartmentCrateSpawner Fix** – Spawner now selects a free grid cell before spawning crates.
- **Production Flow Managers** – Integrated `ProductionManager`, `DistributionQueueManager`, and `DistributionPanel` to handle movie production.
- **Shop Manager** – Added a `ShopManager` and expanded the multi-currency economy for in-game purchases.
- **Talent Inventory** – Created `TalentInventory` and collectible talent cards.
- **Crate Shop Panel** – Added buttons to buy a department crate that spawns on the grid and deducts currency.
- **Distribution Options** – Added placeholder buttons for Streaming, Festival, and Film Market distribution types (currently disabled).

- **Inventory Overflow** – Items that exceed board capacity move to an overflow list. Players can purchase additional slots or clear space to reclaim them.
- **Universal Crate Timer** – A single timer controls automatic crate spawns. Department type is chosen using weighted chances for varied drops.
- **Department Crate Purchase & Upgrade** – `DepartmentCrateManager` spawns crates using weighted item tiers and scales costs as departments advance.
- **OverflowPanel UI** – Expandable panel shows overflow items and warns the player when space runs low.
- **Genre Synergy Bonus** – Matching items grant a synergy percentage that boosts Money and Ticket rewards during distribution.
- **CurrencyPanel integrated into HUD across top of screen.**
- **Planned Main Menu scene with Settings overlay for initial navigation.**
- **Continuous Map Navigation** – Implement anchors, parallax layers, and camera controls for smooth scrolling. Project currently mixes old and new input APIs.

## Next Steps
- Finalize synergy bonus calculation and integrate with movie rewards.
- Implement timed tile spawning to introduce new items over time.
- Prototype the movie recipe system to combine merged items into short film projects.
- Explore social leaderboards and a player marketplace for trading high-tier items.

