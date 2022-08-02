using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobbysys : UIbase
{
    public Image m_head;
    public List<Image> m_buffs;

    public override void DoCreate(string path)
    {
        m_buffs = new List<Image>();
        base.DoCreate(path);
    }

    public override void DoShow(bool v)
    {
        //显示与否
        base.DoShow(v);
        //找到对应的组件
        m_head = m_go.transform.Find("head").GetComponent<Image>();
        Transform buffgo = m_go.transform.Find("bufflayout").transform;
        for (int i = 0; i < buffgo.childCount; i++)
        {
            m_buffs.Add(buffgo.GetChild(i).GetComponent<Image>());
        }
    }

    public override void Destory()
    {
        base.Destory();
    }
}
