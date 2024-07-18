using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FreneticAntiCheat : UdonSharpBehaviour
{
    [Header("Detection")]
    [Space]
    [Tooltip("This is the 'respawn' point for when someone gets detected as a cheater")]
    public Vector3 detectionPoint;
    [Tooltip("Uses a transform as a spawnpoint instead")]
    public bool enableSpawnPoint = false;
    public GameObject spawnPointLocation;
    [Tooltip("Distance from the detection area a player can be without the anti cheat doing anything")]
    public float detectionProtection = 2;
    [Tooltip("Allowed Colliders to be used for other things such as Triggers (not case sensitive & separated with ,)")]
    public string allowedColliderNames = "examplecollider, bounding box example, bounding box second example";
    [Header("Height")]
    [Space]
    [Tooltip("Maximum allowed height (ensure that 'Always Enforce Height' is turned on.)")]
    public float maxOVRAdvancedHeight = 0.9f;
    [Header("Automatic Settings")]
    [Space]
    [Tooltip("Automatically sets the maximum height allowed!")]
    public bool autoMaxOVRHeight = true;
    [Tooltip("Tells the anti cheat to automatically ignore pickupable items!")]
    public bool autoIgnorePickupables = true;
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
    [Tooltip("Enable or disable printing detection messages to the console")]
    public bool printDetection = true;

    private string[] funnycolliders;
    private float maxSpeed, jumpspeed, runspeed, spt, ct;
    private Vector3 previousPosition, velocity, middlepoint, pv, camerapos;
    private VRCPlayerApi localPlayer;
    private Vector3[] funnytable;
    private int CS;
    private bool velocityChanged;

    // Startup \\

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        SendCustomEventDelayedSeconds(nameof(CheckSpeed), 0.5f);
        SendCustomEventDelayedSeconds(nameof(CheckViewing), 0.5f);
        SendCustomEventDelayedSeconds(nameof(CheckAvatarCollider), 0.5f);
        SendCustomEventDelayedSeconds(nameof(CheckOVRAdvanced), 15f);

        previousPosition = camerapos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        funnytable = new Vector3[10];
        jumpspeed = 3.75f;
        runspeed = 2.75f;
        spt = localPlayer.GetRunSpeed() + 0.5f;

        string[] tempArray = allowedColliderNames.ToLower().Split(',');
        funnycolliders = new string[tempArray.Length];
        for (int i = 0; i < tempArray.Length; i++)
        {
            funnycolliders[i] = tempArray[i].Trim();
        }

        if (enableSpawnPoint && spawnPointLocation != null)
        {
            detectionPoint = spawnPointLocation.transform.position;
        }
    }

    // Anti Cheat \\

    void FixedUpdate()
    {
        camerapos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Vector3 currentVelocity = (camerapos - previousPosition) / Time.fixedDeltaTime;
        funnytable[CS] = currentVelocity;
        CS = (CS + 1) % 10;

        velocity = Vector3.zero;
        for (int i = 0; i < 10; i++) velocity += funnytable[i];
        velocity /= 10;

        previousPosition = camerapos;

        Vector3 playerVelocity = localPlayer.GetVelocity();
        maxSpeed = localPlayer.GetRunSpeed() + (allowBhopping && (playerVelocity.y < -0.25f || playerVelocity.y > 0.25f) ? jumpspeed : runspeed);
    }

    public void CheckSpeed()
    {
        if (AC() && !allowSpeedManipulation)
        {
            Vector3 hv = new Vector3(velocity.x, 0f, velocity.z);
            if (hv.magnitude > maxSpeed)
            {
                PTC("Speed Manipulation");
                Detected();
            }
            if ((hv - new Vector3(pv.x, 0f, pv.z)).magnitude > spt)
            {
                PTC("Sudden Speed");
                Detected();
            }
            pv = velocity;
        }
        SendCustomEventDelayedSeconds(nameof(CheckSpeed), 0.05f);
    }

    public void CheckOVRAdvanced()
    {
        if (autoMaxOVRHeight)
        {
            float height = localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.5f;
            maxOVRAdvancedHeight = height * 1.4f;
            middlepoint = new Vector3(0f, height, 0f);
        }

        if (AC() && !allowOVRAdvanced && Vector3.Distance(localPlayer.GetPosition() + middlepoint, camerapos) >= maxOVRAdvancedHeight)
        {
            PTC("OVR/ Gogo Loco");
            Detected();
        }
        SendCustomEventDelayedSeconds(nameof(CheckOVRAdvanced), 0.05f);
    }

    private void CheckLongArms()
    {
        if (AC() && !allowLongArms)
        {
            float arml = localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.677f;

            if (Vector3.Distance(camerapos, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position) > arml || Vector3.Distance(camerapos, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position) > arml)
            {
                PTC("Long Arms");
                Detected();
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckOVRAdvanced), 0.05f);
    }

    public void CheckAvatarCollider()
    {
        if (AC() && !allowFlight)
        {
            Collider[] colliders = Physics.OverlapSphere(localPlayer.GetPosition(), 0.025f, 1 << LayerMask.NameToLayer("PlayerLocal"));

            foreach (Collider collider in colliders)
            {
                if (collider == null || collider.gameObject == null || !collider.gameObject.activeInHierarchy)
                {
                    PTC("Avatar Collider");
                    Detected();
                    break;
                }
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckAvatarCollider), 0f);
    }

    public void CheckViewing()
    {
        if (AC() & !allowColliderView)
        {
            Collider[] colliders = Physics.OverlapSphere(camerapos, 0.1f, Physics.AllLayers & ~(1 << LayerMask.NameToLayer("PlayerLocal")) & ~(1 << LayerMask.NameToLayer("Player")));

            foreach (Collider collider in colliders)
            {
                if (collider != null && collider.gameObject != null)
                {
                    bool isAllowed = false;

                    if (collider.GetComponent<VRCStation>() != null ||
                        (collider.transform.parent != null && collider.transform.parent.GetComponent<VRCStation>() != null))
                    {
                        isAllowed = true;
                    }
                    else
                    {
                        for (int i = 0; i < funnycolliders.Length; i++)
                        {
                            if (funnycolliders[i] == collider.gameObject.name.ToLower().Trim())
                            {
                                isAllowed = true;
                                break;
                            }
                        }

                        if (!isAllowed && autoIgnorePickupables)
                        {
                            VRC.SDK3.Components.VRCPickup pickup = collider.GetComponent<VRC.SDK3.Components.VRCPickup>();
                            if (pickup == null && collider.transform.parent != null)
                            {
                                pickup = collider.transform.parent.GetComponentInParent<VRC.SDK3.Components.VRCPickup>();
                            }
                            if (pickup != null)
                            {
                                isAllowed = true;
                            }
                        }
                    }

                    if (!isAllowed)
                    {
                        PTC("Collider Viewing");
                        Vector3 reflection = Vector3.Reflect(localPlayer.GetVelocity(), (camerapos - collider.ClosestPoint(camerapos)).normalized) * 0.8f;
                        reflection = reflection.magnitude < 0.5f ? reflection.normalized * 2f : reflection;
                        localPlayer.SetVelocity(reflection);
                        if (localPlayer.IsUserInVR())
                        {
                            localPlayer.TeleportTo(localPlayer.GetPosition(), localPlayer.GetRotation());
                        }
                        else
                        {
                            localPlayer.TeleportTo(localPlayer.GetPosition() + (camerapos - collider.ClosestPoint(camerapos)).normalized * 0.045f, localPlayer.GetRotation());
                        }
                        break;
                    }
                }
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckViewing), 0.01f);
    }

    // Helper Functions \\

    private bool AC()
    {
        return Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat;
    }

    private void PTC(string item)
    {
        if (printDetection)
        {
            Debug.Log($"[<color=#007d0e>Frenetic Anti Cheat:</color>] [<color=#aa0104>{item}</color>] has been detected!");
        }
    }

    public void resetvelo()
    {
        if (!velocityChanged)
        {
            velocityChanged = true;
            ct = Time.time;
            if (localPlayer.GetVelocity().magnitude < localPlayer.GetRunSpeed() + 3.75f)
            {
                if (Time.time - ct >= 0.25f)
                {
                    jumpspeed = 3.75f;
                    runspeed = 2.75f;
                    spt = localPlayer.GetRunSpeed() + 0.5f;
                    velocityChanged = false;
                }
                else
                {
                    SendCustomEventDelayedSeconds(nameof(resetvelo), 0.05f);
                }
            }
            else
            {
                velocityChanged = false;
                SendCustomEventDelayedSeconds(nameof(resetvelo), 0.05f);
            }
        }
    }

    public void Teleported()
    {
        spt = localPlayer.GetRunSpeed() + 0.5f;
        isTeleporting = false;
    }

    public void Detected()
    {
        localPlayer.TeleportTo(detectionPoint, Quaternion.identity, VRC_SceneDescriptor.SpawnOrientation.Default, false);
        localPlayer.SetVelocity(Vector3.zero);
    }

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            spt = float.PositiveInfinity;
            isTeleporting = true;
            localPlayer.SetVelocity(Vector3.zero);
            SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
            PTC("Respawning");
        }
    }

    // Recreated VRChat functions to add proper support for them (because vrchat doesnt allow you to hook onto functions afaik!) \\

    public void SetPlayerVelocity(Vector3 velo) // Example usage: antiCheat.SetPlayerVelocity(new Vector3(0,5,0))
    {
        resetvelo();
        if (allowBhopping)
        {
            spt = velo.magnitude + 4f;
            jumpspeed = velo.magnitude + 3.75f;
            runspeed = velo.magnitude + 2.75f;
        }
        else
        {
            runspeed = velo.magnitude + 2.75f;
        }
        localPlayer.SetVelocity(velo);
    }

    public void TeleportPlayer(Vector3 pos, Quaternion rot, VRC_SceneDescriptor.SpawnOrientation sori, bool smooth) // Example usage: antiCheat.TeleportPlayer(Position, Rotation, SpawnOrientation, Smooth), antiCheat.TeleportPlayer(new Vector3(0,5,0), Quaternion.identity, VRC_SceneDescriptor.SpawnOrientation.Default, false);
    {
        localPlayer.TeleportTo(pos, rot, sori, smooth);
        spt = math.INFINITY;
        isTeleporting = true;
        SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
    }

    public void Seat()
    {
        PTC("VRCStation");
        spt = math.INFINITY;
        isTeleporting = true;
        SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
    }
}
