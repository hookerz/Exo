using UnityEngine;


[ExecuteInEditMode]
public class MainButtonScaleController : MonoBehaviour
{
    public float scale = 0.5f;
    public Transform target;

    void Update()
    {
        Vector3 s = new Vector3(scale, 0.5f, scale);

        target.localScale = s;
    }
}
