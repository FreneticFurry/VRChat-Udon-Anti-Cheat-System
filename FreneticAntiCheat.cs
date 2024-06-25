// VRChat anticheat by: Frenetic Furry!

using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FreneticAntiCheat : UdonSharpBehaviour
{
    [Header("Detection")]
    [Tooltip("This is the 'respawn' point for when someone gets detected as a cheater")]
    public Vector3 detectionPoint;
    [Tooltip("Uses a transform as a spawnpoint instead")]
    public bool enableSpawnPoint = false;
    public GameObject spawnPointLocation;
    [Tooltip("Distance from the detection area a player can be without the anti cheat doing anything")]
    public float detectionProtection = 2;

    [Header("Height")]
    [Tooltip("Maximum allowed height (ensure that 'Always Enforce Height' is turned on.)")]
    public float maxOVRAdvancedHeight = 0.9f;

    [Header("Automatic Settings")]
    [Tooltip("Automatically sets the maximum height allowed!")]
    public bool autoMaxOVRHeight = true;

    [Tooltip("Tells the anti cheat to automatically ignore pickupable items!")]
    public bool autoIgnorePickupables = true;

    [Header("Anti Cheat")]
    [Tooltip("Enables or Disables the anti cheat system")]
    public bool antiCheat = true;
    [Tooltip("Tells the system that the localplayer is teleporting or not & will allow them to teleport properly")]
    public bool isTeleporting = false;

    [Header("Debugging")]
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
    private Vector3 previousPosition;
    private Vector3 velocity;
    private Vector3 middlepoint;
    private VRCPlayerApi localPlayer;
    private bool isFlying = false;
    private float fTimer = 0f;
    private Vector3[] tableindex;
    private int CS = 0;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        SendCustomEventDelayedSeconds(nameof(CheckSpeed), 0.5f);
        SendCustomEventDelayedSeconds(nameof(CheckAvatarCollider), 0.5f);
        SendCustomEventDelayedSeconds(nameof(CheckOVRAdvanced), 15f); // assumes the avatar loads before 15 seconds is up, vrc doesnt enforce height properly upon avatar changes sometimes so this'll hopefully prevent most cases of a false flag upon joining.
        previousPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        tableindex = new Vector3[10];
    }

    // Player respawning protection
    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            isTeleporting = true;
            SendCustomEventDelayedSeconds(nameof(Respawned), 0.1f);
        }
    }

    public void Respawned()
    {
        isTeleporting = false;
    }

    // accurate velocity calculation, if you know of a better way that is accurate please feel free to let me know! (test your method at all fps types eg. low: 25 or below, med: 60, high: 140+) (could use lerp but thats slower then a linear approach)
    void FixedUpdate()
    {
        Vector3 currentPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Vector3 currentVelocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;

        tableindex[CS] = currentVelocity;
        CS = (CS + 1) % 10;

        Vector3 SV = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            SV += tableindex[i];
        }
        velocity = SV / 10;

        previousPosition = currentPosition;
    }

    // Anticheat main loop
    private void LateUpdate()
    {
        Vector3 localPlayerCameraPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        if (enableSpawnPoint)
        {
            detectionPoint = spawnPointLocation.transform.position;
        }

        if (Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat)
        {
            CheckColliderView(localPlayerCameraPosition);
            CheckLongArms(localPlayerCameraPosition);
        }
    }

    // anti Speed manip
    public void CheckSpeed()
    {
        if (Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat && !allowSpeedManipulation)
        {
            if (new Vector3(velocity.x, 0f, velocity.z).magnitude > maxSpeed)
            {
                isDetected();
            }

            maxSpeed = localPlayer.GetRunSpeed() * (allowBhopping && localPlayer.GetVelocity().y < -0.25f || (localPlayer.GetVelocity().y > 0.25f) ? 1.6f : 1.35f);
        }
        else
        { maxSpeed = math.INFINITY; }
        SendCustomEventDelayedSeconds(nameof(CheckSpeed), 0.05f);
    }

    // Anti Collider flying
    public void CheckAvatarCollider()
    {
        if (Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat && !allowFlight)
        {
            Collider[] colliders = Physics.OverlapSphere(localPlayer.GetPosition(), 0.1f, Physics.AllLayers & ~(1 << LayerMask.NameToLayer("PlayerLocal")) & ~(1 << LayerMask.NameToLayer("Player")));

            if (colliders.Length == 0)
            { }
            else
            {
                foreach (Collider collider in colliders)
                {
                    if (collider != null && collider.gameObject != null && collider.gameObject.activeInHierarchy)
                    { break; }
                    else
                    {
                        isDetected();
                        break;
                    }
                }
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckAvatarCollider), 0.05f);
    }

    // anti OVR/ Gogo Loco view manip
    public void CheckOVRAdvanced()
    {
        if (autoMaxOVRHeight)
        {
            float height = localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.5f;
            maxOVRAdvancedHeight = height * 1.4f;
            middlepoint = new Vector3(0f, height, 0f);
        }
        if (Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat && !allowOVRAdvanced && Vector3.Distance(localPlayer.GetPosition() + middlepoint, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position) >= maxOVRAdvancedHeight)
        {
            isDetected();
        }
        SendCustomEventDelayedSeconds(nameof(CheckOVRAdvanced), 0.05f);
    }

    // Anti Collider viewing / Anti putting head through colliders
    private void CheckColliderView(Vector3 localPlayerCameraPosition)
    {
        if (Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat && !allowColliderView)
        {
            Collider[] colliders = Physics.OverlapSphere(localPlayerCameraPosition, 0.1f, Physics.AllLayers & ~(1 << LayerMask.NameToLayer("PlayerLocal")) & ~(1 << LayerMask.NameToLayer("Player")));
            foreach (Collider collider in colliders)
            {
                if (collider != null && Vector3.Distance(localPlayerCameraPosition, collider.ClosestPointOnBounds(localPlayerCameraPosition)) <= 0.1f)
                {
                    bool skipCollider = false;
                    if (autoIgnorePickupables)
                    {
                        VRC.SDK3.Components.VRCPickup pickup = collider.GetComponent<VRC.SDK3.Components.VRCPickup>();
                        if (pickup != null || (collider.transform.parent != null && collider.transform.parent.GetComponentInParent<VRC.SDK3.Components.VRCPickup>() != null))
                        {
                            skipCollider = true;
                        }
                    }

                    if (!skipCollider)
                    {
                        switch (collider.gameObject.name.ToLower())
                        {
                            case "examplecollider":
                            case "bounding box example":
                            case "bounding box second example":
                                // anything with these names can be used for things like Triggers or other things!
                                break;
                            default:
                                float push = 0.1f - Vector3.Distance(localPlayerCameraPosition, collider.ClosestPointOnBounds(localPlayerCameraPosition));
                                localPlayer.TeleportTo(localPlayer.GetPosition() + (localPlayerCameraPosition - collider.ClosestPointOnBounds(localPlayerCameraPosition)).normalized * push, localPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                                break;
                        }
                    }
                }
            }
        }
    }

    // Anti Long Arms
    private void CheckLongArms(Vector3 localPlayerCameraPosition)
    {
        if (!allowLongArms)
        {
            float maxArmLength = localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.677f;
            if (Vector3.Distance(localPlayerCameraPosition, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position) > maxArmLength || Vector3.Distance(localPlayerCameraPosition, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position) > maxArmLength)
            {
                isDetected();
            }
        }
    }

    // Detection Area
    private void isDetected()
    {
        localPlayer.TeleportTo(detectionPoint, Quaternion.identity, VRC_SceneDescriptor.SpawnOrientation.Default, false);
        localPlayer.SetVelocity(Vector3.zero);
    }
}
