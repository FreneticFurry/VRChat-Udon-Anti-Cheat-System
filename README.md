# VRChat-Udon-Anti-Cheat-System
{What This protects against}

1. Collider Flying
2. Seat Cheating
3. OVR Advanced & gogo loco abuse
4. Menu Bugging
5. Sticking heads through walls
6. Long Armed Avatars
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

{Do Note}

things can change at any moment! mainly detection methods so trying to stay up to date with this is recommended!

this is meant to be pretty easy to comprehend even for someone who doesnt have the most experience with C# so that is why there is alot of comments that tell you what detects what :D

{Planned Future Updates}

1. Editor UI or a method to automatically setup anti-personal mirror & anti-camera to prevent "ESP"ing/ seeing players through walls

{Terms}

you may redistribute this code & even edit it, i only ask that you credit me for the code!

{Support}

if you'd like to support my random shananagains then feel free! https://www.patreon.com/freneticfurry
i intend to keep most if not all of my things free so supporting would be very nice :3
