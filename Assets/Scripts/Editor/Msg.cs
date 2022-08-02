using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

public class ClassSit
{
    public List<string> list;
}
public class Msg : EditorWindow
{
    Vector2 scrollPosition = Vector2.one * 200;
    string inputFiled = "";
    static List<ClassSit> lists;
    [MenuItem("Window/ClassSit")]
    static void Init()
    {
        Msg msg = EditorWindow.GetWindow<Msg>();
        string json = System.IO.File.ReadAllText(Application.dataPath + "/classSitTable.json");
        lists = JsonConvert.DeserializeObject<List<ClassSit>>(json);
    }
    private void OnGUI()
    {
        scrollPosition=GUILayout.BeginScrollView(
                        scrollPosition, false, false, GUILayout.Width(700), GUILayout.Height(500));
        GUILayout.BeginVertical();
        for (int i = 0; i < lists.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (i == 0 || i == 1 || i == 11 || i == 7)
            {
                GUILayout.Button(lists[i].list[0], GUILayout.Width(690), GUILayout.Height(30));
            }
            else if (i == 8 || i == 9)
            {
                for (int j = 0; j < lists[i].list.Count; j++)
                {
                    if (j == 2)
                    {
                        GUILayout.Button(lists[i].list[j], GUILayout.Width(180));
                    }
                    else if (j == 4 || j == 6)
                    {
                        GUILayout.Button(lists[i].list[j], GUILayout.Width(120));
                    }
                    else
                    if (GUILayout.Button(lists[i].list[j], GUILayout.Width(60)))
                    {
                        Debug.Log(lists[i].list[j]);
                    }
                }
            }
            else if (i == 10)
            {
                for (int j = 0; j < lists[i].list.Count; j++)
                {
                    if (j == 6 || j == 8)
                    {
                        GUILayout.Button(lists[i].list[j], GUILayout.Width(120));
                    }
                    else
                    if (GUILayout.Button(lists[i].list[j], GUILayout.Width(60)))
                    {
                        Debug.Log(lists[i].list[j]);
                    }
                }
            }
            else if (true)
            {
                for (int j = 0; j < lists[i].list.Count; j++)
                {
                    if (GUILayout.Button(lists[i].list[j], GUILayout.Width(60)))
                    {
                        Debug.Log(lists[i].list[j]);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        inputFiled = GUILayout.TextField(inputFiled);
    }
}
