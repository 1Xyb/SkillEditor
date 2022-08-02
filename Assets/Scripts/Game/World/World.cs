using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : SingleTon<World>
{
    public Dictionary<int, ObjectBase> m_insDic = new Dictionary<int, ObjectBase>();
    public HostPlayer m_plyer;//�Լ����е����
    private GameObject npcroot; //npc���ڵ�
    public Camera m_main;//�����

    //��ͼ�ߴ�
    public float xlength;
    public float ylength;

    public void Init()
    {
        //��ͼ�ߴ�ļ���
        GameObject plan = GameObject.Find("Plane");
        Vector3 length = plan.GetComponent<MeshFilter>().mesh.bounds.size;
        xlength = length.x * plan.transform.lossyScale.x;
        ylength = length.z * plan.transform.lossyScale.z;
        Debug.Log($"��ͼ�ĳߴ�Ϊ  x:{xlength}  y:{ylength}");

        m_main = GameObject.Find("Main Camera").GetComponent<Camera>();
        npcroot = GameObject.Find("Npc_Root");
        UIMgr.Instance.Init(GameObject.Find("UIRoot"), GameObject.Find("HUD"));

        //��������ݳ�ʼ����ֵ
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
        m_plyer.CreateObj(MonsterType.Null);//�������
        JoyStickMgr.Instance.SetJoyArg(m_main, m_plyer);//����ҡ�������������ĸ����
        JoyStickMgr.Instance.JoyActive = true;

        CreateIns();
    }

    private void CreateIns()
    {
        //�õ��������Ϣ
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
    /// ����������������
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
            Debug.Log("����ʧ��!!!!");
        }
    }
}
