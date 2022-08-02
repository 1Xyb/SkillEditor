using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 过度
/// </summary>
public enum Transition
{
    NullTransition=-1,
    SeePlayer,
    LostPlayer,//跟丢玩家
    Max=99
}

/// <summary>
/// 状态ID
/// </summary>
public enum StateID
{
    NullStateID=-1,
    Patrol,//巡逻
    Chase,//追赶
    Max=99
}

/// <summary>
/// 状态基类
/// </summary>
public abstract class FSMState 
{
    protected StateID stateID;//状态ID
    public StateID ID { get { return stateID; } } //自己封装的属性
    //条件库   当前状态可以通过哪些条件进入哪些状态
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
    //状态基类持有状态机系统
    protected FSMSystem fsm;

    public FSMState(FSMSystem fsm)
    {
        this.fsm = fsm;
    }

    /// <summary>
    /// 增
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="id"></param>
    public void AddTransition(Transition trans,StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.Log("不允许NullTransition");return;
        }
        if (id == StateID.NullStateID)
        {
            Debug.LogError("不允许NullStateID"); return;
        }
        if (map.ContainsKey(trans))
        {
            Debug.LogError("添加转换条件的时候，" + trans + "已经存在于map中"); return;
        }
        map.Add(trans, id);//添加
    }

    /// <summary>
    /// 删
    /// </summary>
    /// <param name="trans"></param>
    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("不允许NullTransition"); return;
        }
        if (map.ContainsKey(trans) == false)
        {
            Debug.LogError("删除转换条件的时候，" + trans + "不存在于map中"); return;
        }
        map.Remove(trans);
    }

    /// <summary>
    /// 查
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public StateID GetOutputState(Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    //技能前摇后摇
    public virtual void DoBeforeEntering() { }//进入状态钱
    public virtual void DoAfterLeaving() { }//离开状态后
    //抽象函数不实现函数主体       重写的时候实现
    public abstract void Act(GameObject npc);//当前状态的动作
    public abstract void Reason(GameObject npc);//当前状态的条件检查
}
