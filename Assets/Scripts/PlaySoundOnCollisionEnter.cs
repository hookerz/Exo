using UnityEngine;

public class PlaySoundOnCollisionEnter : MonoBehaviour
{
    public GvrAudioSource audioSource;
    public float relativeSpeedThreshold = 1;
    public float minTimeBetweenSounds = 0.25f;
    public float minPitch = 0.5f, maxPitch = 1.5f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > relativeSpeedThreshold && !audioSource.isPlaying && Time.time >= minTimeBetweenSounds) {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }
    }
}
