# Continuous Map Navigation

This system lets the player move across a larger map without loading new scenes. The world contains several anchor points that act as focus targets. When a player selects a new area or drags the screen past a threshold, the camera pans smoothly from its current anchor to the next.

- Uses `Cinemachine` to interpolate camera position and zoom.
- Motion blur is enabled through Unity's Post‑Processing stack, giving a sense of speed during transitions.
- Anchors can be placed on any game object to define new points of interest.
- Ideal for large, scrolling environments such as the studio backlot or office floors.

See [README](../README.md) for how this integrates with the rest of the project.
