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
        //��ʾ���
        base.DoShow(v);
        //�ҵ���Ӧ�����
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
