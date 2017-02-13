using UnityEngine;
using System.Collections;

public class StackedInOutAnimator : MonoBehaviour, IInOutAnimated
{
    public GameObject[] inOutAnimatedObjects;
    public bool testIn = false;
    public bool testOut = false;
    public float timeScale = 1.0f;

    private int current = -1;
    private float currentStartTime = 0.0f;

    private float InTime
    {
        get
        {
            if (float.IsNaN(inTime))
            {
                inTime = 0;
                for (int i = 0; i < inOutAnimatedObjects.Length; i++)
                    inTime += InOutAnimated[i].DefaultInTime;
            }

            return inTime;
        }
    }
    private float inTime = float.NaN;
    private float OutTime
    {
        get
        {
            if (float.IsNaN(outTime))
            {
                outTime = 0;
                for (int i = 0; i < inOutAnimatedObjects.Length; i++)
                    outTime += InOutAnimated[i].DefaultOutTime;
            }

            return outTime;
        }
    }
    private float outTime = float.NaN;

    public IInOutAnimated[] InOutAnimated
    {
        get
        {
            if (inOutAnimated == null)
            {
                inOutAnimated = new IInOutAnimated[inOutAnimatedObjects.Length];
                for (int i = 0; i < inOutAnimatedObjects.Length; i++)
                    inOutAnimated[i] = inOutAnimatedObjects[i].GetComponent<IInOutAnimated>();
            }

            return inOutAnimated;
        }
    }
    private IInOutAnimated[] inOutAnimated = null;


    public float DefaultInTime { get { return InTime * timeScale; } }
    public float DefaultOutTime { get { return OutTime * timeScale; } }

    public void StartActivation()
    {
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].Deactivated();
        current = 0;
        currentStartTime = 0.0f;
        InOutAnimated[current].StartActivation();
    }

    public void ActivationProgress(float progress)
    {
        float time = progress * InTime * timeScale;
        while (true)
        {
            var cur = InOutAnimated[current];
            float elapsed = time - currentStartTime;
            float p = Mathf.Clamp01(elapsed / (cur.DefaultInTime * timeScale));
            cur.ActivationProgress(p);

            if (p == 1.0f)
            {
                // we need to progress to the next animation
                cur.Activated();
                current++;
                if (current < InOutAnimated.Length)
                {
                    currentStartTime += cur.DefaultInTime * timeScale;
                    cur = InOutAnimated[current];
                    cur.StartActivation();
                }
                else
                {
                    current--;
                    break;
                }
            }
            else
                break;
        }
    }

    public void Activated()
    {
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].Activated();
    }

    public void StartDeactivation()
    {
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].Activated();
        current = InOutAnimated.Length - 1;
        currentStartTime = 0.0f;
        InOutAnimated[current].StartDeactivation();
    }

    public void DeactivationProgress(float progress)
    {
        float time = progress * InTime * timeScale;

        while (true)
        {
            var cur = InOutAnimated[current];
            float ellapsed = time - currentStartTime;
            float p = Mathf.Clamp01(ellapsed / (cur.DefaultOutTime * timeScale));
            cur.DeactivationProgress(p);

            if (p == 1.0f)
            {
                // we need to progress to the next animation
                cur.Deactivated();
                current--;
                if (current >= 0)
                {
                    currentStartTime = currentStartTime + (cur.DefaultOutTime * timeScale);
                    cur = InOutAnimated[current];
                    cur.StartDeactivation();
                }
                else
                {
                    current = 0;
                    break;
                }
            }
            else
                break;
        }

    }

    public void Deactivated()
    {
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].Deactivated();
    }

    void Update()
    {
        if (testIn == true)
        {
            StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(this));
            testIn = false;
        }
        if (testOut == true)
        {
            StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(this));
            testOut = false;
        }
    }
}
