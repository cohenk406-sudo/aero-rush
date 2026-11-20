# Aero Rush — Unity 6 prototype

An original 3D car-soccer prototype. This repository now includes gameplay reward and lobby foundations.

## Included features

- **Earnable rare car skins:** Copper Comet (5 matches), Frostline (15 matches and 3 wins), and Solar Crown (30 matches and 12 wins). Progress is saved locally with `PlayerPrefs`; there are no paid unlocks.
- **Modes:** Normal Match and Private Free-for-All are represented by `MatchMode` and lobby creation methods.
- **Friends lobby flow:** create a lobby, show a six-character invite code, and join by code.
- **Unity 6 scripts:** `Assets/AeroRush/Scripts/Online/` and `Assets/AeroRush/Scripts/Progression/`.

## Unity setup

1. Open a Unity 6 project and copy the `Assets/AeroRush` folder into it.
2. Add `LobbyManager` and `SkinProgression` to scene objects.
3. Connect menu buttons to `CreateNormalMatch`, `CreatePrivateFreeForAll`, and `JoinByCode`.
4. Call `RecordMatchResult(won)` when a match finishes. Use `SelectSkin(skinId)` to equip an unlocked skin and `ApplySelectedSkin(renderers)` to tint the car renderers.

## Online networking note

The lobby menu flow exposes `ILobbyTransport`, but this repository does **not** yet include a networking backend. To let friends join across the internet, implement that interface using Unity Gaming Services Lobby/Relay (or another provider), configure the service project, and add the required packages. Until then, the scripts report that a transport is needed; they do not pretend to create a real online room. Platform friends-list invites also require platform-specific services.
