using UnityEngine;

public class PlayClipFromArrayOnEvent : MonoBehaviour 
{
    public AudioClip[] clips;
    public GvrAudioSource source;

    void Awake()
    {
        if (source == null)
            source = GetComponent<GvrAudioSource>();
    }

    public void Play(int i)
    {
        source.PlayOneShot(clips[i]);
    }
}
