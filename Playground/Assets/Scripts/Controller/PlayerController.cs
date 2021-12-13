using Cinemachine;
using Mirror;

namespace Playground
{
    public class PlayerController : NetworkBehaviour
    {
        private void Start()
        {
            if (isLocalPlayer)
            {
                var cam = FindObjectOfType<CinemachineVirtualCamera>();
                cam.Follow = transform;
            }
        }
    }
}
