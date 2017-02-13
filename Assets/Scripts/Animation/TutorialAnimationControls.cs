using UnityEngine;

public class TutorialAnimationControls : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = transform.GetComponent<Animator>();
    }

    void SignalTrigger(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
