using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机系统
/// </summary>
public class FSMSystem
{
    /// <summary>
    /// 库
    /// </summary>
    private Dictionary<StateID, FSMState> states = new Dictionary<StateID, FSMState>();

    private StateID currentStateID;
    private FSMState currentState;

    public void Update(GameObject npc)
    {
        currentState.Act(npc);//替换currentState实现动作切换
        currentState.Reason(npc);
    }

    /// <summary>
    /// 增
    /// </summary>
    /// <param name="s"></param>
    public void AddState(FSMState s)
    {
        if (s == null)
        {
            Debug.Log("FSMState不能为空");return;
        }
        if (currentState == null)
        {
            currentState = s;//默认状态
            currentStateID = s.ID;
        }
        if (states.ContainsKey(s.ID))
        {
            Debug.LogError("状态" + s.ID + "已经存在，无法重复添加"); return;
        }
        states.Add(s.ID, s);
    }

    /// <summary>
    /// 删
    /// </summary>
    /// <param name="id"></param>
    public void DeleteState(StateID id)
    {
        if (id == StateID.NullStateID)
        {
            Debug.LogError("无法删除空状态"); return;
        }
        if (states.ContainsKey(id) == false)
        {
            Debug.LogError("无法删除不存在的状态：" + id); return;
        }
        states.Remove(id);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="trans">条件</param>
    public void PerformTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("无法执行空的转换条件"); return;
        }
        StateID id = currentState.GetOutputState(trans);//拿到状态ID
        if (id == StateID.NullStateID)
        {
            Debug.LogWarning("当前状态" + currentStateID + "无法根据转换条件" + trans + "发生转换"); return;
        }
        if (states.ContainsKey(id) == false)
        {
            Debug.LogError("在状态机里面不存在状态" + id + "，无法进行状态转换！"); return;
        }
        FSMState state = states[id];
        currentState.DoAfterLeaving();//后摇
        currentState = state;//替换状态
        currentStateID = id;
        currentState.DoBeforeEntering();//前摇
    }
}
