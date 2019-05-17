using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToggleGroupConfig))]
public class ToggleGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var toggleGroupConfig = (ToggleGroupConfig)target;
        if(GUILayout.Button("Draw Toggle Buttons"))
        {
            toggleGroupConfig.UpdateButtons();
        }
    }
}
