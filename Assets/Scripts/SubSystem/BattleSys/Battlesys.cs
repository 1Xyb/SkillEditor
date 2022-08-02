using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �ɼ���UI
/// </summary>
public class Battlesys : UIbase
{
    public Button m_gather_btn;//�ɼ���ť
    public Slider m_gather_slider;//�ɼ�������
    private int m_gatherInsid;

    public override void DoCreate(string path)
    {
        base.DoCreate(path);
        //���ܲɼ�����Ϣ   �ͻ���
        MsgCenter.Instance.AddListener("ClientMsg", RefreshBtn);
        //���ܷ��������صĲɼ���Ϣ
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
                //����Ϣ��UI֪ͨ�������
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
            m_gatherInsid = (int)obj.data[0];//�ɼ����ʵ��ID
            m_gather_btn.gameObject.SetActive(true);//��ʾ�ɼ���ť
        }
    }

    public override void DoShow(bool v)
    {
        base.DoShow(v);
        //�������ֵ
        m_gather_btn = m_go.transform.Find("gather_btn").GetComponent<Button>();
        m_gather_slider = m_go.transform.Find("gather_slider").GetComponent<Slider>();

        //����ɼ���ť
        m_gather_btn.onClick.AddListener(() =>
        {
            //����������
            Notification notification = new Notification();
            notification.Refresh("gather", m_gatherInsid);
            MsgCenter.Instance.SendMsg("ServerMsg", notification);//������������Ϣ
        });
        //����UI
        m_gather_btn.gameObject.SetActive(false);
        m_gather_slider.gameObject.SetActive(false);
    }

    public override void Destory()
    {
        base.Destory();
        //ɾ����ʱ�� �Ƴ�����
        MsgCenter.Instance.RemoveListener("ClientMsg", RefreshBtn);
    }
}
