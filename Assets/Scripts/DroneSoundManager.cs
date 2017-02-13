using UnityEngine;

public class DroneSoundManager : MonoBehaviour
{
    public DroneThruster thruster;

    public GvrAudioSource hoverAudioSource;
    public GvrAudioSource thrustAudioSource;

    public float maxPitchChange = 5.0f;
    public float speed = 2.0f;
    public float lerpFactor = 1.0f;

    private float targetHoverVolume = 1.0f;
    private float targetThrustVolume = 0.0f;
    private float targetThrustPitch = 1.0f;

    void Start()
    {
        if (thruster == null)
            thruster = GetComponent<DroneThruster>();

        hoverAudioSource.Play();
        thrustAudioSource.Play();
    }

    void ApproachTargetVolume(GvrAudioSource source, float targetVolume, float lerpFactor)
    {
        source.volume = Mathf.Lerp(source.volume, targetVolume, lerpFactor * Time.deltaTime);
    }

    void ApproachTargetPitch(GvrAudioSource source, float targetPitch, float lerpFactor)
    {
        source.pitch = Mathf.Lerp(source.pitch, targetPitch, lerpFactor * Time.deltaTime);
    }

    void Update()
    {
        if (thruster.ThrustFactor > 0)
        {
            targetThrustVolume = 1.0f;
            targetThrustPitch = thruster.ThrustFactor * maxPitchChange + 1.0f;
        }
        else
        {
            targetThrustVolume = 0.0f;
            targetThrustPitch = 1.0f;
        }
        
        ApproachTargetVolume(hoverAudioSource, targetHoverVolume, lerpFactor);
        ApproachTargetVolume(thrustAudioSource, targetThrustVolume, lerpFactor);
        ApproachTargetPitch(thrustAudioSource, targetThrustPitch, lerpFactor);
    }
}
