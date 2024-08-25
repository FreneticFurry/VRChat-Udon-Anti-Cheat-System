# VRChat-Udon-Anti-Cheat-System
{VRChat general warning}

as a general notice: i suggest to avoid talking with/ interacting with the VRChat team, multiple of the moderators have been shown be pedofiles and its unknown how many more of them are also pedofiles.
none the less! happy Udoning :)

{What This protects against}

1. Collider Flying
2. Seat Cheating
3. OVR Advanced & gogo loco abuse
4. Menu Bugging
5. Sticking heads through walls
6. Reach/ Long arm manipulation
7. Speed manipulation

this will not prevent someone who is using 3rd party software, that is EAC's job not mine!

{How To Use}

1. download from the releases section and place anti cheat prefab into the scene!
2. ensure that the "Enforce Player Height" is turned on in VRCWorld

if seats exist then make sure to have a script to check when the player wants to enter and exit the seat in question as it is considered teleporting when using a seat!

if pickupable items exist also make sure to mark them properly in the anti cheat so they wont collide with the players camera (unless you want them to collide then just dont exclude them)

(if i ever figure it out it'll also be able to be used with the VRC Creator Companion)

{Complications}

1. VRChat devs do not allow access to avatars so this is about the best that i can think of for things such as collider flying
2. if the vrchat devs with Udon 2 decide to not be inept then actual things can be created! if access & other things happen with Udon 2 this'll get a update accordingly to hopefully make it better!
3. unfortunately world creators also may not outright ban players from their worlds so this system will detect and allow the creator to either a respawn them after being detected as a cheater or b place them into a per server cheater room to ban players per instance (ik its dumb but its the vrchat devs so what'd you expect from them.)

{Planned Future Updates}

1. Editor UI or a method to automatically setup anti-personal mirror & anti-camera to prevent "ESP"ing/ seeing players through walls
2. ~Overhauling some detection methods with some better & newer ones (should happen sometime soon?)~ (done)
3. adding detection for: Player Blocking, if a player blocks another then they cannot see each other and in something like a game world that makes things very difficult so making players visible when blocked will surfice!

{Known Issues}

1. the anti avatar collider detection will detect things like Menus & personal mirrors... this is fine because in order for this to happen a player must intentionally place a menu or mirror at their feet so if anyone does this then its entirely their own fault!
2. potentially some weird cases where Speed Manipulation detection will false flag (though its unlikely/ shouldn't...)
3. Using VRChat's native methods of TeleportTo & SetVelocity dont work! instead use the new function: antiCheat.TeleportPlayer(Position, Rotation, SpawnOrientation, Smooth), antiCheat.SetPlayerVelocity() (until vrchat adds proper hooking i cant do anything about it besides just remaking the functions themselves like this!)

{Terms}

you may redistribute this code & even edit it, i only ask that you credit me for the code!

{Contacting}

if you'd like to contribute/ help with this project then feel free! my discord is: freneticfurry    im very active and will most likely respond decently quickly! so dont feel shy to send a quick dm as im always down for a chat

{Support}

if you'd like to support my random shananagains then feel free! https://www.patreon.com/freneticfurry
i intend to keep most if not all of my things free so supporting would be very nice :3

{Credit}

- RyuukaVR: some bug finding
