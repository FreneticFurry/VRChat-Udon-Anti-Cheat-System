using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FreneticAntiCheat : UdonSharpBehaviour
{
    [Header("Detection")]
    public Vector3 detectionPoint;
    public bool enableSpawnPoint = false;
    public GameObject spawnPointLocation;
    public float detectionProtection = 2;
    public string allowedColliderNames = "examplecollider, bounding box example, bounding box second example";

    [Header("Height")]
    public float maxOVRAdvancedHeight = 0.9f;

    [Header("Automatic Settings")]
    public bool autoMaxOVRHeight = true;
    public bool autoIgnorePickupables = true;

    [Header("Anti Cheat")]
    public bool antiCheat = true;
    public bool isTeleporting = false;
    public bool isRagdoll = false;

    [Header("Debugging")]
    public bool allowBhopping = true;
    public bool allowLongArms = false;
    public bool allowFlight = false;
    public bool allowOVRAdvanced = false;
    public bool allowColliderView = false;
    public bool allowSpeedManipulation = false;
    public bool printDetection = true;

    [HideInInspector] public int LongArmAttempts;
    [HideInInspector] public int FlightAttempts;
    [HideInInspector] public int OVR_GoGoLocoAttempts;
    [HideInInspector] public int ColliderViewAttempts;
    [HideInInspector] public int SpeedManipulationAttempts;
    [HideInInspector] public int SeatAttempts;
    [HideInInspector] public int RespawnAttempts;
    
    private string[] funnycolliders;
    private float maxSpeed, jumpspeed = 3.75f, runspeed = 2.75f, spt, ct, lt, isp;
    private Vector3 previousPosition, velocity, middlepoint, camerapos, lastknowngood;
    private VRCPlayerApi localPlayer;
    private Vector3[] funnytable = new Vector3[10];
    private int CS, AIS;
    private bool velocityChanged;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        previousPosition = camerapos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        spt = localPlayer.GetRunSpeed() + 0.5f;
        funnycolliders = allowedColliderNames.ToLower().Split(',');
        for (int i = 0; i < funnycolliders.Length; i++) funnycolliders[i] = funnycolliders[i].Trim();

        if (enableSpawnPoint && spawnPointLocation != null)
            detectionPoint = spawnPointLocation.transform.position;

        SendCustomEventDelayedSeconds(nameof(CheckStuff), 0.5f);
    }

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

    public void CheckStuff()
    {
        if (AC())
        {
            if (!allowSpeedManipulation && new Vector3(velocity.x, 0f, velocity.z).magnitude > maxSpeed)
            {
                PTC("Speed Manipulation");
                SpeedManipulationAttempts += 1;
                Detected();
            }

            if (!allowLongArms)
            {
                float arml = localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.677f;
                if (Vector3.Distance(camerapos, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position) > arml || Vector3.Distance(camerapos, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position) > arml)
                {
                    PTC("Long Arms");
                    LongArmAttempts += 1;
                    Detected();
                }
            }

            if (!allowFlight)
            {
                Collider[] colliders = Physics.OverlapSphere(localPlayer.GetPosition(), 0.025f, 1 << LayerMask.NameToLayer("PlayerLocal"));
                foreach (Collider collider in colliders)
                {
                    if (collider == null || collider.gameObject == null || !collider.gameObject.activeInHierarchy)
                    {
                        PTC("Avatar Collider");
                        FlightAttempts += 1;
                        Detected();
                        break;
                    }
                }
            }

            if (!allowColliderView)
            {
                bool iic = false;
                bool inc = false;

                foreach (Collider collider in Physics.OverlapSphere(camerapos, 0.1f, Physics.AllLayers))
                {
                    if (collider != null && collider.gameObject != null && !IsAllowedCollider(collider))
                    {
                        inc = true;

                        if (collider.bounds.Contains(camerapos))
                        {
                            PTC("Collider Viewing & attempted to correct.");
                            ColliderViewAttempts += 1;
                            
                            Vector3 movement = (camerapos - collider.ClosestPoint(camerapos)).normalized;

                            if (Time.time - lt >= 0.5f)
                            {
                                Vector3 safe = collider.ClosestPoint(camerapos) + movement * 2f;

                                bool issafe = true;
                                Collider[] near = Physics.OverlapSphere(safe, 0.15f, Physics.AllLayers);
                                for (int i = 0; i < near.Length; i++)
                                {
                                    if (!IsAllowedCollider(near[i]))
                                    {
                                        issafe = false;
                                        break;
                                    }
                                }

                                if (issafe)
                                {
                                    localPlayer.TeleportTo(safe, localPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                                }
                                else
                                {
                                    localPlayer.TeleportTo(lastknowngood, localPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                                }

                                lt = Time.time;
                            }
                            else
                            {
                                localPlayer.SetVelocity(movement * 2f);
                            }

                            iic = true;
                            break;
                        }
                        else
                        {
                            localPlayer.SetVelocity((camerapos - collider.ClosestPoint(camerapos)).normalized * 2f);
                        }
                    }
                }

                if (!iic && inc)
                {
                    if (Time.time - lt >= 0.5f)
                    {
                        lastknowngood = localPlayer.GetPosition();
                        lt = Time.time;
                    }
                }
                else if (!inc)
                {
                    lastknowngood = localPlayer.GetPosition();
                    lt = Time.time;
                }
            }

            if (autoMaxOVRHeight)
            {
                maxOVRAdvancedHeight = (localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.5f) * 1.4f;
                middlepoint = new Vector3(0f, (localPlayer.GetAvatarEyeHeightMaximumAsMeters() * 0.5f), 0f);
            }

            if (!allowOVRAdvanced && Vector3.Distance(localPlayer.GetPosition() + middlepoint, camerapos) >= maxOVRAdvancedHeight)
            {
                PTC("OVR/ Gogo Loco");
                OVR_GoGoLocoAttempts += 1;
                Detected();
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckStuff), 0f);
    }

    private bool IsAllowedCollider(Collider collider)
    {
        Transform current = collider.transform;
        while (current != null)
        {
            if (collider.transform.GetComponent<VRC.SDK3.Components.VRCStation>() != null)
            {
                return true;
            }
            current = current.parent;
        }

        string colliderName = collider.gameObject.name.ToLower().Trim();
        foreach (string allowedName in funnycolliders)
        {
            if (colliderName.StartsWith(allowedName))
                return true;
        }

        if (autoIgnorePickupables)
        {
            VRC.SDK3.Components.VRCPickup pickup = collider.GetComponent<VRC.SDK3.Components.VRCPickup>();
            if (pickup == null && collider.transform.parent != null)
                pickup = collider.transform.parent.GetComponentInParent<VRC.SDK3.Components.VRCPickup>();
            if (pickup != null)
                return true;
        }

        return false;
    }

    private bool AC() => Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection && !isTeleporting && antiCheat;

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
        }

        if (localPlayer.GetVelocity().magnitude < localPlayer.GetRunSpeed() + 3.75f)
        {
            if (Time.time - ct >= 1.0f)
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
            ct = Time.time;
            SendCustomEventDelayedSeconds(nameof(resetvelo), 0.05f);
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
            RespawnAttempts += 1;
        }
    }

    public void SetPlayerVelocity(Vector3 velo)
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

    public void TeleportPlayer(Vector3 pos, Quaternion rot, VRC_SceneDescriptor.SpawnOrientation sori, bool smooth)
    {
        localPlayer.TeleportTo(pos, rot, sori, smooth);
        spt = math.INFINITY;
        isTeleporting = true;
        SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
    }

    public void Seat()
    {
        PTC("VRCStation");
        SeatAttempts += 1;
        spt = math.INFINITY;
        isTeleporting = true;
        SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
    }
}
