// VRChat anti cheat system by: FreneticFurry!
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
public class FreneticAntiCheat : UdonSharpBehaviour
{
    public Vector3 detectionPoint; // this is the "respawn" point for when someone gets detected as a cheater
    public float detectionProtection = 5; // alot like Minecrafts Spawn protection
    public float maxSpeed = 5.5f; // maximum allowed speed! VRChat default runspeed is 4 so i ahve it at 5.5 for vr users because sometimes they may move abit irl & arnt trying to cheat (recommended: test this inside the vrchat application because the ClientSim player is different from that actual vrchat player!)
    public float maxOVRAdvancedHeight = 0.9f;

    // Anti Cheat System
    private void LateUpdate() // can be changed to Update or FixedUpdate but it wouldn't really make a difference afaik so i suggest leaving it
    {
        {
            if (Vector3.Distance(Networking.LocalPlayer.GetPosition() + new Vector3(0f, 0.6625f, 0f), detectionPoint) >= detectionProtection) // protection so it doesnt constantly go off ounce a player has already been detected for attempting to cheat (like using a collider to fly around or menu bugging)
            {
                // Speed Detection (this is gonna affect everyone! prevents players from doing real cheaty things :)  )

                Vector3 localPlayerCameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

                if (Networking.LocalPlayer.GetVelocity().x >= maxSpeed || Networking.LocalPlayer.GetVelocity().x <= -maxSpeed)
                {
                    Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    Networking.LocalPlayer.SetVelocity(Vector3.zero);
                }
                if (Networking.LocalPlayer.GetVelocity().z >= maxSpeed || Networking.LocalPlayer.GetVelocity().z <= -maxSpeed)
                {
                    Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                    Networking.LocalPlayer.SetVelocity(Vector3.zero);
                }

                // Collider detection (this likely wont affect desktop players but in some cases can still affect them such as if you make bounding boxes)

                Collider[] colliders = Physics.OverlapSphere(localPlayerCameraPosition, 0.1f);

                foreach (Collider collider in colliders)
                {
                    if (collider != null)
                    {
                        if (Vector3.Distance(localPlayerCameraPosition, collider.ClosestPointOnBounds(localPlayerCameraPosition)) <= 0)
                        {
                            if (collider.gameObject.name == "BoundingBox" || collider.gameObject.name == "Bounding Box") // this is the exclusion area where you can exclude things that you may want to use something for other things instead
                            {
                            }
                            else
                            {
                                Networking.LocalPlayer.TeleportTo(detectionPoint, new Quaternion(0, 0, 0, 0), VRC_SceneDescriptor.SpawnOrientation.Default, false);
                                Networking.LocalPlayer.SetVelocity(Vector3.zero);
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
        }
    }
}
