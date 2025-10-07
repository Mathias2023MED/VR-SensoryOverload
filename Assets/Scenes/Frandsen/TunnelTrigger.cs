using UnityEngine;

public class TunnelTrigger : MonoBehaviour
{
    public TunnelVisionUI tunnelVision;  // drag in the TunnelVisionUI script in Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tunnelVision.StartTunnelVision();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            tunnelVision.ResetTunnelVision();
        }
    }
}
