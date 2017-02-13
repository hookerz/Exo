using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public GameObject controller;
    public GameObject droneModel;
    public GameObject rockModel;
    private Animator animator;
    public float minimumAmountOfTimeTilting = 3f;

    public SplineWalker droneIntroWalk;
    public GameObject[] debris;
    private bool debrisTutorialActivated = false;
    private bool oneDebrisDetached = false;

    public GameObject rotationRoot;

    void Start()
    {
        if (droneIntroWalk != null)
            droneIntroWalk.WalkEnded += DroneIntroWalk_WalkEnded;
        if(debris != null && debris.Length > 0)
        {
            for (int i = 0; i < debris.Length; i++)
            {
                debris[i].GetComponent<IAttachable>().Attached += TutorialManager_Attached;
                debris[i].GetComponent<IAttachable>().Detached += TutorialManager_Detached;
            }
        }

        animator = GetComponent<Animator>();
        
        if (rotationRoot != null)
        {
            if(GvrSettings.Handedness == GvrSettings.UserPrefsHandedness.Left)
            {
                Vector3 newPosition = rotationRoot.transform.position;
                newPosition.x *= -1;
                rotationRoot.transform.position = newPosition;
            }
        }

        Hide();
    }

    private void Show()
    {
        controller.SetActive(true);
        droneModel.SetActive(true);
        rockModel.SetActive(true);
    }
    private void Hide()
    {
        controller.SetActive(false);
        droneModel.SetActive(false);
        rockModel.SetActive(false);
    }

    private void DroneIntroWalk_WalkEnded()
    {
        animator.SetTrigger("showThrusterHint");
        StartCoroutine(ShowFlyingInstructions());
    }

    IEnumerator ShowFlyingInstructions()
    {
        Show();

        while (true)
        {
            if (GvrController.IsTouching) {
                animator.SetTrigger("showTiltHint");
                break;
            }

            yield return null;
        }
        Vector3 startEuler = GvrController.Orientation.eulerAngles;
        yield return new WaitForSeconds(minimumAmountOfTimeTilting);

        while (true)
        {
            Vector3 endEuler = GvrController.Orientation.eulerAngles;
            float threshold = 10;
            if ((endEuler - startEuler).sqrMagnitude > threshold * threshold)
                break;

            yield return null;
        }

        Hide();
    }

    private void TutorialManager_Attached(GameObject sender, IAttachable attachable)
    {
        if (!debrisTutorialActivated)
            StartCoroutine(ShowDropDebrisInstructions());
    }

    private void TutorialManager_Detached(GameObject sender, IAttachable attachable)
    {
        oneDebrisDetached = true;
    }

    IEnumerator ShowDropDebrisInstructions()
    {
        debrisTutorialActivated = true;
        Show();
        animator.SetTrigger("showRockHint");

        while (true)
        {
            if (oneDebrisDetached)
                break;

            yield return null;
        }
        
        animator.SetTrigger("stop");
        Hide();
    }
}
