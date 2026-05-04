# MyShooterGame

A robust First-Person/Third-Person Shooter project built in Unity. This game takes place in a stylized environment utilizing modern Unity standards, a robust component-based architecture, and heavily optimized graphics packages.

## Core Gameplay Features
- **Player Mechanics**: Features smooth FPS/TPS movement with PlayerMovement.cs, FPSMouseLook, comprehensive PlayerHealth, camera zoom controls, and a custom GunHolder script for weapon management.
- **Combat System**: Includes a functional Crossbow and integration for multiple weapon types (managed via PaidGuns.asset Scriptable Objects). Features full damage vignettes on being hit.
- **Enemy AI**: Features standard EnemyController alongside specialized units like the MutantBossController. Enemies navigate using EnemyMovement.cs, deal damage with EnemyBullet and EnemyShooting, and spawn dynamically via EnemySpawner.cs.
- **Environment**: Primarily utilizes standard "Viking Village" assets along with interactive elements like DoorController.cs. 
- **Game Management**: Handles different game states effectively with SceneBootstrapper, VillageManager, and GameOverManager.
- **UI & HUD**: Features a rich user interface driven by DOTween (UIScaleAnimation, UISlideAnimation, UIIntroAnimation) along with in-game overlays like a KillCounterHUD. Uses HeavyMetalTextMaker for stylized feedback.

## Project Structure
- **Assets/Scripts/**: Core gameplay logic, broken into specialized modules:
  - Player/: Component-based player control (GunHolder.cs).
  - EnemyActions/: Scripts driving the enemy AI (EnemyMovement, EnemyShooting, etc.).
  - Interactions/: Level, world and object interaction handling.
  - GameAssetsControl/: Managers and configuration helpers.
  - Viking/: Scripts specifically tied to the Viking Village asset integration.
- **Assets/Scenes/**: Contains the main game scenes (MainMenu.unity, PlayScene.unity, WorldTest.unity, TestScene.unity).
- **Assets/Scriptable Objects/**: Data-driven architectures (e.g., PaidGuns.asset for weapon stores/management).

## Asset Integration & Third-Party Tools
- **Unity Input System**: Utilizes the modern InputSystem_Actions.inputactions for handling rebindable player inputs.
- **Demigiant (DOTween)**: Employed for fast, code-driven tweening (especially seen in UI scripts).
- **Cartoon FX Remaster (CFXR)**: Provides performant and stylized visual effects/particles.
- **Toony Colors Pro**: Delivers cartoon/cel shading for stylized rendering techniques.
- **KinoBloom**: Adds bloom post-processing effects.
- **TextMesh Pro**: Used for high-quality, crisp UI text rendering.
- **Viking Village**: Environment elements providing a robust level foundation.

## Development Setup
1. **Unity Version**: Ensure you have installed the LTS version required by the dependencies.
2. **Open Project**: From Unity Hub, choose **Add** (or **Open**), browse to c:\Games\Unity Projects\MyShooterGame, and open the folder.
3. **First Steps**: Open the MainMenu or PlayScene from the Assets/Scenes folder.
4. If there are missing references, verify that the required packages are downloaded inside the Package Manager.
