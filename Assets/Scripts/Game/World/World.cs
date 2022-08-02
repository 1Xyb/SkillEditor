using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : SingleTon<World>
{
    public Dictionary<int, ObjectBase> m_insDic = new Dictionary<int, ObjectBase>();
    public HostPlayer m_plyer;//自己持有的玩家
    private GameObject npcroot; //npc根节点
    public Camera m_main;//主相机

    //地图尺寸
    public float xlength;
    public float ylength;

    public void Init()
    {
        //地图尺寸的计算
        GameObject plan = GameObject.Find("Plane");
        Vector3 length = plan.GetComponent<MeshFilter>().mesh.bounds.size;
        xlength = length.x * plan.transform.lossyScale.x;
        ylength = length.z * plan.transform.lossyScale.z;
        Debug.Log($"地图的尺寸为  x:{xlength}  y:{ylength}");

        m_main = GameObject.Find("Main Camera").GetComponent<Camera>();
        npcroot = GameObject.Find("Npc_Root");
        UIMgr.Instance.Init(GameObject.Find("UIRoot"), GameObject.Find("HUD"));

        //对玩家数据初始化赋值
        PlayerInfo info = new PlayerInfo();
        info.ID = 0;
        info.m_name = "tony";
        info.m_level = 9;
        info.m_pos = Vector3.zero;
        info.m_res = "Teddy";
        info.m_HP = 2000;
        info.m_MP = 1000;
        info.m_hpMax = 2000;
        info.m_mpMax = 2000;

        m_plyer = new HostPlayer(info);
        m_plyer.CreateObj(MonsterType.Null);//创建玩家
        JoyStickMgr.Instance.SetJoyArg(m_main, m_plyer);//设置摇杆相机看向和是哪个相机
        JoyStickMgr.Instance.JoyActive = true;

        CreateIns();
    }

    private void CreateIns()
    {
        //得到怪物的信息
        Dictionary<string, MonsterData> data  = MonsterCfg.Instance.GetJsonDate();
        ObjectInfo info;//data

        foreach (var item in data.Values)
        {
            info = new ObjectInfo();
            info.ID = m_insDic.Count + 1;
            info.m_name = string.Format("{0}({1})", item.name, info.ID);
            info.m_res = item.name;
            info.m_pos = new Vector3(item.x, item.y, item.z);
            info.m_type = item.type;
            CreateObj(info);
        }
    }

    ObjectBase monster = null;
    /// <summary>
    /// 根据数据生成物体
    /// </summary>
    /// <param name="info"></param>
    private void CreateObj(ObjectInfo info)
    {
        monster = null;
        if (info != null)
        {
            if (info.m_type == MonsterType.Normal)
            {
                monster = new Normal(info);
            }
            else if (info.m_type == MonsterType.Gather)
            {
                monster = new Gather(info);
            }
            else if (info.m_type == MonsterType.NPC)
            {
                monster = new NPCObj(1, info);
            }
        }
        if (monster != null)
        {
            monster.CreateObj(info.m_type);
            monster.m_go.transform.SetParent(npcroot.transform, false);
            m_insDic.Add(info.ID, monster);
        }
        else
        {
            Debug.Log("生成失败!!!!");
        }
    }
}
