using UnityEngine;
using System.Collections;

public class StereoCameraSetup : MonoBehaviour 
{
    public LayerMask leftCullingMask;
    public LayerMask leftToggleMask;
    public LayerMask rightCullingMask;
    public LayerMask rightToggleMask;

    void Start() 
    {
        StartCoroutine(FindCamerasAndSetup());
    }

    void DumpCameras()
    {
        Camera[] cams = FindObjectsOfType<Camera>();
        for (int i = 0; i < cams.Length; i++)
        {
            var go = cams[i].gameObject;

            string s = "Camera: " + go.name;
            var comps = go.GetComponents(typeof(Component));
            for (int j = 0; j < comps.Length; j++)
            {
                var comp = comps[j];
                s += " " + comp.GetType().Name + ", ";
            }
            Debug.Log(s);
        }
    }

    IEnumerator FindCamerasAndSetup()
    {
        int i = 0;

        while (true)
        {
            if (i < 10)
            {
                DumpCameras();
            }
            i++;

            // generated at run-time by the GVR SDK
            var go = GameObject.Find("Main Camera Left");
            if (go != null)
            {
                go.GetComponent<Camera>().cullingMask = leftCullingMask;
                go.GetComponent<GvrEye>().toggleCullingMask = leftToggleMask;
            }

            go = GameObject.Find("Main Camera Right");
            if (go != null)
            {
                go.GetComponent<Camera>().cullingMask = rightCullingMask;
                go.GetComponent<GvrEye>().toggleCullingMask = rightToggleMask;
                Debug.Log("Assigned culling masks to eyes and cameras");
                break;
            }

            yield return null;
        }
    }
}
