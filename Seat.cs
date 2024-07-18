
using UdonSharp;
using VRC.SDKBase;

public class Seat : UdonSharpBehaviour
{
    public FreneticAntiCheat antiCheat;

    // would make this do something in Start() but i dont know of a method to get the AntiCheat if its found in the scene so you'll just have to manually do it :)

    public override void OnStationEntered(VRCPlayerApi player)
    {
        antiCheat.Seat();
    }

    public override void OnStationExited(VRCPlayerApi player)
    {
        antiCheat.Seat();
    }
}
