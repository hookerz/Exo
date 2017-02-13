using UnityEngine;
using System.Collections.Generic;

public class PauseSounds : MonoBehaviour
{
    private List<GvrAudioSource> audioSources = new List<GvrAudioSource>();
    private bool[] wasPlaying;
    private float[] times;
    private float[] volumes;
    public GvrAudioSource[] exceptions;

    private PauseScreen pauseScreen;

    void Awake()
    {
        GvrAudioSource[] aSources = GameObject.FindObjectsOfType<GvrAudioSource>();
        for (int i = 0; i < aSources.Length; i++)
        {
            bool isException = false;
            if (exceptions.Length > 0)
            {
                for (int j = 0; j < exceptions.Length; j++)
                {
                    if (aSources[i].GetInstanceID() == exceptions[j].GetInstanceID())
                    {
                        isException = true;
                        break;
                    }
                }
            }

            if (!isException)
                audioSources.Add(aSources[i]);
        }

        wasPlaying = new bool[audioSources.Count];
        times = new float[audioSources.Count];
        volumes = new float[audioSources.Count];

        pauseScreen = GameObject.FindObjectOfType<PauseScreen>();
    }

    void OnEnable()
    {
        pauseScreen.GamePaused += PauseScreen_GamePaused;
    }
    void OnDisable()
    {
        pauseScreen.GamePaused -= PauseScreen_GamePaused;
    }

    private void PauseScreen_GamePaused(bool paused)
    {
        if (paused)
        {
            for (int i = 0; i < audioSources.Count; i++)
            {
                times[i] = audioSources[i].time;
                volumes[i] = audioSources[i].volume;
                wasPlaying[i] = audioSources[i].isPlaying;
                audioSources[i].Stop();
            }
        }
        else
        {
            for (int i = 0; i < audioSources.Count; i++)
            {
                audioSources[i].volume = volumes[i];
                audioSources[i].time = times[i];
                if (wasPlaying[i])
                {
                    audioSources[i].Play();
                }
            }
        }
    }
}
