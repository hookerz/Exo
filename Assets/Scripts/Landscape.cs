using UnityEngine;

[ExecuteInEditMode]
public class Landscape : MonoBehaviour
{
    public Material material;
    public Light light1;
    public Light light2;
    public Light light3;
    public Light light4;
    public float attenPower = 2.0f;
    public bool disableLights = true;
    public bool zeroIntensityToStart = true;
    public bool syncIntensities = true;

    private static string[] intensityParameter = new string[] {
        "_PointLightIntensity1",
        "_PointLightIntensity2",
        "_PointLightIntensity3",
        "_PointLightIntensity4",
    };

    private Light[] lights;
    private float[] previousIntensities;

    void Awake()
    {
        lights = new Light[4];
        previousIntensities = new float[4] { 0, 0, 0, 0 };
        lights[0] = light1;
        lights[1] = light2;
        lights[2] = light3;
        lights[3] = light4;
        
        for (int i = 0; i < lights.Length; i++)
        {
            if (disableLights)
                lights[i].enabled = false;
            if (zeroIntensityToStart)
                lights[i].intensity = 0.0f;
        }
    }

    void Start()
    {
        UpdateLights();
    }

    void UpdateLight(int lightIndex, Light light)
    {
        if (!light.enabled)
        {
            material.SetVector("_PointLightPos" + lightIndex, light.transform.position);
            material.SetColor("_PointLightColor" + lightIndex, light.color);
            material.SetFloat("_PointLightIntensity" + lightIndex, light.intensity);
            material.SetFloat("_PointLightAttenuationPower" + lightIndex, attenPower);
        }
        else
        {
            // turn off this 'fake' light if the real one is on
            material.SetFloat("_PointLightIntensity" + lightIndex, 0.0f);
        }
    }

    void UpdateLights()
    {
        UpdateLight(1, light1);
        UpdateLight(2, light2);
        UpdateLight(3, light3);
        UpdateLight(4, light4);
    }

    void SyncIntensity()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            float intensity = lights[i].intensity;
            if (previousIntensities[i] != intensity)
            {
                material.SetFloat(intensityParameter[i], intensity);
                previousIntensities[i] = intensity;
            }
        }
    }

    void Update()
    {
        if (syncIntensities)
        {
#if UNITY_EDITOR
            UpdateLights();
#else
            SyncIntensity();
#endif
        }
    }
}
