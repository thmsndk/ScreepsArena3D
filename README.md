ScreepsArena3D

Unity 2021.1.2f1 is used because gzip handling is broken in the newer unity versions.

Extract `TestReplay\ScreepsArena3D.zip` into `%AppData%\..\LocalLow\ScreepsCommunity`

example:
`C:\Users\thmsn\AppData\LocalLow\ScreepsCommunity\ScreepsArena3D\Replays\606873c364da921cb49855f7\609989f6891dffcde3f09554`

TODO:
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
- Animate actions
  - [ ] creep
  - [ ] tower
  - [ ] spawn
- [ ] Render storage
- [ ] Show a list of arenas
  - [ ] Show a list of your replays for selected arena.
- [ ] Render a selected replay


Later / Ideas
- Save replays as zip files once all data is acquired
- Ability to extract zip files in memory 
- Show multiple arenas at the same time?
- Ability to press "Play" ?
