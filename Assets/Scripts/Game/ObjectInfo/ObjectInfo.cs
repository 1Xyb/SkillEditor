using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ݻ���
/// </summary>
public class ObjectInfo
{
    public int ID;
    public string m_name;
    public Vector3 m_pos;
    public string m_res;
    public MonsterType m_type;
}

/// <summary>
/// �������
/// </summary>
public class PlayerInfo : ObjectInfo
{
    public int m_level;
    public float m_HP;
    public float m_hpMax;
    public float m_MP;
    public float m_mpMax;
    //�����б�
    public List<SkillJson> skillJsons;
}

/// <summary>
/// NPC����
/// </summary>
public class NpcInfo : ObjectInfo
{
    public int m_plotId = 0;//0�ǲ���Ӧ
    //���츳ֵ
    public NpcInfo(int plot,ObjectInfo info)
    {
        this.ID = info.ID;
        this.m_name = info.m_name;
        this.m_pos = info.m_pos;
        this.m_res = info.m_res;
        this.m_type = MonsterType.NPC;
    }
}

/// <summary>
/// ��������
/// </summary>
public class MonsterInfo : ObjectInfo
{
    public float m_HP;
    public float m_hpMax;

    //����
    public MonsterInfo(MonsterType type,ObjectInfo info)
    {
        this.ID = info.ID;
        this.m_name = info.m_name;
        this.m_pos = info.m_pos;
        this.m_res = info.m_res;
        this.m_type = type;
    }
}
