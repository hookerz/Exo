using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public enum Axis
    {
        x, y, z
    }

    public float speed = 45; // degrees per second
    public Axis axis = Axis.y;
    private Vector3 currentEuler = Vector3.zero;

    void Update()
    {
        if (axis == Axis.x)
            currentEuler.x += speed * Time.deltaTime;
        if (axis == Axis.y)
            currentEuler.y += speed * Time.deltaTime;
        if (axis == Axis.z)
            currentEuler.z += speed * Time.deltaTime;
        this.transform.localRotation = Quaternion.Euler(currentEuler);
    }
}
