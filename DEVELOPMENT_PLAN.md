# Save the Village — Development Plan

## Product goal

Ship a landscape Android 3D survival clicker that looks and feels deliberate on a phone: a rounded, toy-like dam on the left and village across the centre and right of a living landscape; clear weather escalation; gratifying taps; and rare, frightening flood events. A good single run should support roughly 50–100 hours of play, without a fixed final level.

The first public release is Android-only. It uses rewarded ads, is playable without an ad, and does not rely on purchased game assets.

## Product rules already decided

| Area | Rule |
| --- | --- |
| Currency | Tapping is the only normal source of coins. |
| Pumps | Pumps remove water only. Their upgrade raises draining power. |
| Progression | Pump and tap output scale so later purchases need more taps but remain achievable. |
| Threat | Procedural rain has calm periods and escalating severe weather. Flooding reduces village health and changes the scene. |
| Run length | A capable player can hold a run for about 50–100 hours. Loss ends that run. |
| Presentation | Smooth, rounded 3D forms; plain materials; natural-feeling water, weather, visual, and sound feedback. |
| Initial ads | An optional rewarded-ad offer appears on a 2–3 minute cooldown and awards extra coins after completion. |

## Technical direction

- **Engine:** Unity 6 LTS with the Universal Render Pipeline (URP).
- **Target:** Android first, landscape orientation, 60 fps target on mid-range phones.
- **Code:** C# with clear, small systems and data-driven balance values.
- **Art:** Custom primitive-based blockout first, then Blender models for the dam, pumps, village buildings, trees, rocks, and bridges. Materials remain simple colored surfaces with baked ambient shading and soft lighting.
- **Simulation:** Lightweight numerical water model drives the visible water, spillway, flood level, village damage, and effects. We will not use real-time fluid simulation on phones.
- **Ads:** Unity LevelPlay rewarded ads, introduced only after the gameplay loop works. Each placement must be initiated by the player.
- **Release:** Google Play Android App Bundle, target API level required by Google Play at submission time.

URP supports Android, while Unity's legacy Built-In Render Pipeline is being deprecated. Unity also recommends its LTS line for live-service games. Google Play requires new submissions and updates to target Android 16 / API 36 from August 31, 2026. Sources: [Unity release support](https://unity.com/releases/unity-6/support), [Unity Android render-pipeline support](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/introducing/requirements-and-compatibility), [Google Play target API requirement](https://developer.android.com/google/play/requirements/target-sdk).

## Scope for version 1.0

### Player experience

1. Tap anywhere on valid game space to earn coins with a visible coin burst, sound, and haptic pulse.
2. Buy a finite set of distinct pumps. Each has a clear position by the dam and a unique visual form.
3. Upgrade installed pumps forever to improve water-draining power.
4. Buy tap-strength upgrades from a visible village worksite to increase coins per tap.
5. Experience dynamic weather: clear skies, drizzle, rain, heavy rain, thunderstorm, and rare extreme storms.
6. Read water danger at a glance through the reservoir level, weather, warning effects, spillway, floodwater, and village health bar.
7. Continue a run until village health is depleted, then receive a run summary and restart.
8. Optionally claim extra coins through one clearly labelled rewarded-ad offer, capped by cooldown.
9. Save and restore the active run locally.

### Explicitly out of scope for 1.0

- Real fluid physics, online accounts, multiplayer, live events, battle pass, crafting, user-created content, paid assets, and a second biome.
- Extra ad rewards such as village healing and free pump levels. We can add them after balancing proves they improve the game.

## Core economy model

Balance is based on **number of taps**, not raw coin amounts. This keeps upgrades understandable as coin values become large.

| System | Starting behavior | Growth rule |
| --- | --- | --- |
| Tap | 1 coin per tap | Worksite upgrades multiply tap output. |
| Pump purchase | First pump is cheap enough to reach quickly | Each new pump requires more target taps than the previous pump. |
| Pump upgrade | Increases drainage rate | Cost grows faster than its drainage gain, but tap-strength upgrades keep it affordable. |
| Rain | Calm flow between events | Event strength and duration rise with run progress, with bounded randomness. |
| Damage | No damage below safe water level | Damage grows rapidly after sustained overflow, with clear warning time. |
| Rewarded ad | Extra coin grant | Sized as useful progress, never as the only way to survive. |

The actual curves will remain editable in one balance asset. We will make a debug panel that fast-forwards time and grants test coins so we can tune hour 1, hour 10, and hour 50 without playing for fifty hours.

## Visual quality bar

Professional quality here means a coherent art direction, consistently strong feedback, readable UI, solid performance, and no unfinished-looking placeholders. It does not mean expensive photorealism.

- Rounded silhouettes and restrained proportions across every prop.
- One warm daylight palette with weather-driven changes in sky, fog, lighting, water, and sound.
- Soft shadows, ambient occlusion, baked lighting where useful, and deliberate camera framing.
- Water uses animated shader motion, foam at the dam, simple waterfall meshes, rain particles, and flood-height visuals.
- Every purchase, tap, upgrade, warning, flood, and recovery has paired animation, sound, and screen feedback.
- UI uses a small visual language: large readable type, low text density, coin and water icons, and no tiny controls.

## Production milestones

### 0. Foundation — 1–2 days

- Install toolchain, create Unity URP project, configure Android, Git, build settings, landscape safe-area UI, and local save shell.
- Create the design and balance documents, scene folders, naming rules, and developer debug menu.
- Deliverable: an Android build launches on a phone.

### 1. Proof of fun — 1 week

- One camera view, blockout dam, water-level model, tap-to-earn, one pump, coin and water UI, simple rain states, and village health.
- Deliverable: 10-minute playable loop with a win-less survival goal, a flood, and a restart.
- Decision gate: confirm that tapping, buying the pump, and surviving a storm feel good before broad content work.

### 2. Complete core loop — 1–2 weeks

- Full pump roster, infinite pump upgrades, worksite tap upgrades, procedural weather director, escalation rules, save/load, run summary, and debug balance tools.
- Deliverable: a stable 1–2 hour prototype that can fast-forward to late-run balance targets.

### 3. Art and feel pass — 2 weeks

- Replace blockout with custom rounded assets, lighting, animations, rain and water effects, weather transitions, camera movement, haptics, and sound hooks.
- Deliverable: a visually coherent vertical slice that feels complete for its first 15 minutes.

### 4. Balance and mobile hardening — 1–2 weeks

- Tune early, middle, and late-run curves; profile on low and mid-range Android devices; reduce heat, battery use, load time, and memory pressure; fix input and save edge cases.
- Deliverable: release candidate that runs smoothly through the core loop.

### 5. Monetization, store, and launch — 1 week

- Add the optional rewarded ad, test callbacks and failed-ad behavior, configure consent and privacy disclosures, capture store media, complete store listing, internal testing, and publish.
- Deliverable: Google Play production submission.

## Responsibility split

I will drive the game design documentation, Unity project structure, C# systems, generated or procedural models, Blender asset specifications and creation where connected, art integration, effects, balancing tools, Android build configuration, testing steps, and release checklist.

You will provide the local access that cannot be automated from this chat: install required software, connect any Blender or Unity integration if we choose it, run builds or grant device access when requested, create developer and ad-network accounts, supply or approve music/SFX, and make final publishing/account decisions.

## First setup request

Install Unity Hub and the latest Unity 6 LTS editor, including **Android Build Support**, **Android SDK & NDK Tools**, and **OpenJDK**. Install Blender as well, but do not spend time creating assets. Once that is ready, tell me the Unity version shown by Unity Hub and whether you have an Android phone available for test builds.

## Decisions deferred until the prototype

- Exact number and identity of the first pumps.
- Exact prices, upgrade curves, and storm duration formulas.
- The game's name, icon, age rating, store copy, and music direction.
- Further rewarded-ad choices, including village healing and free pump levels.
- Whether to add meta-progression after run loss.
