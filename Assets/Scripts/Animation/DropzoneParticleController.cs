using UnityEngine;

public class DropzoneParticleController : MonoBehaviour
{
    public Transform system;
    public float rotationSpeed = 0.5f;

    void Update()
    {
        Vector3 r = system.localEulerAngles;

        r.y += rotationSpeed;

        system.localEulerAngles = r;
    }
}
