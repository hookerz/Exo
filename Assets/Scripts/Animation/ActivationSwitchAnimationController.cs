using UnityEngine;

public class ActivationSwitchAnimationController : MonoBehaviour
{
    void TriggerTowerActivation()
    {
        transform.parent.parent.GetComponent<Animator>().SetTrigger("activate");
    }
}
