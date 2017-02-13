using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StarsManager))]
public class StarsManagerInspector : Editor
{
    private StarsManager starsManager;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        starsManager = target as StarsManager;

        if (GUILayout.Button("Create Stars"))
        {
            starsManager.CreateStars();
        }
        if (GUILayout.Button("Delete Stars"))
        {
            starsManager.DeleteStars();
        }
    }
}