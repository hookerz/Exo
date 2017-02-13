using UnityEngine;

public class BackHandler : MonoBehaviour
{
    void Awake()
    {
        Input.backButtonLeavesApp = true;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
