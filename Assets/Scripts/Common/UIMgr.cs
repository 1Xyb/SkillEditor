using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : SingleTon<UIMgr>
{
    public GameObject m_uiroot, m_hudroot;
    //                      ����uiԤ����     
    public Dictionary<string, UIbase> m_uiDic;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="root"></param>
    /// <param name="hud"></param>
    public void Init(GameObject root,GameObject hud)
    {
        m_uiroot = root;
        m_hudroot = hud;
        m_uiDic = new Dictionary<string, UIbase>();
        m_uiDic.Add("Lobby", new Lobbysys());
        m_uiDic.Add("battle", new Battlesys());
        m_uiDic.Add("minimap", new MinimapSys());
        m_uiDic.Add("taskPanel", new TaskSys());

        Open("Lobby");
        Open("battle");
        Open("minimap");
        Open("taskPanel");
    }

    /// <summary>
    /// ��UI���
    /// </summary>
    /// <param name="key">·��</param>
    public void Open(string key)
    {
        UIbase ui;
        if (m_uiDic.TryGetValue(key, out ui))
        {
            ui.DoCreate(key);
        }
    }

    /// <summary>
    /// �ر����
    /// </summary>
    /// <param name="key"></param>
    public void Close(string key)
    {
        UIbase ui;
        if (m_uiDic.TryGetValue(key, out ui))
        {
            ui.Destory();
        }
    }
}
