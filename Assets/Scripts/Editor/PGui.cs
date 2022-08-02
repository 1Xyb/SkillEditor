using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PGui : EditorWindow
{
    [MenuItem("Window/AAA")]
    static void AAA()
    {
        PGui gui = EditorWindow.GetWindow<PGui>();
    }
    int selGridInt = 0;
    string[] selStrings = { "radio1", "radio2", "radio3" };

    string stringToEdit = "Hello World\nI've got 2 lines...";

    bool toggleTxt = false;

    void OnGUI()
    {
        //GUILayout.BeginVertical("Box");
        //selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 1);
        //if (GUILayout.Button("Start"))
        //{
        //    Debug.Log("You chose " + selStrings[selGridInt]);
        //}
        //GUILayout.EndVertical();

        //GUILayout.Button("I'm the first button");
        //GUILayout.Space(20);
        //GUILayout.Button("I'm a bit further down");

        stringToEdit = GUILayout.TextArea(stringToEdit, 200);
        GUILayout.Space(15);
        toggleTxt = GUILayout.Toggle(toggleTxt, "A Toggle text");

    }
}
