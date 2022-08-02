using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbase
{
    public GameObject m_go;

    public virtual void DoCreate(string path)
    {
        InsGo(path);//实例化
        DoShow(true);//显示
    }

    /// <summary>
    /// 判断是否显示
    /// </summary>
    /// <param name="v"></param>
    public virtual void DoShow(bool v)
    {
        if (m_go)
        {
            m_go.SetActive(v);
        }
    }

    private void InsGo(string path)
    {
        m_go = GameObject.Instantiate(Resources.Load<GameObject>(path));//实例化
        m_go.transform.SetParent(UIMgr.Instance.m_uiroot.transform, false);//设置父级
        m_go.transform.localPosition = Vector3.zero;
        m_go.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void Destory()
    {
        GameObject.Destroy(m_go);
    }
}
