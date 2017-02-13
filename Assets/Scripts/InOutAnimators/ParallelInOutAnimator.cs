using UnityEngine;

public class ParallelInOutAnimator : MonoBehaviour, IInOutAnimated
{
    public GameObject[] inOutAnimatedObjects;
    public bool testIn = false;
    public bool testOut = false;
    public float timeScale = 1.0f;

    private float currentStartTime = 0.0f;

    public MonoBehaviour[] enableOnActive;

    private float InTime
    {
        get
        {
            if (float.IsNaN(inTime))
            {
                inTime = 0;
                for (int i = 0; i < InOutAnimated.Length; i++)
                    inTime = Mathf.Max(inTime, InOutAnimated[i].DefaultInTime);
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
                for (int i = 0; i < InOutAnimated.Length; i++)
                    outTime = Mathf.Max(outTime, InOutAnimated[i].DefaultOutTime);
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
    private IInOutAnimated[] inOutAnimated;

    public float DefaultInTime { get { return InTime * timeScale; } }
    public float DefaultOutTime { get { return OutTime * timeScale; } }

    public void StartActivation()
    {
        currentStartTime = 0.0f;
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].StartActivation();
    }

    public void ActivationProgress(float progress)
    {
        float time = progress * InTime * timeScale;
        for (int i = 0; i < InOutAnimated.Length; i++)
        {
            var cur = InOutAnimated[i];
            float ellapsed = time - currentStartTime;
            float p = Mathf.Clamp01(ellapsed / (cur.DefaultInTime * timeScale));
            cur.ActivationProgress(p);
        }
    }

    public void Activated()
    {
        for (int i = 0; i < enableOnActive.Length; i++)
            enableOnActive[i].enabled = true;
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].Activated();
    }

    public void StartDeactivation()
    {
        for (int i = 0; i < enableOnActive.Length; i++)
            enableOnActive[i].enabled = false;
        
        currentStartTime = 0.0f;
        for (int i = 0; i < InOutAnimated.Length; i++)
            InOutAnimated[i].StartDeactivation();
    }

    public void DeactivationProgress(float progress)
    {
        float time = progress * InTime * timeScale;
        for (int i = 0; i < InOutAnimated.Length; i++)
        {
            var cur = InOutAnimated[i];
            float ellapsed = time - currentStartTime;
            float p = Mathf.Clamp01(ellapsed / (cur.DefaultOutTime * timeScale));
            cur.DeactivationProgress(p);
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
