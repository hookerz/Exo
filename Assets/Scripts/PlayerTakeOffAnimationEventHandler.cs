using UnityEngine;

public class PlayerTakeOffAnimationEventHandler : MonoBehaviour
{
    public Animator animatorToTrigger;
    public string trigger = "peek";

    public EndScreen endScreen;

    void Start()
    {
        if (endScreen == null)
            endScreen = GameObject.FindObjectOfType<EndScreen>();
    }

    public void TakeOffStart()
    {
        animatorToTrigger.SetTrigger(trigger);
    }

    public void TriggerEndScreen()
    {
        endScreen.End();
    }
}
