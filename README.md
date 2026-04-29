# KerbalAdminKit (KAK)

A toolkit mod that customizes the KSP Administration Building and surfaces character-authored program information across KSC. Renders UI for data owned by [KerbalCampaignKit](https://github.com/badgkat/KerbalCampaignKit) and [KerbalDialogueKit](https://github.com/badgkat/KerbalDialogueKit); does not own game state itself.

## Status

v0.1.0 — under development.

## What KAK Does

- Optional admin-building replacement with three-panel IMGUI UI (Characters / Dashboard / Desk).
- Per-building character overlays in Mission Control and Tracking Station.
- KSC notification markers pulled from `KerbalCampaignKit.Notifications`.
- Condition-driven memo pipeline with optional mirroring to the stock mail system.
- `DISPOSITION_DECAY` cfg drives in-process flag transitions on KDK's flag store.
- PR Campaign gameplay action.

## Activation

Install KAK with no content cfg and nothing changes. Stock admin, stock mail, stock KSC view all work normally.

Set `KERBAL_ADMIN_KIT { replaceAdminBuilding = true }` in a content cfg to take over the admin building.

## Requirements

- KSP 1.12.x
- [ModuleManager](https://forum.kerbalspaceprogram.com/index.php?/topic/50533-*)
- [HarmonyKSP](https://github.com/KSPModdingLibs/HarmonyKSP)
- [ClickThroughBlocker](https://forum.kerbalspaceprogram.com/index.php?/topic/170747-*)
- [KerbalDialogueKit](https://github.com/badgkat/KerbalDialogueKit) >= 0.1.0
- [KerbalCampaignKit](https://github.com/badgkat/KerbalCampaignKit) >= 0.1.0

## License

MIT — see `LICENSE`.
