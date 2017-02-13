using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    public Vector3 offset = new Vector3(90, 0, 0);

    void Start()
    {
        Vector3 toCamera = Camera.main.transform.position - this.transform.position;
        transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up) * Quaternion.Euler(offset);
    }
}
