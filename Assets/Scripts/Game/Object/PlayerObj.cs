using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 只要是人，不管是自己还是别人，都是player
/// </summary>
public class PlayerObj : ObjectBase
{
    public PlayerInfo m_info;//玩家数据

    public PlayerObj(PlayerInfo info) { m_info = info; }

    public override void OnCreate()
    {
        base.OnCreate();
        m_go.name = "Player";
        //显示UI
        m_pate = m_go.AddComponent<UIPate>();//绑脚本
        m_pate.InitPate();//初始化
        m_pate.m_gather.SetActive(false);
        m_pate.text.SetActive(false);
        //设置数据
        m_pate.SetData(m_info.m_name, m_info.m_HP / m_info.m_hpMax, m_info.m_MP / m_info.m_mpMax);
    }
}

/// <summary>
/// 自己控制的player
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

        //atkActionPlay 监听消息 播放攻击动作
        MsgCenter.Instance.AddListener("atkActionPlay", (notify) => {
            if (notify.msg.Equals("ByServer"))
            {
                int skillid = (int)notify.data[0];//技能名字
                player.SetData(skillid.ToString());//重新设置技能 更新了当前技能组件
                player.Play();//播放
            }
        });
    }

    //Notification notify = new Notification();
    /// <summary>
    /// 摇杆控制移动
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

    //TODO   == 技能释放 方法  派发事件  通知服务器
    public void JoyButtonHandler(string btnName)
    {
        List<SkillBase> componentList;
        switch (btnName)
        {
            case "Attack":
                //player.SetData("1");//设置为技能名“1” 的组件
                //player.Play();

                Notification m_notify = new Notification();
                m_notify.Refresh("atkOther", 1, 2, 1);//SenderID,targetID,SkillID
                MsgCenter.Instance.SendMsg("ByClent_Battle", m_notify);//给服务器发攻击消息
                ////TODO 遍历 List赋值播放
                break;

        }
    }
}

/// <summary>
/// 一个有角色数据的怪物或者NPC
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
