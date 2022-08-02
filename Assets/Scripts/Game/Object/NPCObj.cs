using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 ����NPC����
*/
public class NPCObj : ObjectBase
{
    public NpcInfo m_info;//NPC����
    public NPCObj(NpcInfo info)
    {
        m_info = info;
        m_insID = info.ID;
        m_modelPath = info.m_res;
    }

    public NPCObj(int plot, ObjectInfo info)
    {
        m_info = new NpcInfo(plot, info);
        m_insID = info.ID;
        m_modelPath = info.m_res;

    }

    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);
        base.CreateObj(type);
    }

    public override void OnCreate()
    {
        base.OnCreate();
    }
}


