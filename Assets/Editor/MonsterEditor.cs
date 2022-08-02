using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Newtonsoft.Json;

public class MonsterValue
{
    public bool isSelect = true;
    public MonsterType type = MonsterType.Normal;
    public MonsterValue() { }
    public MonsterValue(bool iss,MonsterType t)
    {
        this.isSelect = iss;
        this.type = t;
    }
}


public class MonsterEditor : EditorWindow
{
    static MonsterEditor win;

    Transform monsterParent;
    int curCount = 0;
    Dictionary<string, MonsterData> dataDic = new Dictionary<string, MonsterData>();
    Dictionary<string, MonsterValue> valueDic = new Dictionary<string, MonsterValue>();
    Dictionary<string, GameObject> objDic = new Dictionary<string, GameObject>();

    [MenuItem("Tools/种怪编辑器")]
    static void ShowMonsterWindow()
    {
        win = EditorWindow.GetWindow<MonsterEditor>("");
        win.Show();
    }

    private void OnEnable()
    {
        monsterParent = GameObject.Find("Npc_Root").transform;
        Load();
    }

    private void Load()
    {
        dataDic.Clear();
        valueDic.Clear();
        objDic.Clear();
        while (curCount < monsterParent.childCount)
            Add();
        while (curCount > monsterParent.childCount)
            Del();
        if (!File.Exists("Assets/Config/monster.json")) return;
        string json = File.ReadAllText("Assets/Config/monster.json");
        Dictionary<string, MonsterData> data = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(json);
        foreach (var item in data)
        {
            if (dataDic.ContainsKey(item.Key))
            {
                MonsterData monsterData = data[item.Key];
                dataDic[item.Key] = monsterData;
            }
            else
            {
                curCount++;
                dataDic.Add(item.Key, item.Value);
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = item.Key;
                go.transform.SetParent(monsterParent);
                go.transform.position = new Vector3(item.Value.x, item.Value.y, item.Value.z);
                objDic.Add(item.Key, go);
                valueDic.Add(item.Key, new MonsterValue());
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (curCount > 0)
        {
            foreach (var item in dataDic.Values)
            {
                if (objDic[item.name].activeSelf)
                {
                    GUILayout.Label(item.name);
                    GUILayout.BeginHorizontal();
                    valueDic[item.name].isSelect = EditorGUILayout.Toggle("Selected", valueDic[item.name].isSelect);
                    MonsterType type = (MonsterType)EditorGUILayout.EnumPopup("type:", dataDic[item.name].type);
                    if (!type.Equals(dataDic[item.name].type))
                    {
                        dataDic[item.name].type = type;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    Vector3 orgin = new Vector3(dataDic[item.name].x, dataDic[item.name].y, dataDic[item.name].z);
                    Vector3 p = EditorGUILayout.Vector3Field("Position", orgin);
                    if (!orgin.Equals(p))
                    {
                        dataDic[item.name].x = p.x;
                        dataDic[item.name].y = p.y;
                        dataDic[item.name].z = p.z;
                        objDic[item.name].transform.position = p;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);
                }
               
            }
        }

            if (GUILayout.Button("保存"))
            {
                SaveData();
            }
}

    private void SaveData()
    {
        Dictionary<string, MonsterData> newDic = new Dictionary<string, MonsterData>();
        foreach (var item in dataDic)
        {
            if (!valueDic[item.Key].isSelect) continue;
            string key = string.Copy(item.Key);
            newDic.Add(key, item.Value);
        }
        string json = JsonConvert.SerializeObject(newDic);
        File.WriteAllText("Assets/Config/monster.json",json);
        Debug.Log("生成json成功");
    }

    private void Update()//帧   千万不要写运算逻辑
    {
        if (curCount<monsterParent.childCount)
            Add();
        else if (curCount > monsterParent.childCount)
            Del();
    }

    private void Del()
    {
        foreach (var item in dataDic.Keys)
        {
            bool f = false;
            foreach (Transform ite in monsterParent)
            {
                if (ite.name.Equals(item))
                {
                    f = true;
                }
            }
            if (!f)
            {
                dataDic.Remove(item);
                valueDic.Remove(item);
                objDic.Remove(item);
                curCount--;
                break;
            }
        }
    }

    private void Add()
    {
        foreach (Transform item in monsterParent)
        {
            if (dataDic.ContainsKey(item.name))
            {
                continue;
            }
            else
            {
                MonsterData data = new MonsterData(item.name, item.position.x, item.position.y, item.position.z, MonsterType.Normal);
                dataDic.Add(item.name, data);
                valueDic.Add(item.name, new MonsterValue());
                objDic.Add(item.name, item.gameObject);
                curCount++;
                break;
            }
        }
    }
}
