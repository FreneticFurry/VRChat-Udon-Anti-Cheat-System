// VRChat anti cheat system by: FreneticFurry!
using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
public class FreneticAntiCheat : UdonSharpBehaviour
{
    public Vector3 detectionPoint; // this is the "respawn" point for when someone gets detected as a cheater
    public float detectionProtection = 5; // alot like Minecrafts Spawn protection
    public float maxSpeed = 5.5f; // maximum allowed speed! VRChat default runspeed is 4 so i ahve it at 5.5 for vr users because sometimes they may move abit irl & arnt trying to cheat (recommended: test this inside the vrchat application because the ClientSim player is different from that actual vrchat player!)
    public float maxOVRAdvancedHeight = 0.9f;
    private Vector3 previousPosition;
    // Anti Cheat System
    private void Update()
    {
        {
            // Variables

            Vector3 localPlayerCameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Vector3 velocity;

            // Detection Protection

            if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + new Vector3(0f, 0.6625f, 0f), detectionPoint) >= detectionProtection)
            {

                //Speed Detection

                float timer = 0f;
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

                    if (Networking.LocalPlayer.GetVelocity().y > Networking.LocalPlayer.GetJumpImpulse()*1.5) // this detects flying "most" of the time but players can still get around it by going slowly so i suggest using colliders to limit areas to prevent them from getting into places they shouldn't.
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
                                case "Player": case "player": case "Bounding Box": case "bounding box": // excluded colliders so you can still use triggers or make triggers with colliders
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

                if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + new Vector3(0f, 0.6625f, 0f), localPlayerCameraPosition) >= maxOVRAdvancedHeight)
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
}
