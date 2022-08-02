using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础种类
/// </summary>
public abstract class ObjectBase
{
    public GameObject m_go;//存储当前物体
    public Vector3 m_local_pos;//当前物体在本地的位置
    public Animator m_anim;
    public UIPate m_pate;
    public MonsterType m_type;

    public int m_insID;//实例ID 服务器唯一ID
    public string m_modelPath;//模型路径

    public ObjectBase() { }

    /// <summary>
    /// 创建物体的方法
    /// </summary>
    /// <param name="type"></param>
    public virtual void CreateObj(MonsterType type)
    {
        m_type = type;
        if (!string.IsNullOrEmpty(m_modelPath) && m_insID >= 0)
        {
            //实例化物体
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
    /// 在创建的时候初始化的逻辑
    /// </summary>
    public virtual void OnCreate()
    {
        
    }
    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="pos"></param>
    public virtual void SetPos(Vector3 pos)
    {
        m_local_pos = pos;
    }
    /// <summary>
    /// 移动
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
