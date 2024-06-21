// VRChat anti cheat system by: FreneticFurry!
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FreneticAntiCheat : UdonSharpBehaviour
{
    // thanks to Zerithax for telling me about tooltips :) (i had initially thought it was only for shaders)
    [Header("Detection")]
    [Space]
    [Tooltip("This is the 'respawn' point for when someone gets detected as a cheater")]
    public Vector3 detectionPoint;

    [Tooltip("Uses a transform as a spawnpoint instead")]
    public bool enableSpawnPoint = false;

    public GameObject spawnPointLocation;

    [Tooltip("Distance from the detection area a player can be without the anti cheat doing anything")]
    public float detectionProtection = 2;

    [Header("Height")]
    [Space]
    [Tooltip("Maximum allowed height")]
    public float maxOVRAdvancedHeight = 0.9f;

    [Header("Flying")]
    [Space]
    [Tooltip("Radius from the ground before considering if the player is flying/ cheating")]
    public float FlyingDistThreshold = 0.5f;

    [Tooltip("Time required to be considered flying/ cheating in the air (recommended 0.3+)")]
    public float flyTime = 0.3f;

    [Header("Automatic Settings")]
    [Space]
    [Tooltip("Automatically sets the maximum speed based on VRCWorld RunSpeed (may not work well at some speeds)")]
    public bool autoMaxSpeed = true;

    [Tooltip("Automatically sets the Maximum height someone can go before being detected as a cheater")]
    public bool autoMaxOVRHeight = true;

    [Tooltip("Automatically set the maximum distance from the ground before considering that someone might be flying")]
    public bool autoFlyingThreshold = true;

    [Header("Anti Cheat")]
    [Space]
    [Tooltip("Enables or Disables the anti cheat system")]
    public bool antiCheat = true;

    [Tooltip("Tells the system that the localplayer is teleporting or not & will allow them to teleport properly")]
    public bool isTeleporting = false;

    [Header("Debugging")]
    [Space]
    [Tooltip("Allow or disallow Bhopping! (Recommended: true unless players cant jump & wont be falling)")]
    public bool allowBhopping = true;

    [Tooltip("Allow or disallow Long reaching!")]
    public bool allowLongArms = false;

    [Tooltip("Allow or disallow avatars to fly")]
    public bool allowFlight = false;

    [Tooltip("Allow or disallow players to use OVR advanced or GoGo Loco for view adjusting")]
    public bool allowOVRAdvanced = false;

    [Tooltip("Allow or disallow players to look though walls with their head")]
    public bool allowColliderView = false;

    [Tooltip("Allow or disallow players to alter their speed with things such as colliders or OVR advanced")]
    public bool allowSpeedManipulation = false;

    private float maxSpeed = 0;
    private float ftimer = 0f;
    private float gTimer = 0f;
    private Vector3 previousPosition;
    private Vector3 velocity;
    private float timer = 0f;
    private Vector3 middlepoint = new Vector3(0f, 0.65f, 0f);

    // Respawning \\

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            isTeleporting = true;
            SendCustomEventDelayedSeconds("Respawned", 0.1f); // thanks to Zerithax for also suggesting to do this (would've done yet another wait in a loop probably)
        }
    }

    public void Respawned()
    {
        isTeleporting = false;
    }

    // Anti Cheat System \\
    private void Update()
    {
        // Variables \\
        Vector3 localPlayerCameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        // Automatic system \\

        if (autoFlyingThreshold)
        {
            FlyingDistThreshold = Networking.LocalPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.9615384615384615f;
        }
        if (autoMaxOVRHeight)
        {
            float height = Networking.LocalPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.5f;
            maxOVRAdvancedHeight = height * 1.4f;
            middlepoint = new Vector3(0f, height, 0f);
        }

        // Spawnpoint \\

        if (enableSpawnPoint)
        {
            detectionPoint = spawnPointLocation.transform.position;
        }

        // Detection Protection \\
        if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && isTeleporting == false && antiCheat == true)
        {
            // Speed Detection \\
            if (allowSpeedManipulation == false)
            {
                timer += Time.deltaTime;

                if (timer >= 0.05f)
                {
                    timer = 0f;

                    Vector3 amount = localPlayerCameraPosition - previousPosition;
                    velocity = amount / 0.05f;

                    if (new Vector3(velocity.x, 0f, velocity.z).magnitude > maxSpeed)
                    {
                        Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                        Networking.LocalPlayer.SetVelocity(Vector3.zero);
                    }

                    previousPosition = localPlayerCameraPosition;
                }
            }

            if (allowBhopping)
            {
                if (Networking.LocalPlayer.GetVelocity().y < -0.25f)
                {
                    maxSpeed = Networking.LocalPlayer.GetRunSpeed() * 2.2f;
                }
                else
                {
                    maxSpeed = Networking.LocalPlayer.GetRunSpeed() * 1.7f;
                }
            }
            else
            {
                maxSpeed = Networking.LocalPlayer.GetRunSpeed() * 1.7f;
            }

            // Flying detection \\
            if (allowFlight == false)
            {
                if (velocity.y > 0.1f)
                {
                    ftimer += Time.deltaTime;
                    gTimer = 0f;

                    Collider[] Colliders = Physics.OverlapSphere(Networking.LocalPlayer.GetPosition() + middlepoint, FlyingDistThreshold, Physics.AllLayers & ~(1 << LayerMask.NameToLayer("PlayerLocal")) & ~(1 << LayerMask.NameToLayer("Player")));

                    bool ValidCollider = false;

                    foreach (Collider collider in Colliders)
                    {
                        if (collider != null && collider.gameObject != null)
                        {
                            if (collider.gameObject.activeInHierarchy)
                            {
                                if (collider.gameObject.layer != LayerMask.NameToLayer("PlayerLocal") && collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                                {
                                    ValidCollider = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!ValidCollider)
                    {
                        if (ftimer >= flyTime)
                        {
                            Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                            Networking.LocalPlayer.SetVelocity(Vector3.zero);
                            ftimer = 0f;
                        }
                    }
                    else
                    {
                        ftimer = 0f;
                    }
                }
                else
                {
                    gTimer += Time.deltaTime;

                    if (gTimer >= 0.5f)
                    {
                        ftimer = 0f;
                        gTimer = 0f;
                    }
                }
            }

            // Collider detection \\
            if (allowColliderView == false)
            {
                Collider[] colliders = Physics.OverlapSphere(localPlayerCameraPosition, 0.1f, Physics.AllLayers & ~(1 << LayerMask.NameToLayer("PlayerLocal") & ~(1 << LayerMask.NameToLayer("Player"))));

                foreach (Collider collider in colliders)
                {
                    if (collider != null)
                    {
                        if (Vector3.Distance(localPlayerCameraPosition, collider.ClosestPointOnBounds(localPlayerCameraPosition)) <= 0.1f)
                        {
                            switch (collider.gameObject.name.ToLower())
                            {
                                case "ExampleCollider":
                                case "examplecollider":
                                case "Bounding Box Example":
                                case "bounding box second Example": // if a object has any of these names & a collider it'll get ignored & allow them to b used for things like triggers & other things
                                    break;
                                default:
                                    float push = 0.1f - Vector3.Distance(localPlayerCameraPosition, collider.ClosestPointOnBounds(localPlayerCameraPosition)); // instead of placing the player back to detection area pushing them away would be better because someone in vr may mistakenly put their head in or to close to the wall so this acts to forgive that :)
                                    Networking.LocalPlayer.TeleportTo(Networking.LocalPlayer.GetPosition() + (localPlayerCameraPosition - collider.ClosestPointOnBounds(localPlayerCameraPosition)).normalized * push, Networking.LocalPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                                    break;
                            }
                        }
                    }
                }
            }

            // Distance detection (prevention for OVR advanced abuse :D ) \\
            if (allowOVRAdvanced == false)
            {
                if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + middlepoint, localPlayerCameraPosition) >= maxOVRAdvancedHeight)
                {
                    Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    Networking.LocalPlayer.SetVelocity(Vector3.zero);
                }
            }

            // Anti long arm abuse \\
            if (allowLongArms == false)
            {
                if (Vector3.Distance(localPlayerCameraPosition, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position) > Networking.LocalPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.6769230769230769f || Vector3.Distance(localPlayerCameraPosition, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position) > Networking.LocalPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.6769230769230769f)
                {
                    Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    Networking.LocalPlayer.SetVelocity(Vector3.zero);
                }
            }
        }
        else
        {
            // reset for when the anti cheat is turned off so when it turns back on it wont falsely trigger \\
            velocity = new Vector3(0, 0, 0);
            previousPosition = localPlayerCameraPosition;
        }
    }
}
