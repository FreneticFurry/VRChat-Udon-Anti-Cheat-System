# -{FAC Rewrite}-

warning: this is subject to change, this is put here in preperation for FAC Rewrite to eventually replace the current ReadMe and documentation, just temporary storage and lets you view what is actively being added or changed!

# -{Features}-
anti flying (done)
anti speed manipulation (done)
anti seat cheating (done)
anti block invis (done)
anti out of bounds (done)
anti respawning (done)
anti out of bounds (done)

- planned features that will be worked on in order -

anti ovr advanced/ gogo loco (slightly done)
tool/ interaction verification (planned)
anti Mirror's & Camera cheating (planned)
anti avatar overlay esp cheating (planned)

# -{Setup/ Install}-
1. download FAC.UnityPackage from the latest downloads
2. import the unitypackage into your unity project
3. place the prefab from the FAC folder "FAC: AntiCheat" into your scene
4. configure settings for FAC on the prefab if needed
5. under prefab/resources "FAC: Synced Data" should be there containing the blocked player, configure sync settings for it there

# -{Known Issues}-
hackers might be able to manipulate the blocked player sync data, no preventions in place for that but there is plans to add preventions or at minimum reductions to that potential issue

# -{TOS}-

by using, editing or redistributing FAC you agree to these terms:

1. you may make edits, changes and so on to FAC.
2. in some way shape or form provide proper credit to me for FAC.
3. you may not sell FAC, FAC is a F2U asset it is not to be sold by anyone for any reason none of the code found within FAC is not to be sold.
4. you may redistrubute FAC in any F2U way. (preferrably via forking FAC on github)
5. you may make assets that directly use FAC or interact with FAC in any way shape or form.
6. if you're making your own similar system you can use FAC as a example, you may not copy-paste FAC code and call it your own.

# -{Support}-

if you'd like to support my random shananagains then feel free! https://www.patreon.com/freneticfurry
i intend to keep most if not all of my things free! supporting allows me to put more time and effort into F2U projects.

if you need help/ support with FAC please feel free to contact me on discord: freneticfurry
(also a good way to report bugs to me so they can be fixed making FAC overall better for everyone.)

# -{Credit}-

- Zerithax: alot of bug finding & some slight scripting help
- Vector Lotus: some bug finding/ QA help
- Trudolph: some bug finding/ QA help & feature suggestion



# -{Documentation}-

- Public exposed functions (designed for Development)

| Function | Description | Usage |
|:-------|:------------|:------------|
| FAC.SetPlayerVelocity(Velocity) | tells the anticheat that the localPlayer velocity is going to be changed by the world and allows that to happen | use to set the player velocity correctly |
| FAC.TeleportPlayer(Position, Rotation, SpawnOrientation, RemoteSmoothing) | exact same as TeleportTo just with extra steps to tell FAC to allow the teleport | use to teleport the player where you'd want to teleport them |
| FAC.Seat() | similar to TeleportPlayer() but more designed for VRCStations instead | use this to allow a world seat to be used correctly |
| FAC.IsGrounded | returns true if the player is on the floor returns false if the player is not on the floor eg. jumping | use this if you want to know if the LocalPlayer is on the ground or not |
| FAC.ViewAttempts | returns a Int value for the amount of times the LocalPlayer attempted to look in a collider | can be used for any purpose you'd want this int for |
| FAC.SpeedAttempts | returns a Int value for the amount of times the LocalPlayer attempted to gain more speed then what is allowed | can be used for any purpose you'd want this int for |
| FAC.FlyAttempts | returns a Int value for the amount of times the LocalPlayer attempted to fly/ stay in the air when they shouldn't be able to | can be used for any purpose you'd want this int for |
| FAC.ViewManipulationAttempts | returns a Int value for the amount of times the LocalPlayer attempted to use something like Space Drag to view outside a reasonable range | can be used for any purpose you'd want this int for |
| FAC.OutOfBoundsAttempts | returns a Int value for the amount of times the LocalPlayer attempted to get out of bounds | can be used for any purpose you'd want this int for |
| FAC.RespawnCount | returns a Int value for the amount of times the LocalPlayer has respawned | can be used for any purpose you'd want this int for |

- Internal functions (not designed for development)

| Function | Description | Usage |
|:---------|:------------|:------------|
| FAC.Detected(State, Reason, Counter) | formats output and teleports the player to the harsh location | Detected(0, "Example", 420) & Detected(1, "Example", 420) |
| FAC.onTeleportEnd() | used to define when a teleport has ended/ finished | this function is exposed but not meant for development is intended for internal usage |
