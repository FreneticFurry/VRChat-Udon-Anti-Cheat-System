using System.Threading;
using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FreneticAntiCheat : UdonSharpBehaviour
{
    [Header("Anti Objects")]
    public Transform antiMirror_Camera;
    public GameObject antiBlocked_Player;
    public GameObject blackoutObj;

    [Header("Detection")]
    public Vector3 detectionPoint; [Space]
    [Space]
    public bool enableSpawnPoint = false;
    public GameObject spawnPointLocation;
    public float detectionProtectionRadius = 2;
    [Header("Colliders")]
    public string[] allowedColliderNames = new string[0];
    public LayerMask allowedLayers;

    [Header("Out Of Bounds")]
    public Collider[] inBounds;

    [Header("Anti Cheat")]
    public bool antiCheat = true;
    public bool isTeleporting = false;

    [Header("Settings")]
    public bool allowBhopping = true;
    public bool allowLongArms, allowFlight, ragdollSupport, allowOVRAdvanced, allowColliderView, allowSpeedManipulation, allowBlockInvis, AllowPersonalMirrors_Cameras, noColliderBlackout, disableBounds, noPickupVerification = false;
    public bool printDetection = true;

    [HideInInspector] public int LongArmAttempts, FlightAttempts, OVR_GoGoLocoAttempts, ColliderViewAttempts, SpeedManipulationAttempts, SeatAttempts, RespawnAttempts, OutOfBoundsAttempts;

    private float maxSpeed, jumpspeed = 3f, runspeed = 2f, ft, ic, ct;
    private Vector3 previousPosition, velocity, middlepoint, camerapos, lastknowngood, flightp;
    private VRCPlayerApi localPlayer;
    private Vector3[] funnytable = new Vector3[10];
    private int CS, AIS;
    private bool velocityChanged, ptcl;

    // Startup \\

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        previousPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        if (enableSpawnPoint && spawnPointLocation != null) detectionPoint = spawnPointLocation.transform.position;
        SendCustomEventDelayedSeconds(nameof(CheckStuff), 0.5f);
    }

    // AntiCheat Main \\

    void FixedUpdate()
    {
        camerapos = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Vector3 currentVelocity = (camerapos - previousPosition) / Time.fixedDeltaTime;
        funnytable[CS] = currentVelocity;
        CS = (CS + 1) % funnytable.Length;
        velocity = Vector3.zero;
        for (int i = 0; i < funnytable.Length; i++) velocity += funnytable[i];
        velocity /= funnytable.Length;
        previousPosition = camerapos;
        Vector3 playerVelocity = localPlayer.GetVelocity();
        maxSpeed = (localPlayer.GetAvatarEyeHeightAsMeters() < 0.83f ? 0.83f : localPlayer.GetAvatarEyeHeightAsMeters()) / 2 + localPlayer.GetRunSpeed() + (allowBhopping && (Mathf.Abs(playerVelocity.y) > 0.25f) ? jumpspeed : runspeed);
        middlepoint = new Vector3(0f, localPlayer.GetAvatarEyeHeightAsMeters() * 0.5f, 0f);

        if (AC() && !allowColliderView)
        {
            bool inc = false;
            Vector3 c = Vector3.zero;

            Collider[] nearbyColliders = Physics.OverlapSphere(localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, 0.075f, (~allowedLayers | ~LayerMask.GetMask("Walkthrough", "Pickup", "Player", "PlayerLocal", "UI", "InternalUI", "HardwareObjects", "UiMenu", "Water")));

            foreach (Collider collider in nearbyColliders)
            {
                if (collider == null) continue;

                bool isInBounds = false;
                foreach (Collider boundCollider in inBounds)
                {
                    if (collider == boundCollider)
                    {
                        isInBounds = true;
                        break;
                    }
                }

                bool isFunnyCollider = false;
                foreach (string funnyColliderName in allowedColliderNames)
                {
                    if (collider.gameObject.name.Equals(funnyColliderName))
                    {
                        isFunnyCollider = true;
                        break;
                    }
                }

                if (isInBounds || isFunnyCollider) continue;

                foreach (Collider targetCollider in nearbyColliders)
                {
                    if (targetCollider != null && collider == targetCollider)
                    {
                        inc = true;
                        c = collider.ClosestPoint(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position);
                        break;
                    }
                }

                if (inc) break;
            }

            if (inc)
            {
                SetPlayerVelocity((Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - c).normalized * 2);

                ic += Time.deltaTime;

                if (ic >= 0.2)
                {
                    TeleportPlayer(lastknowngood, Networking.LocalPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    ic = 0f;
                }
            }
            else
            {
                lastknowngood = Networking.LocalPlayer.GetPosition();
                ic = 0f;
            }
        }
    }

    public void CheckStuff()
    {
        if (AC())
        {
            if (!allowSpeedManipulation && new Vector3(velocity.x, 0f, velocity.z).magnitude > maxSpeed)
            {
                SpeedManipulationAttempts += 1;

                PTC("Speed Manipulation", 1, true, SpeedManipulationAttempts);
                Detected();
            }

            if (!allowFlight && !ragdollSupport)
            {
                Collider[] colliders = Physics.OverlapSphere(localPlayer.GetPosition(), 0.02f, LayerMask.GetMask("PlayerLocal"));
                foreach (Collider collider in colliders)
                {
                    if (collider == null || collider.gameObject == null || !collider.gameObject.activeInHierarchy)
                    {
                        FlightAttempts += 1;
                        PTC("Avatar Collider", 1, true, FlightAttempts);
                        Detected();
                        break;
                    }
                }
            }
            else if (!allowFlight && ragdollSupport)
            {
                bool touching = false;

                Collider[] colliders = Physics.OverlapCapsule(localPlayer.GetPosition() + new Vector3(0, 0.05f, 0), localPlayer.GetPosition() - new Vector3(0, localPlayer.GetJumpImpulse() * 0.115f + 0.05f, 0), 0.2f, ~LayerMask.GetMask("Walkthrough", "Pickup", "Player", "PlayerLocal", "UI", "InternalUI", "HardwareObjects", "UiMenu", "Water", "MirrorReflection", "PickupNoEnvironment", "Interactive", "TransparentFX"));
                foreach (Collider collider in colliders)
                {
                    if (collider != null)
                    {
                        touching = true;
                    }
                }
                if (!touching)
                {
                    if (Vector3.Distance(flightp, localPlayer.GetPosition()) > 0.2)
                    {
                        flightp = localPlayer.GetPosition();
                        ft = 0f;
                    }
                    else
                    {
                        ft += Time.deltaTime;

                        if (ft >= 0.25)
                        {
                            FlightAttempts++;
                            ft = 0f;
                            PTC("Flying", 1, true, FlightAttempts);
                            Detected();
                        }
                    }
                }
                else
                {
                    ft = 0f;
                }
            }

            if (antiMirror_Camera != null)
            {
                if (!AllowPersonalMirrors_Cameras)
                {

                    if (Vector3.Distance(antiMirror_Camera.transform.position, camerapos) > 0.25f)
                    {
                        antiMirror_Camera.transform.position = camerapos;
                        antiMirror_Camera.transform.rotation = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
                    }

                    if (!antiMirror_Camera.gameObject.activeSelf)
                    {
                        antiMirror_Camera.gameObject.SetActive(true);
                    }
                }

                else
                {
                    if (antiMirror_Camera.gameObject.activeSelf)
                    {
                        antiMirror_Camera.gameObject.SetActive(false);
                    }
                }
            }

            if (!allowLongArms)
            {
                Vector3 mid = localPlayer.GetPosition() + middlepoint;
                float dist = Mathf.Max(localPlayer.GetAvatarEyeHeightAsMeters() * 0.81f, 0.7f);
                if (Vector3.Distance(mid, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position) > dist || Vector3.Distance(mid, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position) > dist)
                {
                    LongArmAttempts += 1;

                    PTC("Long Arms", 1, true, LongArmAttempts);
                    Detected();
                }
            }

            if (!noColliderBlackout && blackoutObj != null)
            {
                if (Vector3.Distance(blackoutObj.transform.position, camerapos) > 0.25f)
                {
                    blackoutObj.transform.position = camerapos;
                    blackoutObj.transform.rotation = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
                }

                if (!blackoutObj.gameObject.activeSelf)
                {
                    blackoutObj.gameObject.SetActive(true);
                }

                Vector3 headPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
                bool isInsideCollider = false;

                foreach (Collider collider in Physics.OverlapSphere(camerapos, 0.05f, Physics.AllLayers))
                {
                    if (collider == null || collider.gameObject == null) continue;

                    bool allowedCollider = false;

                    if (((1 << collider.gameObject.layer) & (allowedLayers | LayerMask.GetMask("Walkthrough", "Pickup", "Player", "PlayerLocal", "UI", "InternalUI", "HardwareObjects", "UiMenu", "Water"))) != 0)
                    {
                        allowedCollider = true;
                    }

                    if (!allowedCollider)
                    {
                        Transform current = collider.transform;
                        while (current != null)
                        {
                            if (current.GetComponent<VRC.SDKBase.VRCStation>() != null || current.GetComponent<VRC.SDKBase.VRC_PortalMarker>() != null)
                            {
                                allowedCollider = true;
                                break;
                            }
                            current = current.parent;
                        }
                    }

                    if (!allowedCollider)
                    {
                        foreach (string allowedName in allowedColliderNames)
                        {
                            if (collider.gameObject.name.ToLower().Trim().StartsWith(allowedName.ToLower().Trim()))
                            {
                                allowedCollider = true;
                                break;
                            }
                        }
                    }

                    if (allowedCollider) continue;

                    bool bounds = false;

                    if (!disableBounds)
                    {
                        for (int i = 0; i < inBounds.Length; i++)
                        {
                            if (inBounds[i] == collider)
                            {
                                bounds = true;
                                break;
                            }
                        }
                    }

                    if (disableBounds || (!bounds && collider.bounds.Contains(camerapos)))
                    {
                        isInsideCollider = true;
                        break;
                    }
                }

                blackoutObj.GetComponent<Renderer>().material.SetInt("_Blackout", isInsideCollider ? 1 : 0);
            }
            else
            {
                if (blackoutObj.gameObject.activeSelf && blackoutObj != null)
                {
                    blackoutObj.gameObject.SetActive(false);
                }
            }

            if (!allowOVRAdvanced && Vector3.Distance(localPlayer.GetPosition() + middlepoint, camerapos) >= localPlayer.GetAvatarEyeHeightAsMeters() * 0.75f)
            {
                OVR_GoGoLocoAttempts += 1;
                PTC("OVR/ Gogo Loco", 1, true, OVR_GoGoLocoAttempts);
                Detected();
            }

            if (!disableBounds && inBounds != null)
            {
                bool notnone = false;
                bool isInBounds = false;

                for (int i = 0; i < inBounds.Length; i++)
                {
                    Collider collider = inBounds[i];
                    if (collider != null)
                    {
                        notnone = true;
                        if (collider.bounds.Contains(localPlayer.GetPosition() + middlepoint))
                        {
                            isInBounds = true;
                            break;
                        }
                    }
                }

                if (notnone && !isInBounds)
                {
                    OutOfBoundsAttempts += 1;
                    PTC("Out of bounds", 1, true, OutOfBoundsAttempts);
                    localPlayer.Respawn();
                }
            }

            if (!noPickupVerification)
            {
                foreach (Collider collider in Physics.OverlapSphere(localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, 50f))
                {
                    if (collider == null) continue;

                    VRC_Pickup pickup = collider.GetComponent<VRC_Pickup>();
                    if (pickup != null)
                    {
                        if (IsHandClear(pickup.GetComponent<Collider>(), true, (allowedLayers | LayerMask.GetMask("Walkthrough", "Player", "PlayerLocal", "UI", "InternalUI", "HardwareObjects", "UiMenu", "Water")) & ~(1 << LayerMask.NameToLayer("Pickup")), inBounds) && IsHandClear(pickup.GetComponent<Collider>(), false, (allowedLayers | LayerMask.GetMask("Walkthrough", "Player", "PlayerLocal", "UI", "InternalUI", "HardwareObjects", "UiMenu", "Water")) & ~(1 << LayerMask.NameToLayer("Pickup")), inBounds))
                        {
                            pickup.GetComponent<VRC_Pickup>().pickupable = true;
                        }
                        else
                        {
                            pickup.GetComponent<VRC_Pickup>().pickupable = false;
                        }
                    }
                }
            }
        }
        else if (!antiCheat && antiMirror_Camera != null)
        {
            antiMirror_Camera.transform.gameObject.SetActive(false);
            blackoutObj.gameObject.SetActive(false);
        }

        if (!allowBlockInvis && antiBlocked_Player != null)
        {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);

            foreach (VRCPlayerApi player in players)
            {
                if (player == localPlayer) continue;

                if (player != null && !player.isLocal && antiBlocked_Player != null)
                {
                    bool playerExists = false;

                    foreach (Transform child in antiBlocked_Player.transform.parent)
                    {
                        if (child.name == player.displayName)
                        {
                            playerExists = true;
                            break;
                        }
                    }

                    if (!playerExists)
                    {
                        GameObject BPlayerC = Instantiate(antiBlocked_Player, antiBlocked_Player.transform.parent);
                        BPlayerC.name = player.displayName;

                        if (BPlayerC.transform.Find("UsernameC").Find("UsernameT").GetComponent<TextMeshProUGUI>() != null) BPlayerC.transform.Find("UsernameC").Find("UsernameT").GetComponent<TextMeshProUGUI>().text = player.displayName;
                    }
                }

                Transform BPlayer = antiBlocked_Player.transform.parent.Find(player.displayName);

                if (BPlayer != null && player != null)
                {
                    BPlayer.Find("UsernameC").transform.rotation = Quaternion.LookRotation(new Vector3((localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - BPlayer.Find("UsernameC").transform.position).x, (localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - BPlayer.Find("UsernameC").transform.position).y, (localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - BPlayer.Find("UsernameC").transform.position).z));
                    BPlayer.position = player.GetPosition();
                    BPlayer.rotation = player.GetRotation();
                    BPlayer.transform.localScale = new Vector3(player.GetAvatarEyeHeightAsMeters() * 1.183f, player.GetAvatarEyeHeightAsMeters() * 1.183f, player.GetAvatarEyeHeightAsMeters() * 1.183f);
                    BPlayer.Find("UsernameC").transform.localScale = Vector3.Lerp(new Vector3(0.02f, 0.006f, 0.02f), new Vector3(0.045f, 0.014f, 0.045f), Mathf.InverseLerp(2f, 3.75f, Vector3.Distance(BPlayer.Find("UsernameC").transform.position, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position)));

                    bool blocked = false;

                    RaycastHit[] hits = Physics.RaycastAll(player.GetPosition(), Vector3.up, player.GetAvatarEyeHeightAsMeters() * 7.5f);

                    foreach (RaycastHit hit in hits)
                    {
                        Collider collider = hit.collider;
                        if (collider == null || collider.gameObject == null || !collider.gameObject.activeInHierarchy)
                        {
                            blocked = true;
                            break;
                        }
                    }

                    BPlayer.gameObject.SetActive(!blocked);
                }
            }
        }

        SendCustomEventDelayedSeconds(nameof(CheckStuff), 0f);
    }

    // Helper Functions \\

    private bool AC() => Vector3.Distance(localPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtectionRadius && !isTeleporting && antiCheat;

    private void PTC(string item, int state, bool counter, int count)
    {
        if (printDetection && !ptcl)
        {
            string print;
            switch (state)
            {
                case 1:
                    {
                        print = $"[<color=#007d0e>Frenetic Anti Cheat</color>]: [<color=#aa0104>{item}</color>] has been detected!";
                        break;
                    }
                case 2:
                    {
                        print = $"[<color=#007d0e>Frenetic Anti Cheat</color>]: used [<color=#0caa01>{item}</color>]!";
                        break;
                    }
                case 3:
                    {
                        print = $"[<color=#007d0e>Frenetic Anti Cheat</color>]: [<color=#ffffff>{item}</color>] was done!";
                        break;
                    }
                case 4:
                    {
                        print = $"[<color=#007d0e>Frenetic Anti Cheat</color>]: [<color=#08c999>{item}</color>] was used!";
                        break;
                    }
                default:
                    {
                        print = $"[<color=#007d0e>Frenetic Anti Cheat</color>]: Unknown {item} did you properly use a valid State?";
                        break;
                    }
            }

            if (counter) Debug.Log($"{print} {count} times!");
            else Debug.Log(print);

            ptcl = true;
            SendCustomEventDelayedSeconds(nameof(PTCR), 0.01f);
        }
    }

    public void PTCR()
    {
        ptcl = false;
    }

    public void Detected()
    {
        localPlayer.TeleportTo(detectionPoint, Quaternion.identity, VRC_SceneDescriptor.SpawnOrientation.Default, false);
        localPlayer.SetVelocity(Vector3.zero);
    }

    public void resetvelo()
    {
        if (!velocityChanged)
        {
            velocityChanged = true;
            ct = Time.time;
        }

        if (localPlayer.GetVelocity().magnitude < localPlayer.GetRunSpeed() + 3f)
        {
            if (Time.time - ct >= 1.0f)
            {
                jumpspeed = 3f;
                runspeed = 2f;

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
        isTeleporting = false;
    }

    public bool IsHandClear(Collider targetCollider, bool isLeftHand, LayerMask ignoredLayers, Collider[] ignoredColliders)
    {
        Vector3 originPosition = new Vector3(localPlayer.GetPosition().x, (isLeftHand ? localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position : localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position).y, localPlayer.GetPosition().z);

        Debug.DrawLine(originPosition, targetCollider.transform.position, isLeftHand ? Color.blue : Color.green);

        if (!Physics.Raycast(originPosition, (targetCollider.transform.position - originPosition).normalized, out RaycastHit hit, 50f, ~ignoredLayers) || hit.collider == targetCollider) return true;

        if (hit.collider != null)
        {
            foreach (Collider ignoredCollider in ignoredColliders)
            {
                if (hit.collider == ignoredCollider)
                    return true;
            }

            if (hit.collider.GetComponent<VRC.SDKBase.VRCStation>() != null || hit.collider.GetComponent<VRC.SDKBase.VRC_PortalMarker>() != null)
            {
                return true;
            }
        }

        return false;
    }

    // Overrides \\

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RespawnAttempts += 1;
            PTC("Respawning", 3, true, RespawnAttempts);
            isTeleporting = true;
            localPlayer.SetVelocity(Vector3.zero);
            SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (player != null && !player.isLocal && antiBlocked_Player != null)
        {
            Transform BPlayer = antiBlocked_Player.transform.parent.Find(player.displayName);
            if (BPlayer != null) DestroyImmediate(BPlayer.gameObject);
        }
    }

    // Remade Functions \\

    public void SetPlayerVelocity(Vector3 velo)
    {
        resetvelo();

        if (allowBhopping)
        {
            jumpspeed = velo.magnitude + 3f;
            runspeed = velo.magnitude + 2f;
        }
        else
        {
            runspeed = velo.magnitude + 2f;
        }
        PTC("SetPlayerVelocity", 4, false, 0);
        localPlayer.SetVelocity(velo);
    }

    public void TeleportPlayer(Vector3 pos, Quaternion rot, VRC_SceneDescriptor.SpawnOrientation sori, bool smooth)
    {
        localPlayer.TeleportTo(pos, rot, sori, smooth);

        isTeleporting = true;
        PTC("TeleportPlayer", 4, false, 0);
        SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
    }

    public void Seat()
    {
        SeatAttempts += 1;
        PTC("VRCStation", 2, true, SeatAttempts);
        isTeleporting = true;
        SendCustomEventDelayedSeconds(nameof(Teleported), 0.25f);
    }
}
