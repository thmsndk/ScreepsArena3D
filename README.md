ScreepsArena3D

Unity 2021.1.2f1 is used because gzip handling is broken in the newer unity versions.

Extract `TestReplay\ScreepsArena3D.zip` into `%AppData%\..\LocalLow\ScreepsCommunity`

example:
`C:\Users\thmsn\AppData\LocalLow\ScreepsCommunity\ScreepsArena3D\Replays\606873c364da921cb49855f7\609989f6891dffcde3f09554`

Replays are currently cached in `%AppData%\..\LocalLow\ScreepsCommunity`, you can make a copy of theese if you wish to be able to load a replay again at a later time.

TODO:
- [ ] Camera
  - [x] RTS Cinemachine Camera  
        A TOP-DOWN camera view, can tweak later.
    - [x] WASD for moving up,left, down, right in a top
    - [x] Pan camera whne mouse touches edges
    - [x] Zoom in and out with scroll wheel
    - [ ] Rotate with Q + E ?
        
  - [ ] Freelook Cinemachine Camera  
        Kinda like the camera in Screeps3D, the idea is to have a toggle key to swap between the two cameras
    - [ ] Right click to change camera angle
    - [ ] WASD movement, W should move camera towards looking point, S should move away, A+D should strafe
    - [ ] Q+E to rotate camera
- [ ] Rooms should be able to instantly swap to another data state (tick)
- [ ] A component that can fetch data
  - [ ] This data should be fetched from a cache
  - [ ] If there is no data, it should fetch it from arena servers.
  - [ ] data should be cached when fetched.
- Use models from Screeps World 3D client
  - [ ] Creep
    - [x] Render body parts
    - [x] Render badge / color
      - [ ] Adjust colors to be "true" colors. the blue looks purple as an example.
    - [x] Scale size depending on body parts
      - [ ] Update hits of body parts each tick  
        We might need to persist data of objects from tick 0, their initial data. else creeps would grow in sizes when moving backwards in a replay where a creep died in the current tick. but if a creep is spawned mid round, their initial tick is somewhere else.  
        In CTF you can pick up body parts making your body larger.
    - [ ] Render actions
      - [ ] attack
      - [ ] rangedAttack
      - [ ] rangedMassAttack
      - [ ] heal
      - [ ] rangedHeal
      - [ ] repair
      - [ ] build
      - [x] movement
    - [ ] Render storage
  - [x] Tower
    - [ ] Render Owner color
    - [ ] Render Actions
      - [ ] rangedAttack
      - [ ] rangedHeal
      - [ ] repair
    - [ ] Render storage
  - [ ] Spawn
    - [ ] Render storage
  - [ ] Wall
    - [ ] wall height
  - [ ] Rampart
  - [ ] Source
  - [ ] Resource
  - [ ] Container
    - [ ] Render storage
  - [x] Flag
    - [ ] Render Owner color
    - [ ] Change direction of flag, or perhaps the model of flag to not require a direction.
- UI
  - [ ] Selected RoomObject
  - [ ] Render BodyParts on selected creep
- [ ] Show a list of arenas
  - [ ] Show a list of your replays for selected arena.
- [ ] Render a selected replay
- [ ] use gameId.GetHashCode() for room terrain seed - so a unique terrain is generated the same way each time.


Later / Ideas
- Save replays as zip files once all data is acquired
- Ability to extract zip files in memory 
- Show multiple arenas at the same time?
- CI https://isaacbroyles.com/gamedev/2020/07/04/unity-github-actions.html