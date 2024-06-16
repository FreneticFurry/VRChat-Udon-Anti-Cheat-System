using Unity.Mathematics;
// VRChat anti cheat system by: FreneticFurry!
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class FreneticAntiCheat : UdonSharpBehaviour
{
    // thanks to Zerithax for telling me about tooltips :) (i had initially thought it was only for shaders)
    [Header("Detection")]
    [Space]
    [Tooltip("This is the 'respawn' point for when someone gets detected as a cheater")]
    public Vector3 detectionPoint;

    [Tooltip("Similar to Minecraft's Spawn protection")]
    public float detectionProtection = 2;

    [Header("Speed")]
    [Space]
    [Tooltip("Maximum allowed speed! this should be higher than the VRCWorld RunSpeed!")]
    public float maxSpeed = 6.5f;

    [Header("Height")]
    [Space]
    [Tooltip("Maximum allowed height")]
    public float maxOVRAdvancedHeight = 0.9f;

    [Header("Flying")]
    [Space]
    [Tooltip("Radius from the ground before considering if the player is flying/ cheating")]
    public float FlyingDistThreshold = 0.5f;

    [Tooltip("Time required to be considered flying/ cheating in the air")]
    public float flyTime = 0.25f;

    [Header("Automatic Settings")]
    [Space]
    [Tooltip("Automatically sets the maximum speed based on VRCWorld RunSpeed (may not work well at some speeds)")]
    public bool autoMaxSpeed = false;

    [Tooltip("Automatically sets the Maximum height someone can go before being detected as a cheater")]
    public bool autoMaxOVRHeight = false;

    [Header("Anti Cheat")]
    [Space]
    [Tooltip("Enables or disables the anti cheat system")]
    public bool antiCheat = false;

    private float ftimer = 0f;
    private float gTimer = 0f;
    private Vector3 previousPosition;
    private Vector3 velocity;
    private float timer = 0f;
    private Vector3 middlepoint = new Vector3(0f, 0.6625f, 0f); // this is for the height of 1.3 so change it accordingly to match the middle of the height!
    private bool IsRespawning = false;

    // attempted calculation (may not work for all speeds or heights so you may want to turn it off.)
    private void Start()
    {
        if (autoMaxSpeed == true)
        {
            maxSpeed = Networking.LocalPlayer.GetRunSpeed() * 1.625f;
        }
        if (autoMaxOVRHeight == true)
        {
            maxOVRAdvancedHeight = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters() * 0.7f;
            middlepoint = new Vector3(0f, Networking.LocalPlayer.GetAvatarEyeHeightAsMeters() * 0.51f, 0f);
        }
    }

    // Respawning

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            IsRespawning = true;
            SendCustomEventDelayedSeconds("Respawned", 0.5f); // thanks to Zerithax for also suggesting to do this (would've done yet another wait in a loop probably)
        }
    }

    public void Respawned()
    {
        IsRespawning = false;
    }

    // Anti Cheat System
    private void Update()
    {
        // Variables
        Vector3 localPlayerCameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        // Detection Protection
        if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection || IsRespawning == false || antiCheat == false)
        {
            //Speed Detection
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

                if (Networking.LocalPlayer.GetVelocity().y > Networking.LocalPlayer.GetJumpImpulse())
                {
                    Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    Networking.LocalPlayer.SetVelocity(Vector3.zero);
                }

                previousPosition = localPlayerCameraPosition;
            }

            // Flying detection

            if (velocity.y > 0.1f)
            {
                ftimer += Time.deltaTime;
                gTimer = 0f;

                RaycastHit hit;
                if (Physics.Raycast(Networking.LocalPlayer.GetPosition() + new Vector3(0, -0.5f, 0), Vector3.down, out hit, math.INFINITY))
                {
                    if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + new Vector3(0, -0.5f, 0), hit.point) > FlyingDistThreshold)
                    {
                        Debug.DrawLine(Networking.LocalPlayer.GetPosition() + new Vector3(0, -0.5f, 0), hit.point, Color.red); // Debug for Unity!

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

            // Collider detection (this likely won't affect desktop players but in some cases can still affect them such as if you make bounding boxes/triggers)
            Collider[] colliders = Physics.OverlapSphere(localPlayerCameraPosition, 0.1f);

            foreach (Collider collider in colliders)
            {
                if (collider != null)
                {
                    if (Vector3.Distance(localPlayerCameraPosition, collider.ClosestPointOnBounds(localPlayerCameraPosition)) <= 0)
                    {
                        switch (collider.gameObject.name)
                        {
                            case "Player":
                            case "player":
                            case "Bounding Box":
                            case "bounding box": // Excluded colliders so you can still use triggers or make triggers with colliders
                                break;
                            default:
                                Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                                Networking.LocalPlayer.SetVelocity(Vector3.zero);
                                break;
                        }
                    }
                }
            }

            // Distance detection (prevention for OVR advanced abuse :D )
            if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + middlepoint, localPlayerCameraPosition) >= maxOVRAdvancedHeight)
            {
                Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                Networking.LocalPlayer.SetVelocity(Vector3.zero);
            }
        }
        else
        {
            velocity = new Vector3(0, 0, 0); // Resets velocity after being detected so it won't falsely trigger once outside of the detection protection properly
            previousPosition = localPlayerCameraPosition;
        }
    }
}
