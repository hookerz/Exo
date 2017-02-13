using UnityEngine;

public class BoundingSphere : MonoBehaviour {
    public Rigidbody drone;
    public float maxForce = 50;
    public float minRadius = 25f, maxRadius = 37.5f;
    public Color minColor = Color.gray, maxColor = Color.white;

    void Start()
    {
        GameObject droneObj = GameObject.FindGameObjectWithTag("Player");
        drone = droneObj.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float dist = (drone.position - this.transform.position).magnitude;

        {
            float p = Mathf.Clamp01((dist - minRadius) / maxRadius);
            float forceMagnitude = p * maxForce;
            drone.AddForce(-drone.position.normalized * forceMagnitude, ForceMode.Acceleration);
        }
    }

	void OnDrawGizmos()
    {
        Gizmos.color = minColor;
        Gizmos.DrawWireSphere(this.transform.position, minRadius);
        Gizmos.color = maxColor;
        Gizmos.DrawWireSphere(this.transform.position, maxRadius);
    }
}
