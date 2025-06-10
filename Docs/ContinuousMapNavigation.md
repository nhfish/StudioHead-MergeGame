# Continuous Map Navigation


This system lets the player move across a larger map without loading new scenes. The world contains several anchor points that act as focus targets. When a player selects a new area or drags the screen past a threshold, the camera pans smoothly from its current anchor to the next.

- Uses `Cinemachine` to interpolate camera position and zoom.
- Motion blur is enabled through Unity's Post‑Processing stack, giving a sense of speed during transitions.
- Anchors can be placed on any game object to define new points of interest.
- Ideal for large, scrolling environments such as the studio backlot or office floors.

See [README](../README.md) for how this integrates with the rest of the project.

This document describes the planned design for navigating the continuous map used in **Studio Head**. It focuses on how camera movement, map anchors, and scene transitions are organized.

## Map Structure

- The entire world map lives under a root GameObject named `WorldMapRoot` in the scene hierarchy.
- Key locations on the map each have an empty child object following the `Anchor_[Location]` naming convention (e.g. `Anchor_Town`, `Anchor_Studio`).
- Anchors serve as targets for the camera and are used when jumping between regions of the map.

## Camera Panning

- A `WorldMapCameraController` component handles smooth panning.
- Dragging or swiping the screen scrolls the camera across the map while clamping its position to the bounds of `WorldMapRoot`.
- The controller optionally snaps the camera to a specific `Anchor_[Location]` when triggered by UI buttons or scripted events.

## Parallax Layers

- Background and mid‑ground elements are organized in multiple parallax layers.
- Each layer moves at a slightly different speed based on its parallax multiplier, creating depth as the camera pans.
- Layers are children of `WorldMapRoot` so they stay aligned with the anchor positions.

## Scene Transitions and Manual Nudge

- A scene transition manager controls loading and unloading scenes such as `MainMenu` and `GameMain`.
- When the player selects a destination on the world map, the manager nudges the camera toward the target anchor before fading into the new scene.
- Manual nudge controls are available for debugging: pressing arrow keys or on‑screen buttons will slightly shift the camera without fully panning.

---

**Integration Points**

- The map is displayed in the `GameMain` scene. The `MainMenu` scene transitions here when the player presses the Play button.
- Anchors and camera controller scripts are placed under `WorldMapRoot` in `GameMain` so both gameplay scenes share the same navigation system.

