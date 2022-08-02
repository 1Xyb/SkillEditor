using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    int toolbarInt = 0;
    string[] toolbarStrings = { "Toolbar1", "Toolbar2", "Toolbar3" };

    void OnGUI()
    {
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
    }
}
