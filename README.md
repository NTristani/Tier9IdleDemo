# Idle Adventurer Demo

A small single-player Unity demo inspired by the early-game loop of **IdleOn**. The goal of this project is to show a compact idle RPG experience with town interaction, questing, auto-combat, item drops, upgrades, saving/loading, and offline progress.

Built in **Unity 6000.3.11f1**.

---

## Submission Links

**GitHub Repository:**
https://github.com/NTristani/Tier9IdleDemo

**Runnable Build:**
https://github.com/NTristani/Tier9IdleDemo/releases

**Short Demo Video:**


---

## Project Overview

This demo focuses on a small vertical slice of an early idle RPG experience. The player begins in a starter town, talks to a quest guide, travels to a monster field, defeats enemies, collects materials, completes a quest, claims rewards, buys upgrades, and can save/load progress.

The project is fully single-player and designed so its core features can be evaluated quickly.

---

## Controls

| Action                | Input                                                      |
| --------------------- | ---------------------------------------------------------- |
| Move left/right       | `A / D` or `Left Arrow / Right Arrow`                      |
| Travel through portal | Walk into a portal trigger                                 |
| Talk to guide         | Move near the NPC and click `Talk to Guide`                |
| Return to town        | Use the return portal or UI button                         |
| Save/load/clear save  | Use the Save panel                                         |
| Test offline progress | Use `Simulate 10 Min Combat AFK` while in the combat field |

---

## How to Play

1. Start in the town.
2. Move near the **Field Guide** NPC.
3. Click **Talk to Guide** to view the first quest.
4. Walk into the portal labeled **To Monster Field**.
5. Enemies will spawn and move toward the player.
6. The player auto-attacks nearby enemies.
7. Defeat enemies to gain XP, coins, and Green Essence.
8. Complete the quest objective.
9. Return to town and claim the reward from the Field Guide.
10. Spend coins and Green Essence on upgrades.
11. Save/load progress or test offline gains.

---

## Implemented Features

### Town and Combat Zone Flow

* Starter town area.
* Monster field combat area.
* Reusable portal trigger system.
* Zone-aware UI and enemy spawning.
* NPC interaction prompt that only appears when the player is near the guide.

### Player Movement

* Basic left/right movement.
* Player sprite flips based on direction.
* Movement bounds are adjusted based on the current zone.

### Auto-Combat

* Player automatically attacks nearby enemies.
* Enemies move toward the player.
* Enemies stop within attack range.
* Damage scales with player level and upgrades.

### Combat Feedback

* Floating damage numbers.
* Enemy health bars.
* Hit flash feedback.
* Enemy death pop/fade animation.

### Progression

* XP and level system.
* Coin rewards.
* Material drops.
* Green Essence inventory tracking.
* Combat upgrades:

  * Strength Training: increases damage.
  * Quick Hands: increases attack speed.

### Quest System

* NPC quest giver.
* Quest tracker UI.
* First quest: defeat enemies in the field.
* Quest reward claiming.
* Quest progress also updates from offline combat gains.

### Save and Load

* Saves player level, XP, coins, inventory, quest progress, upgrade levels, and current zone.
* Can load saved progress on startup.
* Can manually save, load, and clear save data.
* Clear save resets current runtime progress and prevents the cleared data from being immediately re-saved.

### Offline Progress

* Offline gains are calculated from the last saved timestamp.
* Offline progress only applies if the player saved/quit while in the combat field.
* Time is rounded up to the nearest second.
* Rewards scale based on elapsed time, player damage, attack speed, enemy health, and offline efficiency.
* Popup displays readable elapsed time and rewards earned.
* Includes a demo button to simulate 10 minutes of combat AFK progress.

### Evaluator Guidance

* Current objective hint.
* Tutorial prompt.
* Demo goals panel.
* World labels for NPCs, portals, and zones.

---

## Design Notes

The demo intentionally focuses on feature breadth and a clean early-game loop rather than attempting to recreate the full scope of IdleOn.

The core loop is:

```text
Talk to NPC → Enter combat field → Defeat enemies → Gain XP/coins/materials → Complete quest → Return to town → Claim reward → Buy upgrades → Save/load/offline progress
```

Progression is tuned to be fast so the main systems can be tested in under 30 minutes.

---

## Technical Notes

The project uses a lightweight event-driven architecture.

Key systems include:

* `GameEvents`
* `PlayerStats`
* `AutoCombatController`
* `Enemy`
* `EnemySpawner`
* `EnemyMovement`
* `QuestManager`
* `InventoryManager`
* `UpgradeManager`
* `SaveManager`
* `OfflineProgressManager`
* `WorldZoneManager`

Data-driven content is handled with ScriptableObjects, including:

* Enemy definitions
* Item definitions
* Quest definitions
* Upgrade definitions

UI panels are generally kept active in the hierarchy and hidden using `CanvasGroup` instead of disabling the entire GameObject. This helps event listeners remain active while panels are visually hidden.

---

## Build Instructions

1. Open the project in **Unity 6000.3.11f1**.
2. Open the main scene:

```text
Assets/_Project/Scenes/Main.unity
```

3. Go to:

```text
File > Build Profiles
```

4. Select or create a Windows, Mac, or Web build profile.
5. Make sure the main scene is included in the build.
6. Build the project.
7. For Windows builds, zip the full build folder, not only the `.exe`.

---

## Recommended Test Flow

For evaluating the demo, follow this sequence:

1. Start from a cleared save.
2. Move near the Field Guide.
3. Open the NPC quest dialogue.
4. Travel to the monster field through the portal.
5. Watch enemies move toward the player.
6. Observe auto-combat, damage numbers, health bars, and death animations.
7. Collect coins and Green Essence.
8. Complete the first quest.
9. Return to town.
10. Claim the quest reward.
11. Buy an upgrade.
12. Save the game.
13. Load the game.
14. Save while in the combat field, exit, then relaunch to test offline gains.
15. Use the simulated combat AFK button to quickly demonstrate offline progress.

---

## Asset Credits

Only free 2D assets from approved sources were used.

### Pixel Adventure 1

Used for player/enemy/environment sprites.

* Source: https://assetstore.unity.com/packages/2d/characters/pixel-adventure-1-155360
* Author: Pixel Frog
* License: Standard Unity Asset Store EULA

---

## Known Issues / Limitations

* This is a demo slice, not a full recreation of IdleOn.
* There is currently one main quest and one primary enemy type.
* Combat is simplified into automatic nearby attacks.
* Offline progress is calculated from expected combat performance rather than simulating every individual enemy encounter.
* The game is balanced for quick evaluation rather than long-term progression.

---

## Development Practices

The project was developed with Git and organized around small feature milestones, including:

* Basic Unity setup
* Auto-combat loop
* Combat feedback
* Enemy movement
* Quest tracking
* Inventory and drops
* Upgrade panel
* Save/load system
* Offline progress
* Town/combat zone flow
* UI polish and evaluator guidance

---
