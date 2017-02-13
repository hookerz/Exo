using UnityEngine;

public class MoveTransformByWayPoints : MonoBehaviour
{
    public enum RepeatMode
    {
        Loop,
        PingPong,
    }
    public enum InterpolationMode
    {
        distance,
        time
    }

    public InterpolationMode interpolation = InterpolationMode.distance;
    public Transform[] wayPoints;
    public float[] times;
    public int currentWayPoint = 0;
    private float wayPointStartTime;
    public int direction = 1;
    public Transform targetTransform;
    public Transform[] satelliteTransforms;
    public RepeatMode repeatMode;
    public float pathVelocity = 1.0f;
    public bool drawGizmo = true;
    public Color linesColor = Color.green;

    public bool interpolateRotation = true;

    private Vector3 lastWayPointPosition;
    private Quaternion lastWayPointRotation;

    void Awake()
    {
        if (targetTransform == null)
            Debug.LogWarning("Track (" + this.name + ") has no body associated with it.");

        if (targetTransform != null)
        {
            currentWayPoint = wayPoints.Length - 1;
            Transform pt = wayPoints[currentWayPoint];
            lastWayPointRotation = pt.rotation;
            lastWayPointPosition = pt.position;
            NextWayPoint();
        }
    }

    Transform NextWayPoint()
    {
        currentWayPoint += direction;
        if (currentWayPoint >= wayPoints.Length || currentWayPoint < 0)
        {
            // need to process wrap around
            switch (repeatMode)
            {
                case RepeatMode.Loop:
                    if (direction > 0)
                        currentWayPoint = 0;
                    else
                        currentWayPoint = wayPoints.Length - 1;
                    break;
                case RepeatMode.PingPong:
                    direction = -direction;
                    currentWayPoint += direction * 2;
                    break;
            }
        }
        wayPointStartTime = Time.time;
        return wayPoints[currentWayPoint];
    }

    void Update()
    {
        if (targetTransform == null)
            return;

        if (wayPoints.Length == 0)
            return;

        bool moveToNextWaypoint = false;
        Transform pt = wayPoints[currentWayPoint];
        if (interpolation == InterpolationMode.distance)
        {
            float distanceLeftToTravel = pathVelocity * Time.deltaTime;
            Vector3 currentPos = this.targetTransform.position;
            Vector3 diff = pt.position - currentPos;
            float dist = diff.magnitude;
            if (distanceLeftToTravel < dist)
            {
                Vector3 dir = diff / dist;
                Vector3 position = currentPos + dir * distanceLeftToTravel;
                targetTransform.position = position;
                float totalDist = Vector3.Distance(pt.position, lastWayPointPosition);
                float currentDist = Vector3.Distance(pt.position, position);
                float p = Mathf.Clamp01(currentDist / totalDist);
                if(interpolateRotation)
                    this.targetTransform.rotation = Quaternion.Slerp(pt.rotation, lastWayPointRotation, p);
                for (int i = 0; i < satelliteTransforms.Length; i++)
                {
                    satelliteTransforms[i].position = position;
                    if (interpolateRotation)
                        satelliteTransforms[i].rotation = this.targetTransform.rotation;
                }
            }
            else
                moveToNextWaypoint = true;
        }
        else //if (interpolation == InterpolationMode.time)
        {
            float p = Mathf.Clamp01((Time.time - wayPointStartTime) / times[currentWayPoint]);
            targetTransform.position = Vector3.Lerp(lastWayPointPosition, pt.position, p);
            if(interpolateRotation)
                this.targetTransform.rotation = Quaternion.Slerp(lastWayPointRotation, pt.rotation, p);

            if (p >= 0.99)
                moveToNextWaypoint = true;
        }

        if (moveToNextWaypoint)
        {
            lastWayPointRotation = pt.rotation;
            lastWayPointPosition = pt.position;
            NextWayPoint();
        }
    }

    void OnDrawGizmos()
    {
        if (!drawGizmo)
            return;

        if (wayPoints == null || wayPoints.Length == 0)
            return;

        Gizmos.color = linesColor;
        Vector3 prevPos = wayPoints[0].position;
        for (int i = 1; i < wayPoints.Length; i++)
        {
            Gizmos.DrawLine(prevPos, wayPoints[i].position);
            prevPos = wayPoints[i].position;
        }

        if (repeatMode == RepeatMode.Loop)
        {
            Gizmos.DrawLine(prevPos, wayPoints[0].position);
        }


    }
}
