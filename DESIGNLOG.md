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

## Next Steps
- Implement timed tile spawning to introduce new items over time.
- Prototype the movie recipe system to combine merged items into short film projects.

