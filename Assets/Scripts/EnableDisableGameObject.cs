using UnityEngine;

public class EnableDisableGameObject : MonoBehaviour 
{
    public GameObject[] objects;

    public void Enable()
    {
        Utils.SetActive(objects, true);
    }

    public void Disable()
    {
        Utils.SetActive(objects, false);
    }
}
