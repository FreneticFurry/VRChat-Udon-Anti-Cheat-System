# -{FAC Rewrite}-

development on FAC might be entirely halted and stopped, in short: im probably switching to a vastly more desirable platform then vrchat

i've been asked when rewrite will come out, rewrite will eventually come out as i dont have any deadlines or expected times for it to be out, i am just 1 person working on this and i only have so many people that're willing to help me test and learn... im working on my own game world(s) aswell so that also adds delay onto FAC aswell so with that being said please be patient.

warning: this is subject to change, this is put here in preparation for FAC Rewrite to eventually replace the current ReadMe and documentation, just temporary storage and lets you view what is actively being added or changed!

# -{Features}-
1. anti flying (done)
2. anti speed manipulation (done)
3. anti seat cheating (done)
4. anti block invis (done)
5. anti out of bounds (done)
6. anti respawning (done - Trudolph's feature request)
7. anti Mirror's & Camera cheating (done)
8. anti Shader cheating (done)
9. anti collider viewing/ no clipping (done)
10. anti view manipulation & menu bugging (done)
11. tool/ interaction verification (done)

- planned features roadmap - subject to change: things can be added or removed at any time, its just current thoughts and idea's on what to prevent or at least attempt to prevent

1. anti avatar overlay esp cheating (planned)
2. player collision/ pushing (planned, thinking about it being more like how it is within TF2 where it isnt 100% collisions but rather just a push force to keep players out of each other instead)
3. extra verification for VRCPickup's to prevent VRCPickup position/ hold bugging (planned)

for the people with the FAC rewrite pre alpha build - to use anti mesh viewing you MUST turn on read/write on your meshes or use a explicit collider eg. box collider, if you as a tester know a better way please let me know.

# -{Setup/ Install}-
1. download FAC.UnityPackage from the latest downloads
2. import the UnityPackage into your unity project
3. place the prefab from the FAC folder "FAC: AntiCheat" into your scene
4. configure settings for FAC on the prefab if needed
5. under prefab/resources "FAC: Synced Data" should be there containing the blocked player, configure sync settings for it there

# -{Known Issues}-

will be fixed before any alpha, beta or release - current phase: closed testing.

1. it is possible to bypass speed manipulation by brute force because it sometimes doesnt remember the correct location
2. it is possible to get stuck into the floor in such a way that it prevents menus from opening by brute forcing constantly
3. it is possible to use TeleportTo multiple times causing speed manipulation to allow a teleport when it shouldn't
4. the blocked avatar client user abuse prevention in some cases cause most data to stop being used (intended but should be more lenient for avatars since avatar creators seemingly never have the same arm span because they do not have a unified workflow at all eg. random/ very inconsistent proportions.)
5. it is possible to trigger the anti collider detection and reset the ovr position to teleport onto or past things you shouldn't be able to when brute forcing
6. auto tool verification sometimes will improperly think the tool belongs to someone else and not trigger when it should be triggering due to an oversight with the peer watching other players (unnamed planned feature to watch remote players to see if they're cheating, i just dont know what to name that.)
7. various shader visual problems regarding depth
8. it is possible to get recorrected in such a way from the anti flying + anti collider view that will cause the player to get stuck between multiple colliders because it doesnt check for the players full bounds but just the head
9. players can cause a halt to a section of detection by getting teleported into another player because it doesnt handle that correctly
10. it is theorized that a cheater/ 3rd party client user could change values that they shouldn't be able to that could affect other players
11. blocked player sync sends data at a constant rate when it should only send data when it needs to - if multiple people are blocked could potentially be to much data to be handling causing network overhead to be worse
12. there is currently no anti stuck checks eg. player got misplaced between colliders - this will be made as a final failsafe as the detections should be able to place the player in a good place on its own but to fix potential issues this would be ideal to have as safeguard.

# -{Planned non feature things}-

-1. add translations for multiple languages, i need to find a translator will maybe setup temporary files for this.- wont be able to happen because i couldn't find anyone for this.

# -{TOS}-

by using, editing or redistributing FAC you agree to these terms:

1. you may make edits, changes and so on to FAC.
2. in some way shape or form provide proper credit to me for FAC if you make edits &/ or changes to FAC in some way.
3. you may not sell FAC, FAC is a F2U asset it is not to be sold by anyone for any reason all of the code found within FAC should not be sold nor should snippets.
4. you may redistribute FAC in any F2U way. (preferably via forking FAC on github)
5. you may make assets that directly use FAC or interact with FAC in any way shape or form.
6. if you're making your own similar system you can use FAC as a example, you may not copy-paste FAC code and call it your own.

# Attribution

it would be greatly appreciated if you place in your VRChat world an attribution prefab provided with this package.

you're not required to place or have any sort of message saying FAC is within your world as it is entirely optional but if you do it helps spread the word and supports the growth of this.

# -{Support}-

if you'd like to support my random shenanigans then feel free! https://www.patreon.com/freneticfurry
i intend to keep most if not all of my things free! supporting allows me to put more time and effort into F2U projects.

if you need help/ support with FAC please feel free to contact me on discord: freneticfurry
(also a good way to report bugs to me so they can be fixed making FAC overall better for everyone.)

# -{Credit}-

- Zerithax: alot of bug finding & some slight scripting help (only in versions .041 and below)
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
| Anti Blocked Avatar | set this to a gameObject to define the blocked avatar used, look at the default blocked avatar as a reference to set this up with a custom one. | Anti Block Default
| Enable Bounds | set to true to have the anti cheat detect the player going out of bounds set to false to have the anti cheat allow the player to go out of bounds. | False
| In Bounds Colliders | this is a array of colliders used for Enable Bounds if there is no colliders here the Enable Bounds doesnt work. | None
| Protect Mirrors Cameras Layers | this determines if the anti cheat will or will not protect against layer viewing primarily in mirror's or camera's | True
| Use Shader Block | this will determine if the anti cheat will use "ShaderBlock" $${\color{#ff0000}Warning}$$: will disable Usernames and Chatboxs, anything transparent will not render, change the ShaderBlock render queue to fix (changing the queue will also allow avatars to get around the ShaderBlock, set it up how you want it to be.) | False
| Protected Layers | this is a LayerMask only supporting 31 inputs as unity only allows for 32 layers, change to protect layers within the mirrors and camera's (this cannot be changed in realtime it is a startup process.) | Mixed
| Auto Pickup Verification | this is a bool to let FAC automaticly handle if pickups are or arnt allowed to be picked up or automaticly dropped when they're in places they shouldn't be | True
| Pickup Max Range | this is a float to determine the distance from the player it will be searching for tools in, recommended setting this to be 1.5 the Proximity of your biggest tool eg. tool a.Proximity is 10 should set this to be 15. | 5
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
| FAC._CanInteract(Object, AllowedDistance, IgnoredLayers, PlayerPosition, HandPosition) | returns a bool for if the HandPosition is allowed to interact with something, will return false if behind walls or such | can be used for any purpose you'd want this for, exposed Player Pose and Hand Pose for cases of entirely custom player controllers or similar |
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
