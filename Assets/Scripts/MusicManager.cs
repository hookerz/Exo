using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource[] audioSources;
    public CompletableObstacleCollection completableObstacles;
    public float fadeDuration = 1;
    private float percentageStep, nextPercentageThreshold = 0;
    private int currentAudio = 0;
    public float targetGameplayAudio = 0.6f;
    public float endingFadeupTime = 5.0f;
    
    void Start()
    {
        if (audioSources.Length > 0)
        {
            percentageStep = 1.0f / audioSources.Length;

            FadeInNextAudioSource();
        }
    }
    private void FadeInNextAudioSource()
    {
        StartCoroutine(Utils.SmoothStepAudioSourceVolumeUnscaled(audioSources[currentAudio++], fadeDuration, targetGameplayAudio));
        nextPercentageThreshold += percentageStep;
    }

    void OnEnable()
    {
        completableObstacles.ProgressiveObstaclePercent += CompletableObstacleCollection_ProgressiveObstaclePercent;
    }

    void OnDisable()
    {
        completableObstacles.ProgressiveObstaclePercent -= CompletableObstacleCollection_ProgressiveObstaclePercent;
    }

    private void CompletableObstacleCollection_ProgressiveObstaclePercent(IProgressiveCompletableObstacle obstacle, float percent)
    {
        while(currentAudio < audioSources.Length && percent >= nextPercentageThreshold)
            FadeInNextAudioSource();
    }

    public void FadeMusicUp()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            StartCoroutine(Utils.SmoothStepAudioSourceVolumeUnscaled(audioSources[i], endingFadeupTime, 1.0f));
        }
    }
}
