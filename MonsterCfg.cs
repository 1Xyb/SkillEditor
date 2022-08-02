using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class MonsterCfg
{
    static MonsterCfg _instacn;
    public static MonsterCfg Instance
    {
        get
        {
            if (_instacn == null)
            {
                _instacn = new MonsterCfg();
                _instacn.Init();//初始化   解析种怪编辑器里面存数据的JSON配置文件
            }
            return _instacn;
        }
    }

    string path;
    Dictionary<string, MonsterData> data;
    private void Init()
    {
        path = "Assets/Config/monster.json";
        string json = File.ReadAllText(path);
        data= JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(json);
    }

    public Dictionary<string, MonsterData> GetJsonDate()
    {
        return data;
    }
}

public class MonsterData
{
    public string name;
    public float x;
    public float y;
    public float z;
    public MonsterType type;

    public MonsterData(string name, float x, float y, float z, MonsterType type)
    {
        this.name = name;
        this.x = x;
        this.y = y;
        this.z = z;
        this.type = type;
    }
}
