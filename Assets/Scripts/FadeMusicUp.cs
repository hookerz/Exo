using UnityEngine;

public class FadeMusicUp : MonoBehaviour
{
    public MusicManager musicManager;

    void Awake()
    {
        musicManager = FindObjectOfType<MusicManager>();
    }
    
    public void TriggerFadeMusicUp()
    {
        musicManager.FadeMusicUp();
    }
}
