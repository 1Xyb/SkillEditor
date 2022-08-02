using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 采集物UI
/// </summary>
public class Battlesys : UIbase
{
    public Button m_gather_btn;//采集按钮
    public Slider m_gather_slider;//采集进度条
    private int m_gatherInsid;

    public override void DoCreate(string path)
    {
        base.DoCreate(path);
        //接受采集物消息   客户端
        MsgCenter.Instance.AddListener("ClientMsg", RefreshBtn);
        //接受服务器返回的采集消息
        MsgCenter.Instance.AddListener("ServerMsg", ServerNotify);
    }

    private void ServerNotify(Notification obj)
    {
        if (obj.msg.Equals("gather_callback"))
        {
            m_gather_slider.gameObject.SetActive(true);
            float t = float.Parse(obj.data[2].ToString());
            m_gather_slider.DOValue(1, t).OnComplete(() =>
            {
                m_gather_btn.gameObject.SetActive(false);
                m_gather_slider.gameObject.SetActive(false);
                //发消息给UI通知界面更新
                int synum = (int)obj.data[1];
                int id = (int)obj.data[0];
                Notification notification = new Notification();
                notification.Refresh("RefreshGather", id,synum);
                MsgCenter.Instance.SendMsg("Refresh", notification);
            });
        }
    }

    private void RefreshBtn(Notification obj)
    {
        if (obj.msg.Equals("gather_trigger"))
        {
            m_gatherInsid = (int)obj.data[0];//采集物的实例ID
            m_gather_btn.gameObject.SetActive(true);//显示采集按钮
        }
    }

    public override void DoShow(bool v)
    {
        base.DoShow(v);
        //对组件赋值
        m_gather_btn = m_go.transform.Find("gather_btn").GetComponent<Button>();
        m_gather_slider = m_go.transform.Find("gather_slider").GetComponent<Slider>();

        //点击采集按钮
        m_gather_btn.onClick.AddListener(() =>
        {
            //交互服务器
            Notification notification = new Notification();
            notification.Refresh("gather", m_gatherInsid);
            MsgCenter.Instance.SendMsg("ServerMsg", notification);//给服务器发消息
        });
        //隐藏UI
        m_gather_btn.gameObject.SetActive(false);
        m_gather_slider.gameObject.SetActive(false);
    }

    public override void Destory()
    {
        base.Destory();
        //删掉的时候 移除监听
        MsgCenter.Instance.RemoveListener("ClientMsg", RefreshBtn);
    }
}
