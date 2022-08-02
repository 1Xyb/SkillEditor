using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ֻҪ���ˣ��������Լ����Ǳ��ˣ�����player
/// </summary>
public class PlayerObj : ObjectBase
{
    public PlayerInfo m_info;//�������

    public PlayerObj(PlayerInfo info) { m_info = info; }

    public override void OnCreate()
    {
        base.OnCreate();
        m_go.name = "Player";
        //��ʾUI
        m_pate = m_go.AddComponent<UIPate>();//��ű�
        m_pate.InitPate();//��ʼ��
        m_pate.m_gather.SetActive(false);
        m_pate.text.SetActive(false);
        //��������
        m_pate.SetData(m_info.m_name, m_info.m_HP / m_info.m_hpMax, m_info.m_MP / m_info.m_mpMax);
    }
}

/// <summary>
/// �Լ����Ƶ�player
/// </summary>
public class HostPlayer : PlayerObj
{
    Player player;
    public HostPlayer(PlayerInfo info):base(info) 
    {
        m_insID = info.ID;
        m_modelPath = info.m_res;
    }
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);
        base.CreateObj(type);
    }
    public override void OnCreate()
    {
        base.OnCreate();
        player = m_go.GetComponent<Player>();
        player.InitData();

        //atkActionPlay ������Ϣ ���Ź�������
        MsgCenter.Instance.AddListener("atkActionPlay", (notify) => {
            if (notify.msg.Equals("ByServer"))
            {
                int skillid = (int)notify.data[0];//��������
                player.SetData(skillid.ToString());//�������ü��� �����˵�ǰ�������
                player.Play();//����
            }
        });
    }

    //Notification notify = new Notification();
    /// <summary>
    /// ҡ�˿����ƶ�
    /// </summary>
    /// <param name="h"></param>
    /// <param name="v"></param>
    public void JoystickHandlerMoving(float h, float v)
    {
        if (Mathf.Abs(h) > 0.05f || (Mathf.Abs(v) > 0.05f))
        {                                                                                       //look                                                                                                          //move
            MoveByTranslate(new Vector3(m_go.transform.position.x + h, m_go.transform.position.y, m_go.transform.position.z + v), Vector3.forward * Time.deltaTime * 3);
            //notify.Refresh("Player", m_go.transform.position);
            //MsgCenter.Instance.SendMsg("MovePos", notify);
        }
    }

    //TODO   == �����ͷ� ����  �ɷ��¼�  ֪ͨ������
    public void JoyButtonHandler(string btnName)
    {
        List<SkillBase> componentList;
        switch (btnName)
        {
            case "Attack":
                //player.SetData("1");//����Ϊ��������1�� �����
                //player.Play();

                Notification m_notify = new Notification();
                m_notify.Refresh("atkOther", 1, 2, 1);//SenderID,targetID,SkillID
                MsgCenter.Instance.SendMsg("ByClent_Battle", m_notify);//����������������Ϣ
                ////TODO ���� List��ֵ����
                break;

        }
    }
}

/// <summary>
/// һ���н�ɫ���ݵĹ������NPC
/// </summary>
public class OtherPlayer : PlayerObj
{
    public OtherPlayer(PlayerInfo info) : base(info)
    {
        m_insID = info.ID;
        m_modelPath = info.m_res;
    }
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);
        base.CreateObj(type);
    }
}
