using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public abstract class ObjectBase
{
    public GameObject m_go;//�洢��ǰ����
    public Vector3 m_local_pos;//��ǰ�����ڱ��ص�λ��
    public Animator m_anim;
    public UIPate m_pate;
    public MonsterType m_type;

    public int m_insID;//ʵ��ID ������ΨһID
    public string m_modelPath;//ģ��·��

    public ObjectBase() { }

    /// <summary>
    /// ��������ķ���
    /// </summary>
    /// <param name="type"></param>
    public virtual void CreateObj(MonsterType type)
    {
        m_type = type;
        if (!string.IsNullOrEmpty(m_modelPath) && m_insID >= 0)
        {
            //ʵ��������
            m_go = GameObject.Instantiate(Resources.Load<GameObject>(m_modelPath));
            m_go.name = m_insID.ToString();
            m_go.transform.position = m_local_pos;
            if (m_go)
            {
                OnCreate();
            }
        }
    }
    /// <summary>
    /// �ڴ�����ʱ���ʼ�����߼�
    /// </summary>
    public virtual void OnCreate()
    {
        
    }
    /// <summary>
    /// ����λ��
    /// </summary>
    /// <param name="pos"></param>
    public virtual void SetPos(Vector3 pos)
    {
        m_local_pos = pos;
    }
    /// <summary>
    /// �ƶ�
    /// </summary>
    /// <param name="look">h</param>
    /// <param name="move">v</param>
    public void MoveByTranslate(Vector3 look,Vector3 move)
    {
        m_go.transform.LookAt(look);
        m_go.transform.Translate(move);
    }

    public virtual void Destory()
    {
        //if (m_pate)
        //{
        //    GameObject.Destroy(m_pate);
        //}
        GameObject.Destroy(m_go);
        m_local_pos = Vector3.zero;
        m_anim = null;
        m_insID = -1;
    }
}
