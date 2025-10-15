using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Transform teleportTarget;
    public Transform player;
    public Transform cameraTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && teleportTarget != null)
        {
            player.position = teleportTarget.position;

            if (cameraTarget != null && Camera.main != null)
            {
                Camera.main.transform.position = cameraTarget.position;
            }
        }
    }
}
