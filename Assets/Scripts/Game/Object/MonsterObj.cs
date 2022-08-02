using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Null = 0,//һ�㶼�� -1
    Normal, //����
    Gather, //�ɼ���
    Follow, //������
    NPC,
    MaxValue = 99,
}

public class MonsterObj:ObjectBase
{
    public MonsterInfo m_info;//��������
    public Enemy AI;

    //����
    public MonsterObj(MonsterType type,MonsterInfo info)
    {
        info.m_type = type;
        m_info = info;
        m_insID = info.ID;
        m_modelPath = info.m_res;
    }
}

/// <summary>
/// ��������
/// </summary>
public class Normal : MonsterObj
{
    //����                                            ����Ĺ��캯��
    public Normal(MonsterInfo info) : base(MonsterType.Normal,info )
    {

    }

    public Normal(ObjectInfo info):base(MonsterType.Normal,new MonsterInfo(MonsterType.Normal, info)) { }

    //��������
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);//����λ��
        base.CreateObj(type);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        //������������   Ѫ�� ��
        m_pate = m_go.AddComponent<UIPate>();
        m_pate.InitPate();
        m_pate.m_name.gameObject.SetActive(true);
        m_pate.m_hp.gameObject.SetActive(true);
        m_pate.m_mp.gameObject.SetActive(false);
        m_pate.m_gather.gameObject.SetActive(false);
        m_pate.text.gameObject.SetActive(false);
    }
}


/// <summary>
/// �ɼ���
/// </summary>
public class Gather : MonsterObj
{
    public Gather(MonsterInfo info) : base(MonsterType.Gather, info) { }

    public Gather(ObjectInfo info) : base(MonsterType.Gather, new MonsterInfo(MonsterType.Gather,info)) { }

    //���ɲɼ���
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);//��������λ��
        base.CreateObj(type);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        //���ɼ������Բ�μ�ⷶΧ
        StaticCircleCheck check = m_go.AddComponent<StaticCircleCheck>();
        check.m_target = World.Instance.m_plyer.m_go;//�󶨼��Ŀ��=���
        //�ص�
        check.m_call = (isenter) =>
        {
            Debug.Log("��Ҵ������ң�" + m_info.m_res);
            Notification notification = new Notification();
            notification.Refresh("gather_trigger", m_info.ID);//�ɼ����ID  //������Ϣ������
            MsgCenter.Instance.SendMsg("ClientMsg", notification);//����Ϣ��������
        };

        //���ܿͻ��˷��صĲɼ�����Ϣ
        MsgCenter.Instance.AddListener("Refresh", RefreshNotify);

         //�ɼ�������
         m_pate = m_go.AddComponent<UIPate>();
        m_pate.InitPate();
        m_pate.m_name.gameObject.SetActive(false);
        m_pate.m_hp.gameObject.SetActive(false);
        m_pate.m_mp.gameObject.SetActive(false);
        m_pate.m_gather.gameObject.SetActive(true);
        m_pate.text.gameObject.SetActive(false);
    }

    private void RefreshNotify(Notification obj)
    {
        if (obj.msg.Equals("RefreshGather"))
        {
            int id = (int)obj.data[0];
            //�ж��Ƿ��ǵ�ǰ�Ĳɼ���
            if (id == m_insID)
            {
                m_pate.SetData((int)obj.data[1]);//2
            }
        }
    }

}

public class Follow : MonsterObj
{
    public Follow(MonsterInfo info) : base(MonsterType.Follow, info) { }

    public Follow(ObjectInfo info) : base(MonsterType.Follow, new MonsterInfo(MonsterType.Follow, info)) { }

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
