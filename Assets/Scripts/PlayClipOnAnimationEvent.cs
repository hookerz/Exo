using UnityEngine;

public class PlayClipOnAnimationEvent : MonoBehaviour
{
    public GvrAudioSource source;
    public AudioClip clip;

    public void PlayClipNow()
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayClipOneShot()
    {
        source.PlayOneShot(clip);
    }
}
