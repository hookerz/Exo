using UnityEngine;

public class DroneLocator : MonoBehaviour
{
    public Transform player;
    public MeshRenderer droneMeshRenderer;
    public GameObject arrowObject;
    public Camera mainCamera;

    public float angleOffset = 25f;
    public bool on = true;
    public bool hideArrowWhenDroneIsVisible = true;
    public float distanceFromCenter = 0.35f;

    private Vector3 droneProjectedPosition;

    void Update()
    {
        if (droneMeshRenderer == null)
            return;

        bool showArrow = !hideArrowWhenDroneIsVisible || !droneMeshRenderer.isVisible;
        arrowObject.SetActive(showArrow);

        if (showArrow)
        {
            droneProjectedPosition = Vector3.ProjectOnPlane(player.position, Camera.main.transform.forward);
            Vector3 cameraToDrone = droneProjectedPosition - Camera.main.transform.position;
            arrowObject.transform.rotation = Quaternion.LookRotation(cameraToDrone) * Quaternion.Euler(90, 0, 0);
            Vector3 diff = droneProjectedPosition - Camera.main.transform.position;
            arrowObject.transform.position = Camera.main.transform.position + diff.normalized * distanceFromCenter + Camera.main.transform.forward * 0.7f;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(droneProjectedPosition, 0.25f);
        if(Camera.main != null)
            Gizmos.DrawLine(Camera.main.transform.position, droneProjectedPosition);
    }
}
