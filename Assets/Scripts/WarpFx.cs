using UnityEngine;

public class WarpFx : MonoBehaviour
{
    public float[] lineFxSpeeds;
    public ParticleSystem[] lineFxs;
    public float[] starsFxSpeeds;
    public ParticleSystem[] starsFxs;

    public GameObject[] cylinders;
    public float[] cylinderAngularSpeeds;
    private float[] cylinderRotations;
    private Vector3[] cylinderStartRotations;

    void Start()
    {
        if(lineFxs.Length == lineFxSpeeds.Length)
        {
            for (int i = 0; i < lineFxSpeeds.Length; i++)
                lineFxs[i].playbackSpeed = lineFxSpeeds[i];
        }
        if (starsFxs.Length == starsFxSpeeds.Length)
        {
            for (int i = 0; i < starsFxSpeeds.Length; i++)
                starsFxs[i].playbackSpeed = starsFxSpeeds[i];
        }

        cylinderRotations = new float[cylinders.Length];
        cylinderStartRotations = new Vector3[cylinders.Length];
        for (int i = 0; i < cylinders.Length; i++) { 
            cylinderRotations[i] = 0;
            cylinderStartRotations[i] = cylinders[i].transform.rotation.eulerAngles;
        }
    }

    void Update()
    {
        for(int i = 0; i < cylinderRotations.Length; i++)
        {
            cylinderRotations[i] = (cylinderRotations[i] + cylinderAngularSpeeds[i] * Time.deltaTime);
            Vector3 euler = cylinderStartRotations[i];
            euler.x = cylinderRotations[i];

            cylinders[i].transform.rotation = Quaternion.Euler(euler);
        }
    }
}
