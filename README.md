# Aion

Aion is a turn-based action point battle framework built for Unity. It features a system where each combatant can change their abilities through "Aions" (alter-egos). The project includes basic scripts, enumerations, and example assets to help you get started.

## Features

- **Action Point Combat** – Units act in order based on Agility and spend action points to perform moves.
- **Aion System** – Characters can swap between Aions, each providing its own list of moves and elemental affinities.
- **Simple AI Examples** – Basic AI behaviours, including reckless and predator strategies, demonstrate how enemies can react to player actions.
- **Extensible Base Classes** – Easily create new units, moves, equipment, and effects by inheriting from provided base classes.

## Repository Layout

- `Base Classes/` – Core scriptable objects and MonoBehaviours for units, moves, equipment, and effects.
- `Enumerations/` – Enums used throughout the codebase (elements, affinities, equipment types).
- `UnitClasses/` – Player and enemy AI behaviours.
- `Example/` – Sample assets showing how Aions and moves can be configured.
- `Settings.cs` – Tunable gameplay constants for damage, evasion, critical hits, and other mechanics.

## Getting Started

1. Open or create a Unity project (Unity 2021.3 or newer is recommended).
2. Copy the contents of this repository into your project's `Assets` folder.
3. Add the `AionManager` prefab to your starting scene and assign spawn positions and unit prefabs in the inspector.
4. Create new Aion and Move assets using the provided ScriptableObject menus.

The example assets under `Example/` demonstrate basic configuration of a mythlet (Aion) and a simple fire attack.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
