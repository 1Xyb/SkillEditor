using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Null = 0,//一般都是 -1
    Normal, //怪物
    Gather, //采集物
    Follow, //跟随物
    NPC,
    MaxValue = 99,
}

public class MonsterObj:ObjectBase
{
    public MonsterInfo m_info;//怪物数据
    public Enemy AI;

    //构造
    public MonsterObj(MonsterType type,MonsterInfo info)
    {
        info.m_type = type;
        m_info = info;
        m_insID = info.ID;
        m_modelPath = info.m_res;
    }
}

/// <summary>
/// 正常怪物
/// </summary>
public class Normal : MonsterObj
{
    //构造                                            基类的构造函数
    public Normal(MonsterInfo info) : base(MonsterType.Normal,info )
    {

    }

    public Normal(ObjectInfo info):base(MonsterType.Normal,new MonsterInfo(MonsterType.Normal, info)) { }

    //创建物体
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);//设置位置
        base.CreateObj(type);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        //正常怪物的组件   血条 蓝
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
/// 采集物
/// </summary>
public class Gather : MonsterObj
{
    public Gather(MonsterInfo info) : base(MonsterType.Gather, info) { }

    public Gather(ObjectInfo info) : base(MonsterType.Gather, new MonsterInfo(MonsterType.Gather,info)) { }

    //生成采集物
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);//设置物体位置
        base.CreateObj(type);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        //给采集物添加圆形检测范围
        StaticCircleCheck check = m_go.AddComponent<StaticCircleCheck>();
        check.m_target = World.Instance.m_plyer.m_go;//绑定检测目标=玩家
        //回调
        check.m_call = (isenter) =>
        {
            Debug.Log("玩家触发了我：" + m_info.m_res);
            Notification notification = new Notification();
            notification.Refresh("gather_trigger", m_info.ID);//采集物的ID  //发送消息的内容
            MsgCenter.Instance.SendMsg("ClientMsg", notification);//发消息给服务器
        };

        //接受客户端返回的采集物消息
        MsgCenter.Instance.AddListener("Refresh", RefreshNotify);

         //采集物数量
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
            //判断是否是当前的采集物
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
