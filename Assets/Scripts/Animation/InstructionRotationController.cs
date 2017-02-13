using UnityEngine;

[ExecuteInEditMode]
public class InstructionRotationController : MonoBehaviour
{
    public Transform controller;
    public Transform drone;

    public float rotationX = 0f;
    public float rotationY = 0f;
    public float rotationZ = 0f;

    public bool isControlling = true;

    void Update()
    {
        if (isControlling)
        {
            Quaternion r = Quaternion.Euler(rotationX, rotationY, rotationZ);

            controller.localRotation = r;
            drone.localRotation = r;
        }
    }
}
