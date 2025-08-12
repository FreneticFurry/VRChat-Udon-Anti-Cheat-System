# -{FAC Rewrite}-

i've been asked when rewrite will come out, rewrite will eventually come out as i dont have any deadlines or expected times for it to be out, i am just 1 person working on this and i only have so many people that're willing to help me test and learn... im working on my own game world(s) aswell so that also adds delay onto FAC aswell so with that being said please be patient.

warning: this is subject to change, this is put here in preperation for FAC Rewrite to eventually replace the current ReadMe and documentation, just temporary storage and lets you view what is actively being added or changed!

# -{Features}-
1. anti flying (done)
2. anti speed manipulation (done)
3. anti seat cheating (done)
4. anti block invis (done)
5. anti out of bounds (done)
6. anti respawning (done - trudolph's feature request)
7. anti Mirror's & Camera cheating (done)

- planned features -

8. mark client users as cheating remotely like in PD2 (in progess, will be a Experimental addon for FAC, use at your own risk and ensure that you set it up correctly as that is very important. - Vector Lotus's Feature request)
9. anti ovr advanced/ gogo loco (in progress - slightly done)
10. tool/ interaction verification (planned)
11. anti avatar overlay esp cheating (planned)
12. anti menu bugging (planned, some other detection will likely fix it but if no other one does then direct support will be made)

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
3. you may not sell FAC, FAC is a F2U asset it is not to be sold by anyone for any reason all of the code found within FAC should not be sold nor should snippets.
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
- Vector Lotus: good amount of bug finding/ QA help & feature suggestion
- Trudolph: some bug finding/ QA help & feature suggestion

# -{Documentation}-

- Settings

| Setting | Description | Default Value |
|:---------|:------------|:------------|
| Enable Anti Cheat | set to true to have the anti cheat work set to false to disable the entire anti cheat. | True
| Collider Viewing | set to true to have the anti cheat detect collider viewing set to false to have the anti cheat allow collider viewing. | True
| Collider Viewing Range | change the float to define the range before fac will try to prevent the player from viewing inside a collider. | 0.125
| Speed Manipulation | set to true to have the anti cheat detect speed manipulation set to false to have the anti cheat allow speed manipulation. | True
| Speed Leniency | set the float to define the allowed leniency before it starts thinking that the player is speed manipulating. | 3
| Flight | set to true to have the anti cheat detect flight set to false to have the anti cheat flight manipulation. | True
| Flight Leniency | set the float to define the allowed leniency before it starts thinking that the player is flying when they shouldn't be. | 0.1
| View Manipulation | set to true to have the anti cheat detect View Manipulation set to false to have the anti cheat allow View Manipulation. | True
| View Leniency | set the float to define the allowed leniency before it starts thinking that the player is attempting to go outside a reasonable range via something like space drag. | 0
| Anti Block Invis | set to true to have a visual blocked avatar when someone blocks another player set to false to let players just remain invisible to each other if they block each other. | True
| Anti Blocked Avatar | set this to a gameobject to define the blocked avatar used, look at the default blocked avatar as a reference to set this up with a custom one. | Anti Block Default
| Enable Bounds | set to true to have the anti cheat detect the player going out of bounds set to false to have the anti cheat allow the player to go out of bounds. | False
| In Bounds Colliders | this is a array of colliders used for Enable Bounds if there is no colliders here the Enable Bounds doesnt work. | None
| Protect Mirrors Cameras Layers | this determines if the anti cheat will or will not protect against layer viewing primarily in mirror's or camera's | True
| Use Shader Block | this will determine if the anti cheat will use "ShaderBlock" $${\color{#ff0000}Warning}$$: will disable Usernames and Chatboxs, anything transparent will not render, change the ShaderBlock render queue to fix (changing the queue will also allow avatars to get around the ShaderBlock, set it up how you want it to be.) | False
| Protected Layers | this is a LayerMask only supporting 31 inputs as unity only allows for 32 layers, change to protect layers within the mirrors and camera's (this cannot be changed in realtime it is a startup process.) | Mixed
| Punishment Location | set this to a location you want to use when a harsh punishment is used on a player. | (0, 0, 0)
| Punish Collider Viewing | set to true to send the player to the punishment location when they get detected for attempting to look inside a collider. | False
| Punish Speed Manipulation | set to true to send the player to the punishment location when they get detected for attempting to gain more speed then what is allowed. | False
| Punish Flight | set to true to send the player to the punishment location when they get detected for attempting to fly. | False
| Punish ViewManipulation | set to true to send the player to the punishment location when they get detected for attempting to space drag out of a reasonable range. | False
| Punish Out Of Bounds | set to true to send the player to the punishment location when they get detected for being out of bounds. | False
| Disallow Respawning | set to true to turn off respawning via the quick menu. | False
| Is Grounded Radius | set the float to change how large the check is for seeing if the player is or isnt grounded. | 0.2
| Ignored Objects | this is a array of objects to be ignored by the anti cheat, example: if a object here is set to Walkthrough and the player stands on top it will consider them as fly cheating or not grounded/ not on any floor. | None
| Collider Layers | used to define what layers the Is Grounded logic will use, should be set to be just on Default but can be used for more layers if needed. | Default
| Log Errors | set to true to output Error logs from FAC. | False
| Log Events | set to true to output Event/ Function logs from FAC. | False
| Log Detections | set to true to output each detection when they happen from FAC. | False

- Public exposed functions (designed for Development)

| Function | Description | Usage |
|:-------|:------------|:------------|
| FAC._SetPlayerVelocity(Velocity) | tells the anticheat that the localPlayer velocity is going to be changed by the world and allows that to happen | use to set the player velocity correctly |
| FAC._TeleportPlayer(Position, Rotation, SpawnOrientation, RemoteSmoothing) | exact same as TeleportTo just with extra steps to tell FAC to allow the teleport | use to teleport the player where you'd want to teleport them |
| FAC._Seat() | similar to TeleportPlayer() but more designed for VRCStations instead | use this to allow a world seat to be used correctly |
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
| FAC._Detected(State, Reason, Counter) | formats output and teleports the player to the harsh location | Detected(0, "Example", 420) = formatting & Detected(1, "Example", 420) = teleport player to harsh location |
| FAC._onTeleportEnd() | used to define when a teleport has ended/ finished | this function is exposed but not meant for development is intended for internal usage |
