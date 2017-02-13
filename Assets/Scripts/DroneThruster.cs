using UnityEngine;

public enum ThrustMode
{
    FullOnTouch,
    OneToMax,
    ZeroToMax,
    CustomToMax,
}

public class DroneThruster : MonoBehaviour 
{
    public Rigidbody body;
    public float velocityLimit = 5.0f;
    public AnimationCurve touchPositionToThrust;
    public float speedLerpFactor = 5.0f;
    public bool simulateThrust = false;

    private float thrustFactor = 0.0f;
    private float targetSpeed = 0.0f;

    public float ThrustFactor { get { return thrustFactor; } }
    public float MaxVelocityPercent { get { return Mathf.Clamp01(body.velocity.magnitude / velocityLimit); } }

    void Awake()
    {
        if (body == null)
            body = GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
    {
        if (GvrController.State != GvrConnectionState.Connected)
            return;

        if (simulateThrust)
        {
            // fake thrust while the drone is landing during the intro...
            thrustFactor = 1.0f;
            return;
        }

        if (GvrController.IsTouching)
        {
            float y01 = 1 - GvrController.TouchPos.y;
            thrustFactor = touchPositionToThrust.Evaluate(y01);
            targetSpeed = thrustFactor * velocityLimit;
            Vector3 targetVelocity = targetSpeed * body.transform.up;
            Vector3 newVelocity = Vector3.Lerp(body.velocity, targetVelocity, speedLerpFactor * Time.deltaTime);
            body.velocity = newVelocity;
        }
        else
        {
            thrustFactor = 0.0f;
        }
    }
}
