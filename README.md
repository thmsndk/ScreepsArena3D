ScreepsArena3D

Unity 2021.1.2f1 is used because gzip handling is broken in the newer unity versions.

Extract `TestReplay\ScreepsArena3D.zip` into `%AppData%\..\LocalLow\ScreepsCommunity`

example:
`C:\Users\thmsn\AppData\LocalLow\ScreepsCommunity\ScreepsArena3D\Replays\606873c364da921cb49855f7\609989f6891dffcde3f09554`

Replays are currently cached in `%AppData%\..\LocalLow\ScreepsCommunity`, you can make a copy of theese if you wish to be able to load a replay again at a later time.

TODO:
- [ ] RTS Cinemachine Camera
- [ ] Rooms should be able to instantly swap to another data state (tick)
- [ ] A component that can fetch data
  - [ ] This data should be fetched from a cache
  - [ ] If there is no data, it should fetch it from arena servers.
  - [ ] data should be cached when fetched.
- Use models from Screeps World 3D client
  - [ ] Creep
  - [ ] Tower
  - [ ] Spawn
  - [ ] Wall
  - [ ] Rampart
  - [ ] Source
  - [ ] Resource
  - [ ] Flag
- Animate actions
  - [ ] creep
  - [ ] tower
  - [ ] spawn
- [ ] Render storage
- [ ] Show a list of arenas
  - [ ] Show a list of your replays for selected arena.
- [ ] Render a selected replay
- [ ] use gameId.GetHashCode() for room terrain seed - so a unique terrain is generated the same way each time.


Later / Ideas
- Save replays as zip files once all data is acquired
- Ability to extract zip files in memory 
- Show multiple arenas at the same time?
