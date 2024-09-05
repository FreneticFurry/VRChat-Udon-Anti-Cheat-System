# ----VRChat-Udon-Anti-Cheat-System----
# **{Slight warning about the VRC Team}**

The VRChat devs have hired some sus (pedo) individuals & there could be more so my suggestion is just not interacting with the VRChat team :)

# **{Features}**

1. Collider Flying
2. Seat Cheating
3. OVR Advanced & gogo loco abuse
4. Menu Bugging
5. Sticking heads through walls
6. Reach/ Long arm manipulation
7. Speed manipulation
8. Mirror's & Camera cheating

this will not prevent someone who is using 3rd party software, that is EAC's job not mine!

# **{How To Use}**

1. download from the releases section and place anti cheat prefab into the scene!
2. ensure that the "Enforce Player Height" is turned on in VRCWorld

if a seat exists please use the "Seat" code on it so it will actually work!

# **{VRC Restrictions}**

1. VRChat devs do not allow access to avatars so this is about the best that i can think of for things such as collider flying
2. if the vrchat devs with Udon 2 decide to not be inept then actual things can be created! if access & other things happen with Udon 2 this'll get a update accordingly to hopefully make it better!
3. unfortunately world creators also may not outright ban players from their worlds so this system will detect and allow the creator to either a respawn them after being detected as a cheater or b place them into a per server cheater room to ban players per instance (ik its dumb but its the vrchat devs so what'd you expect from them.)

# **{Planned Future Updates}**

1. adding detection for: Player Blocking, if a player blocks another then they cannot see each other and in something like a game world that makes things very difficult so making players visible when blocked will surfice!

# **{Known Issues}**

1. Players can use "UI" to slightly bypass the anti Mirror's & Camera but will eventually get fixed if possible.
2. Player Ragdolls DONT work currently & this may not get fixed because ragdolls are very very cosmetic.
3. i forgot to do the Spawn Protection so it just doesnt work currently, oopsies (will be fixed in the next update)

# **{Terms}**

you may redistribute this code & even edit it, i only ask that you credit me for the code!

# **{Support}**

if you'd like to support my random shananagains then feel free! https://www.patreon.com/freneticfurry
i intend to keep most if not all of my things free so supporting would be very nice :3

# **{Credit}**

- RyuukaVR: some bug finding

# **----AntiCheat Documentation----**

this is the documentation for the AntiCheat, moved to here instead of being tooltips as developers have requested!

# **{Settings}**

1. 'antiObject' - this is the object that would contain the shader for Anti Mirrors & Camera's to prevent players from abusing them
2. 'detectionPoint' - the location where a player who is cheating will be sent to! alternatively can use "EnableSpawnPoint" to instead use a object's position instead
3. 'enableSpawnPoint' - distables the "Detection Point" and instead uses a object to determine where to place a player if they've cheated instead
4. 'spawnPointLocation' - only gets used if "Enable SpawnPoint" is turned on to determine where to place the player if they've cheated
5. 'detectionProtectionRadius' - a radius from the Detection area where players can do anything without being detected
6. 'allowedColliderNames' - separated with a "," takes names for colliders to ignore for detection! example usage: "examplecollider, testobject, among us imposter!" it is case-insensitive so object that're named "ExampleCollider" would also be ignored
7. 'maxOVRAdvancedHeight' - the maximum allowed height a player can be at before being detected as cheating, can use the "autoMaxOVRHeight" if you dont want to manually set this
8. 'autoMaxOVRHeight' - automatically sets the maximum possible allowed height, in VRCWorld "Enforce Height" should be enabled
9. 'autoIgnorePickupables' - if set to true it will ignore any object that a player can pickup similar to inputting a name into the: "allowedColliderNames"
10. 'antiCheat' - turns the anticheat on or off
11. 'isTeleporting' - this tells that anticheat when a player is teleporting (remember to set it to false when you're done with a teleport)

# **{Debug}**

1. 'allowBhopping' - allows for faster speeds, if jumping is allowed in a world this should be turned to true but its also recommended to have it on regardless,  recommended value: <span style="color: green;">True</span>
2. 'allowLongArms' - allows players to have "LongArms" this is recommended to be turned on because players like to sometimes use avatar with very long arms or edit their steamvr settings for extended reaching,  recommended value: <span style="color: red;">False</span>
3. 'allowFlight' - allows players to use things like colliders within a avatar to fly within a world, recommended value: <span style="color: red;">False</span>
4. 'allowOVRAdvanced' - allows players to use GogoLoco/ Ovr Advanced settings to move their view into places that it shouldn't be,  recommended value: <span style="color: red;">False</span>
5. 'allowColliderView' - allows players to place their heads into walls, recommended value: <span style="color: red;">False</span>
6. 'allowSpeedManipulation' - allows players to gain speed via things like OVR advanced, recommended value: <span style="color: red;">False</span>
7. 'printDetection' - this will print what is being detected into the console so you can manually test cases of cheating and debug the world, recommended value: <span style="color: red;">False</span>
8. 'AllowPersonalMirrors_Cameras' - allows players to use camera's or mirrors to see other players or the enviorment, recommended value: <span style="color: red;">False</span> (you can also go within the prefab to set what it will prevent the player from seeing)

# **{Detection Attempts}**

visible within a script you can do antiCheat.LongArmAttempts to get a value telling you how many times a user has abused a singular thing so you can setup special things that may happen when abusing 1 thing to much maybe, not recommended to write values eg. antiCheat.LogArmAttempts = 0

1. 'LongArmAttempts' - returns the amount of times a player has attempted to have long arms
2. 'FlightAttempts' - returns the amount of times a player has attempted to use a avatar collider to fly
3. 'OVR_GoGoLocoAttempts' - returns the amount of times a player has attempted to abuse OVR/GogoLoco
4. 'ColliderViewAttempts' - returns the amount of times a player has attempted to stick their head within a collider (this gets spammed alot so it isnt recommended to setup anything for it)
5. 'SpeedManipulationAttempts' - returns the amount of times a player has attempted to use something like OVR Advanced Settings to give themselves more speed then they should have
6. 'SeatAttempts' - returns the amount of times a player has entered or left a seat (this is combined so if a player enters a seat it will also say when they left a seat so 1 = im in a seat 2 = im noy in a seat, requires the seat to have the "Seat.cs" applied on it)
7. 'RespawnAttempts' - returns the amount of times a player has respawned

# **{Functions}**

1. 'TeleportPlayer(Vector3 Position, Quaternion Rotation, VRC_SceneDescriptor.SpawnOrientation SpawnOrientation, Bool smooth)' - allows a player to properly teleport, a replacement for: "localplayer.TeleportTo"
2. 'SetPlayerVelocity(Vector3 Velocity)' - allows a player to properly use the 'SetVelocity', a replacement for: "localplayer.SetVelocity()"
3. 'Detected' - this function is called for every single detection method used, this is used to tell the anticheat 'this player has been detected! move them to the "DetectionPoint"'
4. 'PTC(String Message)' - "Print To Console" prints out "[Frenetic Anti Cheat]: YourMessageHere has been detected!" but will only work if the "printDetection" is set to true

**-Helper Functions-**

1. 'resetvelo' - properly resets the velocity for the function: "SetPlayerVelocity(Vector3 Velocity)"
2. 'Teleported' - properly resets the "TeleportPlayer(Vector3 Position, Quaternion Rotation, VRC_SceneDescriptor.SpawnOrientation SpawnOrientation, Bool smooth)"
3. 'OnPlayerRespawn' - this is a override it tells the anticheat that a player has respawned and properly allows them to
4. 'Seat' - used within the "Seat.cs" allows players to use seats properly
