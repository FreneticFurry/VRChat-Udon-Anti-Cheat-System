// VRChat anti cheat system by: FreneticFurry!
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
public class FreneticAntiCheat : UdonSharpBehaviour
{
    // thanks to Zerithax for telling me about tooltips :)
    [Tooltip("This is the 'respawn' point for when someone gets detected as a cheater")]
    public Vector3 detectionPoint;

    [Tooltip("Similar to Minecraft's Spawn protection")]
    public float detectionProtection = 2;

    [Tooltip("Maximum allowed speed!, this should be higher then the VRCWorld RunSpeed!")]
    public float maxSpeed = 6.4f;

    [Tooltip("Maximum allowed height for OVR Advanced")]
    public float maxOVRAdvancedHeight = 0.9f;

    [Tooltip("Automatically set the maximum speed based on VRCWorld RunSpeed (may not work well at some speeds)")]
    public bool autoMaxSpeed = false;

    [Tooltip("Automatically set the Maximum height someone can go before being detected as a cheater")]
    public bool autoMaxOVRHeight = false;

    private float maxJumpHeight;
    private Vector3 previousPosition;
    private Vector3 velocity;
    private float timer = 0f;
    private Vector3 middlepoint = new Vector3(0f, 0.6625f, 0f); // this is for the height of 1.3 so change it accordingly to match the middle of the height!
    // attempted calculation (may not work for all speeds or heights so you may want to turn it off.)
    private void Start()
    {
        if (autoMaxSpeed == true)
        {
            maxSpeed = Networking.LocalPlayer.GetRunSpeed() * 2.75f;
        }
        if (autoMaxOVRHeight == true)
        {
            maxOVRAdvancedHeight = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters() * 0.7f;
            middlepoint = new Vector3(0f, Networking.LocalPlayer.GetAvatarEyeHeightAsMeters() * 0.51f, 0f);
        }
    }

    // Anti Cheat System
    private void Update()
    {
        // Variables

        Vector3 localPlayerCameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        // Detection Protection

        if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + middlepoint, detectionPoint) >= detectionProtection)
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

                if (Networking.LocalPlayer.GetVelocity().y > Networking.LocalPlayer.GetJumpImpulse() * 1.075) // this detects flying "most" of the time but players can still get around it by going slowly so i suggest using colliders to limit areas to prevent them from getting into places they shouldn't.
                {
                    Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    Networking.LocalPlayer.SetVelocity(Vector3.zero);
                }

                previousPosition = localPlayerCameraPosition;
            }

            // Collider detection (this likely wont affect desktop players but in some cases can still affect them such as if you make bounding boxes/ triggers)

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
                            case "bounding box": // excluded colliders so you can still use triggers or make triggers with colliders
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
            velocity = new Vector3(0, 0, 0); // resets velocity after being detected so it wont falsely trigger ounce outside of the detection protection properly
            previousPosition = localPlayerCameraPosition;
        }
    }
}
